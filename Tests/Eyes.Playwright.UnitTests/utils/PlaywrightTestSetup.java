package com.applitools.eyes.utils;

import com.applitools.connectivity.RestClient;
import com.applitools.eyes.*;
import com.applitools.eyes.metadata.SessionResults;
import com.applitools.eyes.playwright.ClassicRunner;
import com.applitools.eyes.playwright.Eyes;
import com.applitools.eyes.playwright.visualgrid.VisualGridRunner;
import com.applitools.eyes.selenium.StitchMode;
import com.applitools.utils.ClassVersionGetter;
import com.fasterxml.jackson.databind.DeserializationFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.microsoft.playwright.Browser;
import com.microsoft.playwright.BrowserType;
import com.microsoft.playwright.Page;
import com.microsoft.playwright.Playwright;
import org.testng.Assert;
import org.testng.ITest;
import org.testng.annotations.BeforeSuite;

import javax.ws.rs.HttpMethod;
import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.UriBuilder;
import java.net.URI;
import java.util.Arrays;

public class PlaywrightTestSetup extends ReportingTestSuite implements ITest {
    protected Eyes eyes;
    protected EyesRunner runner;
    protected Page page;

    private BatchInfo batch;
    private String apiKey;
    private String readApiKey;

    @BeforeSuite
    public void beforeSuite() {
        super.setGroupName("selenium");
        super.setSdkName("java_playwright");

        String name = System.getenv("APPLITOOLS_BATCH_NAME");
        if (name == null) name = "Eyes Playwright Java";
        batch = new BatchInfo(name);
        String id = System.getenv("APPLITOOLS_BATCH_ID");
        if (id != null) batch.setId(id);
        apiKey = System.getenv("APPLITOOLS_API_KEY");
        readApiKey = System.getenv("APPLITOOLS_API_KEY_READ");
    }

    protected Page initDriver(Boolean headless) {
        Playwright playwright = Playwright.create();

        Browser browser = playwright.chromium().launch(new BrowserType.LaunchOptions()
                .setHeadless(headless)
                .setIgnoreDefaultArgs(headless ? Arrays.asList("--hide-scrollbars") : null)
        );

        return browser.newPage();
    }

    protected void initEyes(boolean isVisualGrid, String stitching) {
        runner = isVisualGrid ? new VisualGridRunner(10) : new ClassicRunner();
        eyes = new Eyes(runner);
        eyes.setMatchTimeout(0);
        eyes.setApiKey(apiKey);
        eyes.setBatch(batch);
        eyes.setSaveNewTests(false);
        eyes.setHideScrollbars(true);
        eyes.setHideCaret(true);
        eyes.setStitchMode(stitching.equals("CSS") ? StitchMode.CSS : StitchMode.SCROLL);
    }

    public Page getPage() {
        return page;
    }

    @Override
    public String getTestName()  {
        return getClass().getName();
    }

    public SessionResults getTestInfo(TestResults results) {
        SessionResults sessionResults = null;
        try {
            sessionResults = TestUtils.getSessionResults(eyes.getApiKey(), results);
        } catch (Throwable e) {
            e.printStackTrace();
            Assert.fail("Exception appeared while getting session results");
        }
        return sessionResults;
    }
}
