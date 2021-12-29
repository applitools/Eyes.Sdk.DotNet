using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Eyes.Universal.Sdk.DotNet.Commands.Client;
using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet
{
    public class SocketMessageManager
    {
        private readonly WebSocketClient _socketClient;
        private static Process nativeProc_;

        public SocketMessageManager()
        {
            _socketClient = new WebSocketClient();
        }

        public async Task SendRequest(CoreMakeSdk CoreMakeSdk)
        {
        }

        public async Task<CoreMakeManagerResponse> SendRequest(CoreMakeManager coreMakeManager)
        {
            TryStartProcess_();
            //var coreMakeManager = CreateCoreMakeManager();
            var coreMakeManagerJson = ConvertData(coreMakeManager);

            await _socketClient.SendData(coreMakeManagerJson);

            CoreMakeManagerResponse coreMakeManagerResponse = null;

            do
            {
                var text = await _socketClient.ReceiveMessage();

                coreMakeManagerResponse = JsonConvert.DeserializeObject<CoreMakeManagerResponse>(text);
            } while (coreMakeManagerResponse?.Payload?.Result?.ApplitoolsRefId != coreMakeManager.Key);

            return coreMakeManagerResponse;
        }

        private void TryStartProcess_()
        {
            if (nativeProc_ == null || nativeProc_.HasExited)
            {
                string path = Assembly.GetExecutingAssembly().Location;
                string osVersion = Environment.OSVersion.VersionString.ToLower();
                string os, suffix;
                if (osVersion.Contains("windows")) { os = "win-x64"; suffix = "win.exe"; }
                else if (osVersion.Contains("mac")) { os = "mac-x64"; suffix = "macos"; }
                else { os = "linux-x64"; suffix = "linux"; }

                path = Path.Combine(Path.GetDirectoryName(path), "runtimes", os, "native", "eyes-universal-" + suffix);
                nativeProc_ = Process.Start(path);
            }
        }

        public string ConvertData<T>(T data)
        {
            string json = JsonConvert.SerializeObject(data);
            return json;
        }

        private CoreMakeSdk CreateCoreMakeSdk()
        {
            var result = new CoreMakeSdk();
            result.AddOrUpdatePayload("name", "netSDK");
            result.AddOrUpdatePayload("protocol", "webdriver");
            result.AddOrUpdatePayload("version", Assembly.GetEntryAssembly().GetName().Version.ToString());
            result.AddOrUpdatePayload("cwd", Environment.CurrentDirectory);

            return result;
        }
        private CoreMakeManager CreateCoreMakeManager()
        {
            var result = new CoreMakeManager();
            result.Key = Guid.NewGuid().ToString();
            result.AddOrUpdatePayload("type", "classic");

            return result;
        }
    }
}