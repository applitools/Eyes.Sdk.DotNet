using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Applitools.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools.Images.Tests
{
    [TestFixture]
    public class CustomerImagesTests
    {
        private BatchInfo batch_;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            batch_ = new BatchInfo("CustomerImagesTests");
        }

        [Test]
        public void TestPDF()
        {
            Eyes eyes = Setup();
            eyes.Open("CustomerImagesTests", "Check PDF");
            using (Document document = new Document(CommonUtils.ReadResourceStream("Customers.Test.Eyes.Selenium.Resources.gre_research_validity_data.pdf")))
            {
                for (int i = 0; i < document.Pages.Count; ++i)
                {
                    Page currentPage = document.Pages[i];
                    using (Bitmap pdfImage = new Bitmap(currentPage.Render(700, 700, new RenderingSettings())))
                    {
                        //pdfImage.Save(Path.Combine(FOLDER_PATH, $"temp_{fileName}_{i+1}.png"));
                        eyes.CheckImage(pdfImage, $"test page {i + 1}");
                    }
                }
            }
            Teardown(eyes);
        }

        public Eyes Setup()
        {
            Eyes eyes = new Eyes();
            eyes.Batch = batch_;
            eyes.SetLogHandler(new StdoutLogHandler());
            eyes.Logger.Log("running test: " + TestContext.CurrentContext.Test.Name);
            return eyes;
        }

        public void Teardown(Eyes eyes)
        {
            try
            {
                TestResults results = eyes.Close();
                eyes.Logger.Log("Mismatches: " + results.Mismatches);
            }
            finally
            {
                eyes.Abort();
            }
        }
    }
}
