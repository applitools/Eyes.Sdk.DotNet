using Applitools.Exceptions;
using Applitools.Selenium;
using Applitools.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools.VisualGrid.Demo
{
    public class Agilent
    {
        private static string appName = "Agilent";
        private static string batchName = "Agilent_32844";
        private static BatchInfo bi = new BatchInfo(batchName);

        [Test]
        public void TestAgilent()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();

            try
            {
                List<string> URLs = GetUrls();
                int ctr = 0;
                foreach (string url in URLs)
                {
                    ctr++;
                    EyesRunner runner = new VisualGridRunner(10);
                    Eyes eyes = new Eyes(runner);

                    RunVisualTest(driver, eyes, url, appName, "Test url: " + url);
                    runner.GetAllTestResults(false);

                    runner = null;
                    eyes = null;
                    GC.Collect();
                }
            }
            finally
            {
                driver.Quit();
            }
        }

        private static void RunVisualTest(IWebDriver driver, Eyes eyes, string url, string appName, string testName)
        {
            ConfigureEyes(eyes);
            ConfigureUltrafastGrid(eyes);
            TestUtils.SetupLogging(eyes);

            driver.Url = url;

            eyes.Open(driver, appName, testName, new Size(1000, 600));

            eyes.Check(Target.Window());

            eyes.CloseAsync();
        }

        private static void ConfigureEyes(Eyes eyes)
        {
            Selenium.Configuration conf = eyes.GetConfiguration();

            //Set the batch
            conf.SetBatch(bi);
            
            eyes.SetConfiguration(conf);
        }

        private static void ConfigureUltrafastGrid(Eyes eyes)
        {
            Selenium.Configuration conf = eyes.GetConfiguration();

            //Set browsers and devices for the UFG
            conf.AddBrowser(800, 600, BrowserType.CHROME);
            //conf.AddBrowser(1200, 800, BrowserType.FIREFOX);

            eyes.SetConfiguration(conf);
        }

        private static List<string> GetUrls()
        {
            List<string> URLs_A = new List<string>();
            //URLs_A.Add("https://www.agilent.com/home");
            //URLs_A.Add("https://www.agilent.com/en/products/gas-chromatography");
            //URLs_A.Add("https://www.agilent.com/en/products/gas-chromatography/gc-supplies");
            //URLs_A.Add("https://www.agilent.com/en/products/gas-chromatography/gc-supplies/capillary-tubing-accessories");
            //URLs_A.Add("https://www.agilent.com/en/products/gas-chromatography/gc-supplies/capillary-tubing-accessories/capillary-column-rinse-kit");
            //URLs_A.Add("https://www.agilent.com/en/product/chemical-standards");
            //URLs_A.Add("https://www.agilent.com/en/product/chemical-standards/agency-methods");
            //URLs_A.Add("https://www.agilent.com/en/product/chemical-standards/agency-methods/environmental-protection-agency-epa-methods ");
            //URLs_A.Add("https://www.agilent.com/en/product/chemical-standards/agency-methods/environmental-protection-agency-epa-methods/environmental-protection-agency-epa-european-eu-methods");
            //URLs_A.Add("https://www.agilent.com/en/solutions/energy-chemicals");
            //URLs_A.Add("https://www.agilent.com/en/solutions/energy-chemicals/polymers");
            //URLs_A.Add("https://www.agilent.com/en/solutions/energy-chemicals/polymers/portable-ftir");
            //URLs_A.Add("https://www.agilent.com/en/newsletters/accessagilent/2013/nov");
            URLs_A.Add("https://www.agilent.com/en/newsletters/accessagilent/2013/nov");
            URLs_A.Add("https://www.agilent.com/en/promotions-discounts-special-offers");
            URLs_A.Add("https://www.agilent.com/en/support");
            URLs_A.Add("https://www.agilent.com/en-us/training-events/eseminars");
            URLs_A.Add("https://www.agilent.com/en-us/training-events");
            URLs_A.Add("https://www.agilent.com/en-us/training-events/events/worldwideevents");
            URLs_A.Add("https://www.agilent.com/zh-cn/video/4210-mp-aes-video");
            URLs_A.Add("https://www.agilent.com/en/products");
            //URLs_A.Add("https://www.agilent.com/en/services");
            URLs_A.Add("https://www.agilent.com/en/contact-us/page");
            URLs_A.Add("https://www.agilent.com/store/");
            URLs_A.Add("https://www.agilent.com/en/support");
            URLs_A.Add("https://www.agilent.com/home/more-countries");
            URLs_A.Add("https://www.agilent.com/en/products/liquid-chromatography");
            URLs_A.Add("https://www.agilent.com/en/training-events/events/agilent-university");
            URLs_A.Add("https://www.agilent.com/en/products");
            URLs_A.Add("https://www.agilent.com/en/dako-products");
            URLs_A.Add("https://www.agilent.com/zh-cn/products/gas-chromatography");
            URLs_A.Add("https://www.agilent.com/about/companyinfo/index.html");
            URLs_A.Add("https://www.agilent.com/en/products/genomics-agilent");
            URLs_A.Add("https://www.agilent.com/zh-cn/solutions/environmental/water-analysis");
            URLs_A.Add("https://www.agilent.com/en/product/chemical-standards");
            URLs_A.Add("https://www.agilent.com/en-us/products");
            URLs_A.Add("https://www.agilent.com/en/products/gas-chromatography");
            URLs_A.Add("https://www.agilent.com/en/products/cell-analysis/seahorse-analyzers");
            URLs_A.Add("https://www.agilent.com/zh-cn/contact-us/page");
            URLs_A.Add("https://www.agilent.com/en/product/gc-columns");
            //URLs_A.Add("https://www.agilent.com/community/ikb");
            URLs_A.Add("https://www.agilent.com/en/product/small-molecule-columns/reversed-phase-hplc-columns/zorbax");
            URLs_A.Add("https://www.agilent.com/en/products/liquid-chromatography/lc-supplies/pumps/infinitylab-stay-safe-caps");
            URLs_A.Add("https://www.agilent.com/about/");
            URLs_A.Add("https://www.agilent.com/en/products/vacuum-technologies");
            URLs_A.Add("https://www.agilent.com/en/product/automated-electrophoresis");
            URLs_A.Add("https://www.agilent.com/en/training-events/eseminars");
            URLs_A.Add("https://www.agilent.com/en/products/liquid-chromatography-mass-spectrometry-lc-ms");
            URLs_A.Add("https://www.agilent.com/en/promotions/cary3500uv-vis-ebook");
            URLs_A.Add("https://www.agilent.com/en/product/automated-electrophoresis/bioanalyzer-systems/bioanalyzer-instrument/2100-bioanalyzer-instrument-228250");
            URLs_A.Add("https://www.agilent.com/en/promotions/flexible-repair-options");
            URLs_A.Add("https://www.agilent.com/en/promotions/lc-msd-iq");
            URLs_A.Add("https://www.agilent.com/en/products/gas-chromatography/gc-systems");
            URLs_A.Add("https://www.agilent.com/en/products/cell-analysis/seahorse-xf-consumables/kits-reagents-media/seahorse-xf-cell-mito-stress-test-kit");
            URLs_A.Add("https://www.agilent.com/cs/agilent/en/contact-us/united-states");
            URLs_A.Add("https://www.agilent.com/en/products/gas-chromatography-mass-spectrometry-gc-ms/gc-ms-instruments");
            URLs_A.Add("https://www.agilent.com/en/training-course-catalogs-calendars");
            URLs_A.Add("https://www.agilent.com/zh-cn/products/liquid-chromatography");
            URLs_A.Add("https://www.agilent.com/en/products/gas-chromatography-mass-spectrometry-gc-ms");
            URLs_A.Add("https://www.agilent.com/en/product/mutagenesis-cloning/mutagenesis-kits/site-directed-mutagenesis-kits/quikchange-ii-233117");
            URLs_A.Add("https://www.agilent.com/en/products/liquid-chromatography-mass-spectrometry-lc-ms/lc-ms-instruments/triple-quadrupole-lc-ms/ultivo-triple-quadrupole-lc-ms");
            URLs_A.Add("https://www.agilent.com/en/product/automated-electrophoresis/fragment-analyzer-systems");
            URLs_A.Add("https://www.agilent.com/en/promotions/990microgc");
            URLs_A.Add("https://www.agilent.com/zh-cn/support");
            URLs_A.Add("https://www.agilent.com/en/products/cell-analysis/how-seahorse-xf-analyzers-work");
            URLs_A.Add("https://www.agilent.com/en/crosslab");
            URLs_A.Add("https://www.agilent.com/en/product/sample-preparation");
            URLs_A.Add("https://www.agilent.com/en/products/liquid-chromatography/infinitylab-analytical-lc-solutions/1260-infinity-ii-lc-systems/1260-infinity-ii-lc-system");
            URLs_A.Add("https://www.agilent.com/en/products/ftir");
            //URLs_A.Add("https://www.agilent.com/community/technical/gc");
            //URLs_A.Add("https://www.agilent.com/community/technical/software");
            URLs_A.Add("https://www.agilent.com/en/products/ldir-chemical-imaging/ldir-chemical-imaging-system/8700-ldir-chemical-imaging-system");
            URLs_A.Add("https://www.agilent.com/en/service/laboratory-services/maintenance-repair");
            URLs_A.Add("https://www.agilent.com/en/training-events/events/world-wide-events");
            URLs_A.Add("https://www.agilent.com/en/training-events/eseminars/gc-gc-ms-webinars");
            URLs_A.Add("https://www.agilent.com/en/product/next-generation-sequencing/sureselect-innovate-inspire-trust");
            URLs_A.Add("https://www.agilent.com/en/product/automated-electrophoresis/bioanalyzer-systems/bioanalyzer-dna-kits-reagents/bioanalyzer-high-sensitivity-dna-analysis-228249");
            URLs_A.Add("https://www.agilent.com/en/product/automated-electrophoresis/tapestation-systems/tapestation-instruments/4200-tapestation-system-228263");
            URLs_A.Add("https://www.agilent.com/en/promotions/new-store-capabilities");
            //URLs_A.Add("https://www.agilent.com/en/promotions/agilent-4200-tapestation-system");
            URLs_A.Add("https://www.agilent.com/en-us/newsletters");
            URLs_A.Add("https://www.agilent.com/en/product/small-molecule-columns");
            URLs_A.Add("https://www.agilent.com/en/product/immunohistochemistry");
            URLs_A.Add("https://www.agilent.com/en/products/software-informatics/openlab-software-suite");
            URLs_A.Add("https://www.agilent.com/zh-cn/promotions/effortless-ngs-sample-qc");
            URLs_A.Add("https://www.agilent.com/en/products/liquid-chromatography-mass-spectrometry-lc-ms/lc-ms-instruments");
            URLs_A.Add("https://www.agilent.com/about/brands/");
            URLs_A.Add("https://www.agilent.com/en/products/liquid-chromatography-mass-spectrometry-lc-ms/lc-ms-instruments/triple-quadrupole-lc-ms/6495c-triple-quadrupole-lc-ms");
            URLs_A.Add("https://www.agilent.com/en/product/automated-electrophoresis/bioanalyzer-systems/bioanalyzer-rna-kits-reagents/bioanalyzer-rna-analysis-228256");
            URLs_A.Add("https://www.agilent.com/en/product/small-molecule-columns/reversed-phase-hplc-columns/infinitylab-poroshell-120");
            URLs_A.Add("https://www.agilent.com/zh-cn/products/ldir-chemical-imaging/ldir-chemical-imaging-system/8700-ldir-chemical-imaging-system");
            URLs_A.Add("https://www.agilent.com/en/products/cell-analysis/how-to-run-an-assay");
            URLs_A.Add("https://www.agilent.com/en-us/library/safetydatasheets?n=133");
            URLs_A.Add("https://www.agilent.com/en/products/uv-vis-uv-vis-nir/uv-vis-uv-vis-nir-systems");
            URLs_A.Add("https://www.agilent.com/en/products/software-informatics");
            URLs_A.Add("https://www.agilent.com/en-us/library/usermanuals?n=135");
            URLs_A.Add("https://www.agilent.com/en/products/mass-spectrometry/gc-ms-instruments/5977b-gc-msd");
            //URLs_A.Add("https://www.agilent.com/community/technical/lc");
            URLs_A.Add("https://www.agilent.com/search/?Ntt=db-1");
            URLs_A.Add("https://www.agilent.com/search/?Ntt=123");

            return URLs_A;
        }
    }
}
