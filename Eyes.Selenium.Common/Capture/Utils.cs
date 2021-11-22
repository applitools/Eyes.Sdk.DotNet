using Applitools.Utils;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Applitools.Selenium.Capture
{
    internal static class Utils
    {
        private static readonly string DOM_SCRIPTS_WRAPPER = "return ({0})({1});";
        internal static TimeSpan CAPTURE_TIMEOUT = TimeSpan.FromMinutes(5);

        internal static string RunDomScript(Logger logger, IEyesJsExecutor jsExecutor, string[] testIds,
            string domScript, object domScriptArguments, object pollingScriptArguments, string pollingScript)
        {
            logger.Log(TraceLevel.Info, testIds, Stage.General, StageType.DomScript, new { message = "Starting" });
            if (domScriptArguments == null)
            {
                domScriptArguments = new { };
            }

            string domScriptWrapped = string.Format(DOM_SCRIPTS_WRAPPER, domScript, JsonConvert.SerializeObject(domScriptArguments));
            string pollingScriptWrapped = string.Format(DOM_SCRIPTS_WRAPPER, pollingScript, JsonConvert.SerializeObject(pollingScriptArguments));

            try
            {
                string resultAsString = (string)jsExecutor.ExecuteScript(domScriptWrapped);
                CaptureStatus scriptResponse = JsonConvert.DeserializeObject<CaptureStatus>(resultAsString);
                CaptureStatusEnum status = scriptResponse.Status;
                Stopwatch stopwatch = Stopwatch.StartNew();
                while (status == CaptureStatusEnum.WIP && stopwatch.Elapsed < CAPTURE_TIMEOUT)
                {
                    Thread.Sleep(200);
                    logger.Log(TraceLevel.Info, testIds, Stage.General, StageType.DomScript, new { message = "Polling" });
                    resultAsString = (string)jsExecutor.ExecuteScript(pollingScriptWrapped);
                    scriptResponse = JsonConvert.DeserializeObject<CaptureStatus>(resultAsString);
                    status = scriptResponse.Status;
                }

                if (status == CaptureStatusEnum.ERROR)
                {
                    throw new EyesException("DomSnapshot Error: " + scriptResponse.Error);
                }

                if (stopwatch.Elapsed > CAPTURE_TIMEOUT)
                {
                    throw new EyesException("DOM capture timeout.");
                }

                if (status == CaptureStatusEnum.SUCCESS)
                {
                    return scriptResponse.Value.ToString();
                }

                StringBuilder value = new StringBuilder();
                string chunk;
                while (status == CaptureStatusEnum.SUCCESS_CHUNKED && !scriptResponse.Done && stopwatch.Elapsed < CAPTURE_TIMEOUT)
                {
                    logger.Log(TraceLevel.Info, testIds, Stage.General, StageType.DomScript, new { message = "Chunks Polling" });
                    chunk = scriptResponse.Value.ToString();
                    value.Append(chunk);
                    resultAsString = (string)jsExecutor.ExecuteScript(pollingScriptWrapped);
                    scriptResponse = JsonConvert.DeserializeObject<CaptureStatus>(resultAsString);
                    status = scriptResponse.Status;
                }

                if (status == CaptureStatusEnum.ERROR)
                {
                    throw new EyesException("DomSnapshot Error: " + scriptResponse.Error);
                }

                if (stopwatch.Elapsed > CAPTURE_TIMEOUT)
                {
                    throw new EyesException("Domsnapshot Timed out");
                }

                chunk = scriptResponse.Value.ToString();
                value.Append(chunk);
                return value.ToString();
            }
            finally
            {
                logger.Log(TraceLevel.Info, testIds, Stage.General, StageType.DomScript, new { message = "Done" });
            }
        }

    }
}
