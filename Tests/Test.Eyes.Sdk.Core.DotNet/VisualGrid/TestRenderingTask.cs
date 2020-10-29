using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.Utils;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Applitools.VisualGrid
{
    [Parallelizable(ParallelScope.All)]
    public class TestRenderingTask
    {
        //[Test]
        public void TestTryGetTextualData()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());
            DomAnalyzer.TryGetTextualData_(null, null, logger);
        }

        [Test]
        public void TestParseValidSVG()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());

            HashSet<string> allResourceUris = new HashSet<string>();
            DomAnalyzer.TextualDataResource tdr = new DomAnalyzer.TextualDataResource()
            {
                MimeType = "image/svg+xml",
                OriginalData = CommonUtils.ReadResourceBytes("Test.Eyes.Sdk.Core.DotNet.Resources.chevron.svg"),
                Data = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.chevron.svg"),
                Uri = new Uri("https://a.co/chevron.svg")
            };
            ConcurrentDictionary<string, HashSet<string>> cache = new ConcurrentDictionary<string, HashSet<string>>();
            DomAnalyzer.ParseSVG_(tdr, allResourceUris, logger, cache);
            Assert.AreEqual(0, allResourceUris.Count);
        }

        [Test]
        public void TestParseValidSVGWithBOM()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());

            HashSet<string> allResourceUris = new HashSet<string>();
            DomAnalyzer.TextualDataResource tdr = new DomAnalyzer.TextualDataResource()
            {
                MimeType = "image/svg+xml",
                OriginalData = CommonUtils.ReadResourceBytes("Test.Eyes.Sdk.Core.DotNet.Resources.ios.svg"),
                Data = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.ios.svg"),
                Uri = new Uri("https://a.co/ios.svg")
            };
            ConcurrentDictionary<string, HashSet<string>> cache = new ConcurrentDictionary<string, HashSet<string>>();
            DomAnalyzer.ParseSVG_(tdr, allResourceUris, logger, cache);
            Assert.AreEqual(0, allResourceUris.Count);
        }

        [Test]
        public void TestParseValidSVGWithLinks()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());

            HashSet<string> allResourceUris = new HashSet<string>();
            DomAnalyzer.TextualDataResource tdr = new DomAnalyzer.TextualDataResource()
            {
                MimeType = "image/svg+xml",
                OriginalData = CommonUtils.ReadResourceBytes("Test.Eyes.Sdk.Core.DotNet.Resources.applitools_logo_combined.svg"),
                Data = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.applitools_logo_combined.svg"),
                Uri = new Uri("https://applitools.github.io/demo/TestPages/VisualGridTestPage/applitools_logo_combined.svg")
            };
            ConcurrentDictionary<string, HashSet<string>> cache = new ConcurrentDictionary<string, HashSet<string>>();
            DomAnalyzer.ParseSVG_(tdr, allResourceUris, logger, cache);
            Assert.AreEqual(3, allResourceUris.Count);
        }

        [Test]
        public void TestParseInvalidSVGWithCommentOnTop()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());

            HashSet<string> allResourceUris = new HashSet<string>();
            DomAnalyzer.TextualDataResource tdr = new DomAnalyzer.TextualDataResource()
            {
                MimeType = "image/svg+xml",
                OriginalData = CommonUtils.ReadResourceBytes("Test.Eyes.Sdk.Core.DotNet.Resources.fa-regular-400.svg"),
                Data = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.fa-regular-400.svg"),
                Uri = new Uri("https://a.co/fa-regular-400.svg")
            };
            ConcurrentDictionary<string, HashSet<string>> cache = new ConcurrentDictionary<string, HashSet<string>>();
            DomAnalyzer.ParseSVG_(tdr, allResourceUris, logger, cache);
            Assert.AreEqual(0, allResourceUris.Count);
        }
    }
}
