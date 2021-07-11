using Applitools.VisualGrid;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Applitools.Selenium.Tests.WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            btnTestClassic.Enabled = false;
            EyesRunner runner = new ClassicRunner();
            RunTest_(runner);
            btnTestClassic.Enabled = true;
        }

        private void btnTestUFG_Click(object sender, EventArgs e)
        {
            btnTestUFG.Enabled = false;
            EyesRunner runner = new VisualGridRunner();
            RunTest_(runner);
            btnTestUFG.Enabled = true;
        }

        private void RunTest_(EyesRunner runner)
        {
            Eyes eyes = new Eyes(runner);
            IWebDriver driver = new ChromeDriver();
            driver.Url = tbUrl.Text;
            try
            {
                string url = tbUrl.Text.Split('?')[0];
                eyes.Open(driver, "WinForms Test", url, new Size(1000, 700));
                eyes.Check(Target.Window().SendDom().Fully(false));
                eyes.Close();
            }
            catch (Exception e)
            {
                tbOutput.Text = e.ToString();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }
    }
}
