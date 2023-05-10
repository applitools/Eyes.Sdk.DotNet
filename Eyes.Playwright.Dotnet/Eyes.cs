using System.Collections.Generic;
using Applitools.Commands;
using Applitools.Playwright.Fluent;
using Applitools.Playwright.Universal.Dto;
using Applitools.Playwright.Universal.Mapper;
using Applitools.Universal;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Microsoft.Playwright;

namespace Applitools.Playwright
{
    public class Eyes : EyesBase
    {
        /// <summary>
        /// The runner.
        /// </summary>
        private readonly EyesRunner runner_;

        /// <summary>
        /// The command executor to execute the commands.
        /// </summary>
        private readonly CommandExecutor commandExecutor_;

        /// <summary>
        /// The configuration that will be used in eyes related requests.
        /// </summary>
        private Configuration configuration_ = new Configuration();

        /// <summary>
        /// This reference has to be used in eyes related requests
        /// (Eyes.Check, Eyes.Locate, Eyes.ExtractTextRegions, Eyes.ExtractText, Eyes.Close, Eyes.Abort)
        /// </summary>
        private Reference eyesRef_;

        /// <summary>
        /// Is eyes closed, used to check abort after close.
        /// </summary>
        private bool isClosed_;

        /// <summary>
        /// The driver reference associated with the test.
        /// </summary>
        private Driver driver_;

        /// <summary>
        /// Instantiates a new Eyes.
        /// </summary>
        public Eyes() : this(new ClassicRunner())
        {
        }

        /// <summary>
        /// Instantiates a new Eyes.
        /// </summary>
        /// <param name="runner">The runner</param>
        public Eyes(EyesRunner runner)
        {
            runner_ = runner ?? new ClassicRunner();
            commandExecutor_ = runner_.CommandExecutor;
        }

        /// <summary>
        /// Starts a test.
        /// </summary>
        /// <param name="page">The web driver that controls the browser hosting the application under test.</param>
        /// <returns>The Page</returns>
        public IPage Open(IPage page)
        {
            return Open(page, configuration_.AppName, configuration_.TestName);
        }
        
        public IPage Open(IPage page, string appName, string testName)
        {
            return Open(page, appName, testName, configuration_.ViewportSize);
        }

        public IPage Open(IPage page, string appName, string testName, RectangleSize viewportSize)
        {
            if (IsDisabled)
            {
                return page;
            }

            driver_ = new Driver(page);
            Refer refer = GetRefer_();
            driver_.ApplitoolsRefId = refer.Ref(page, driver_.Root);
            driver_.setWebDriverProxy(configuration_.getWebDriverProxy());

            configuration_.setAppName(appName).setTestName(testName);
            if (viewportSize != null && !viewportSize.isEmpty())
            {
                configuration_.setViewportSize(new RectangleSize(viewportSize));
            }

            ConfigurationDto configurationDto = ConfigurationMapper
                .toConfigurationDto(configuration_, runner_.isDontCloseBatches());
            OpenSettingsDto settingsDto =
                SettingsMapper.toOpenSettingsDto(configuration_, runner_.isDontCloseBatches());

            /*
                var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(Runner.ManagerApplitoolsRefId, driver);
    
                var openEyesResponsePayload = Runner.SendRequest<EyesManagerOpenEyes, EyesManagerOpenEyesResponse>(eyesManagerOpenEyes).Payload;
              */

            eyesRef_ = commandExecutor_.managerOpenEyes(runner_.getManagerRef(), driver_, settingsDto,
                configurationDto);
            IsOpen = true;
            return page;
        }

        /**
     * Takes multiple screenshots at once (given all <code>ICheckSettings</code> objects are on the same level).
     * @param checkSettings Multiple <code>ICheckSettings</code> object representing different regions in the viewport.
     */
        public void Check(params ICheckSettings[] checkSettings)
        {
            foreach (ICheckSettings cSs in checkSettings)
            {
                Check(cSs);
            }
        }

        public void Check(string tag, ICheckSettings checkSettings)
        {
            Check(checkSettings.WithName(tag));
        }

        public void Check(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                return;
            }

            ArgumentGuard.NotNull(checkSettings, nameof(checkSettings));
            ArgumentGuard.NotOfType(checkSettings, typeof(IPlaywrightCheckSettings), nameof(checkSettings));
            IPlaywrightCheckSettings playwrightCheckSettings = (IPlaywrightCheckSettings)checkSettings;

            EyesConfig checkSettingsDto = PlaywrightCheckSettingsMapper.ToCheckSettingsDto(
                playwrightCheckSettings,
                Configure(), GetRefer_(), driver_.getRoot());

            checkDto(checkSettingsDto, driver_);
        }

        public List<string> ExtractText(params BaseOcrRegion[] ocrRegions)
        {
            if (!IsOpen)
            {
                Abort();
                throw new EyesException("Eyes not open");
            }

            List<OCRExtractSettingsDto> ocrExtractSettingsDtoList = PlaywrightOCRExtractSettingsDtoMapper
                .toOCRExtractSettingsDtoList(Arrays.asList(ocrRegions), GetRefer_(), driver_.getRoot());
            ConfigurationDto configurationDto = ConfigurationMapper
                .toConfigurationDto(Configure(), false);

            return ExtractTextDto(driver_, ocrExtractSettingsDtoList, configurationDto);
        }

        /**
     * Locate text. Formerly known as extractTextRegions.
     *
     * @param textRegionSettings  the image from which the location of the text will be extracted.
     * @return mapping of the located text regions
     */
        public Map<string, List<TextRegion>> ExtractTextRegions(TextRegionSettings textRegionSettings)
        {
            OCRSearchSettingsDto ocrSearchSettingsDto =
                OCRSearchSettingsMapper.toOCRSearchSettingsDto(textRegionSettings);
            ConfigurationDto configurationDto = ConfigurationMapper
                .toConfigurationDto(Configure(), false);

            return locateTextDto(driver_, ocrSearchSettingsDto, configurationDto);
        }

        /**
     * Locate.
     * @param visualLocatorSettings the locator settings.
     * @return mapping of the region locations.
     */
        public Map<string, List<Region>> Locate(VisualLocatorSettings visualLocatorSettings)
        {
            ArgumentGuard.notNull(visualLocatorSettings, "visualLocatorSettings");

            if (!IsOpen)
            {
                this.abort();
                throw new EyesException("Eyes not open");
            }

            VisualLocatorSettingsDto visualLocatorSettingsDto = VisualLocatorSettingsMapper
                .toVisualLocatorSettingsDto(visualLocatorSettings);

            ConfigurationDto configurationDto = ConfigurationMapper
                .toConfigurationDto(Configure(), false);

            return locateDto(driver_, visualLocatorSettingsDto, configurationDto);
        }

        /// <summary>
        /// Close test results.
        /// </summary>
        /// <param name="shouldThrowException">Throw exception if a diff is found</param>
        /// <returns>The test results</returns>
        public override TestResults Close(bool shouldThrowException = true)
        {
            if (IsDisabled)
            {
                return null;
            }

            if (!IsOpen)
            {
                throw new EyesException("Eyes not open");
            }

            ConfigurationDto configurationDto = ConfigurationMapper
                .toConfigurationDto(configuration_, runner_.isDontCloseBatches());
            CloseSettingsDto settings = SettingsMapper.toCloseSettingsDto(getConfiguration());

            commandExecutor_.close(eyesRef_, settings, configurationDto);
            GetResultsSettings getResultsSettings = new GetResultsSettings(shouldThrowException);
            List<CommandEyesGetResultsResponseDto> getResultsResponse =
                commandExecutor_.eyesGetResults(eyesRef_, getResultsSettings);

            eyesRef_ = null;
            isClosed_ = true;
            IsOpen = false;
            GetRefer_().Destroy(driver_.Root);
            driver_ = null;

            List<TestResults> testResults =
                TestResultsMapper.toTestResultsList(getResultsResponse, getApiKey(), getServerUrl(), getProxy());
            if (testResults == null)
                return null;
            testResults.forEach(
                testResults1->runner.logSessionResultsAndThrowException(shouldThrowException, testResults1));
            return testResults.get(0);
        }

        public Configuration Configure()
        {
            return configuration_;
        }
        
        /**
     * Sets the branch in which the baseline for subsequent test runs resides.
     * If the branch does not already exist it will be created under the
     * specified parent branch (see {@link #setParentBranchName}).
     * Changes to the baseline or model of a branch do not propagate to other
     * branches.
     * @param branchName Branch name or {@code null} to specify the default branch.
     */
        public Configuration setBranchName(string branchName)
        {
            return Configure().setBranchName(branchName);
        }

        public Configuration setAgentId(string agentId)
        {
            return Configure().setAgentId(agentId);
        }

        /**
     * @return The current branch (see {@link #setBranchName(String)}).
     */
        public string getBranchName()
        {
            return Configure().getBranchName();
        }

        public string getAgentId()
        {
            return Configure().getAgentId();
        }

        /**
     * Sets the branch under which new branches are created. (see {@link
     * #setBranchName(String)}.
     * @param branchName Branch name or {@code null} to specify the default branch.
     */
        public Configuration setParentBranchName(string branchName)
        {
            return Configure().setParentBranchName(branchName);
        }

        /**
     * @return The name of the current parent branch under which new branches
     * will be created. (see {@link #setParentBranchName(String)}).
     */
        public string getParentBranchName()
        {
            return Configure().getParentBranchName();
        }

        /**
     * Sets the branch under which new branches are created. (see {@link
     * #setBranchName(String)}.
     * @param branchName Branch name or {@code null} to specify the default branch.
     */
        public Configuration setBaselineBranchName(string branchName)
        {
            return Configure().setBaselineBranchName(branchName);
        }

        /**
     * @return The name of the current parent branch under which new branches
     * will be created. (see {@link #setBaselineBranchName(String)}).
     */
        public string getBaselineBranchName()
        {
            return Configure().getBaselineBranchName();
        }

        /**
     * Automatically save differences as a baseline.
     * @param saveDiffs Sets whether to automatically save differences as baseline.
     */
        public Configuration setSaveDiffs(bool saveDiffs)
        {
            return Configure().setSaveDiffs(saveDiffs);
        }

        /**
     * Returns whether to automatically save differences as a baseline.
     * @return Whether to automatically save differences as baseline.
     */
        public bool getSaveDiffs()
        {
            return Configure().getSaveDiffs();
        }

        /**
     * Sets the maximum time (in ms) a match operation tries to perform a match.
     * @param ms Total number of ms to wait for a match.
     */
        public Configuration setMatchTimeout(int ms)
        {
            return Configure().setMatchTimeout(ms);
        }

        /**
     * @return The maximum time in ms.
     */
        public int getMatchTimeout()
        {
            return Configure().getMatchTimeout();
        }

        /**
     * Set whether or not new tests are saved by default.
     * @param saveNewTests True if new tests should be saved by default. False otherwise.
     */
        public Configuration setSaveNewTests(boolean saveNewTests)
        {
            return Configure().setSaveNewTests(saveNewTests);
        }

        /**
     * @return True if new tests are saved by default.
     */
        public boolean getSaveNewTests()
        {
            return Configure().getSaveNewTests();
        }

        /**
     * Set whether or not failed tests are saved by default.
     * @param saveFailedTests True if failed tests should be saved by default, false otherwise.
     */
        public Configuration setSaveFailedTests(boolean saveFailedTests)
        {
            return Configure().setSaveFailedTests(saveFailedTests);
        }

        /**
     * @return True if failed tests are saved by default.
     */
        public boolean getSaveFailedTests()
        {
            return Configure().getSaveFailedTests();
        }

        /**
     * Sets the batch in which context future tests will run or {@code null}
     * if tests are to run standalone.
     * @param batch The batch info to set.
     */
        public Configuration setBatch(BatchInfo batch)
        {
            if (IsDisabled)
            {
                return Configure();
            }

            return Configure().setBatch(batch);
        }

        /**
     * @return The currently set batch info.
     */
        public BatchInfo getBatch()
        {
            return Configure().getBatch();
        }


        /**
     * Updates the match settings to be used for the session.
     * @param defaultMatchSettings The match settings to be used for the session.
     */
        public Configuration setDefaultMatchSettings(ImageMatchSettings
            defaultMatchSettings)
        {
            ArgumentGuard.notNull(defaultMatchSettings, "defaultMatchSettings");

            return Configure().setDefaultMatchSettings(defaultMatchSettings);
        }

        /**
     * @return The match settings used for the session.
     */
        public ImageMatchSettings getDefaultMatchSettings()
        {
            return Configure().getDefaultMatchSettings();
        }

        /**
     * This function is deprecated. Please use {@link #setDefaultMatchSettings} instead.
     * <p>
     * The test-wide match level to use when checking application screenshot
     * with the expected output.
     * @param matchLevel The match level setting.
     * @return The match settings used for the session.
     * @see com.applitools.eyes.MatchLevel
     */
        public Configuration setMatchLevel(MatchLevel matchLevel)
        {
            Configuration config = Configure();
            config.getDefaultMatchSettings().setMatchLevel(matchLevel);
            return config;
        }

        public Configuration setIgnoreDisplacements(boolean isIgnoreDisplacements)
        {
            return Configure().setIgnoreDisplacements(isIgnoreDisplacements);
        }

        public Configuration setAccessibilityValidation(AccessibilitySettings accessibilityValidation)
        {
            return Configure().setAccessibilityValidation(accessibilityValidation);
        }

        public Configuration setUseDom(boolean useDom)
        {
            return Configure().setUseDom(useDom);
        }

        public Configuration setEnablePatterns(boolean enablePatterns)
        {
            return Configure().setEnablePatterns(enablePatterns);
        }

        /**
     * @return The test-wide match level.
     * @deprecated Please use{@link #getDefaultMatchSettings} instead.
     */
        public MatchLevel getMatchLevel()
        {
            return Configure().getDefaultMatchSettings().getMatchLevel();
        }

        public boolean getIgnoreDisplacements()
        {
            return Configure().getIgnoreDisplacements();
        }

        public AccessibilitySettings getAccessibilityValidation()
        {
            return Configure().getAccessibilityValidation();
        }

        public boolean getUseDom()
        {
            return Configure().getUseDom();
        }

        public boolean getEnablePatterns()
        {
            return Configure().getEnablePatterns();
        }

        /**
     * @return Whether to ignore or the blinking caret or not when comparing images.
     */
        public boolean getIgnoreCaret()
        {
            bool ignoreCaret = Configure().getDefaultMatchSettings().getIgnoreCaret();
            return ignoreCaret == null || ignoreCaret;
        }

        /**
     * Sets the ignore blinking caret value.
     * @param value The ignore value.
     */
        public Configuration setIgnoreCaret(boolean value)
        {
            Configuration config = Configure();
            Configure().getDefaultMatchSettings().setIgnoreCaret(value);
            return config;
        }

        /**
     * Returns the stitching overlap in pixels.
     */
        public int getStitchOverlap()
        {
            return Configure().getStitchOverlap();
        }

        /**
     * Sets the stitching overlap in pixels.
     * @param pixels The width (in pixels) of the overlap.
     */
        public Configuration setStitchOverlap(int pixels)
        {
            return Configure().setStitchOverlap(pixels);
        }

        /**
     * @param hostOS The host OS running the AUT.
     */
        public Configuration setHostOS(string hostOS)
        {
            Configuration config = Configure();
            if (hostOS == null || hostOS.isEmpty())
            {
                config.setHostOS(null);
            }
            else
            {
                config.setHostOS(hostOS.trim());
            }

            return config;
        }

        /**
     * @return get the host OS running the AUT.
     */
        public string getHostOS()
        {
            return Configure().getHostOS();
        }

        /**
     * @param hostApp The application running the AUT (e.g., Chrome).
     */
        public Configuration setHostApp(string hostApp)
        {
            Configuration config = Configure();
            if (hostApp == null || hostApp.isEmpty())
            {
                config.setHostApp(null);
            }
            else
            {
                config.setHostApp(hostApp.trim());
            }

            return config;
        }

        /**
     * @return The application name running the AUT.
     */
        public string getHostApp()
        {
            return Configure().getHostApp();
        }

        /**
     * @param baselineName If specified, determines the baseline to compare
     *                     with and disables automatic baseline inference.
     * @deprecated Only available for backward compatibility. See {@link #setBaselineEnvName(String)}.
     */
        public void setBaselineName(string baselineName)
        {
            setBaselineEnvName(baselineName);
        }

        /**
     * @return The baseline name, if specified.
     * @deprecated Only available for backward compatibility. See {@link #getBaselineEnvName()}.
     */
        @SuppressWarnings("UnusedDeclaration")

        public string getBaselineName()
        {
            return getBaselineEnvName();
        }

        /**
     * If not {@code null}, determines the name of the environment of the baseline.
     * @param baselineEnvName The name of the baseline's environment.
     */
        public Configuration setBaselineEnvName(string baselineEnvName)
        {
            Configuration config = Configure();
            if (baselineEnvName == null || baselineEnvName.isEmpty())
            {
                config.setBaselineEnvName(null);
            }
            else
            {
                config.setBaselineEnvName(baselineEnvName.trim());
            }

            return config;
        }
        
        public void CloseAsync()
        {
            if (IsDisabled)
            {
                return;
            }

            if (!IsOpen)
            {
                throw new EyesException("Eyes not open");
            }

            ConfigurationDto configurationDto = ConfigurationMapper
                .toConfigurationDto(configuration_, runner_.isDontCloseBatches());
            CloseSettingsDto settings = SettingsMapper.toCloseSettingsDto(getConfiguration());

            commandExecutor_.close(eyesRef_, settings, configurationDto);
            eyesRef_ = null;
            isClosed_ = true;
            IsOpen = false;
            GetRefer_().destroy(driver_.getRoot());
            driver_ = null;
        }

        public override void AbortAsync()
        {
            if (!isClosed_ && IsOpen)
            {
                CloseSettingsDto settings = SettingsMapper.toCloseSettingsDto(getConfiguration());
                commandExecutor_.Abort(eyesRef_, settings);

                eyesRef_ = null;
                IsOpen = false;
                GetRefer_().destroy(driver_.getRoot());
                driver_ = null;
            }
        }

        public override TestResults Abort()
        {
            if (!isClosed_ && IsOpen)
            {
                CloseSettingsDto settings = SettingsMapper.toCloseSettingsDto(getConfiguration());
                commandExecutor_.abort(eyesRef_, settings);

                GetResultsSettings getResultsSettings = new GetResultsSettings(false);
                List<CommandEyesGetResultsResponseDto> getResultsResponse =
                    commandExecutor_.eyesGetResults(eyesRef_, getResultsSettings);

                eyesRef_ = null;
                IsOpen = false;
                GetRefer_().destroy(driver_.getRoot());
                driver_ = null;

                List<TestResults> testResults =
                    TestResultsMapper.toTestResultsList(getResultsResponse, getApiKey(), getServerUrl(), getProxy());
                if (testResults != null)
                {
                    return testResults.isEmpty() ? null : testResults.get(0);
                }
            }

            return null;
        }

        private void checkDto(CheckSettingsDto checkSettingsDto, ITargetDto driverTargetDto)
        {
            if (IsDisabled)
            {
                return;
            }

            if (!IsOpen)
            {
                this.Abort();
                throw new EyesException("you must call open() before checking");
            }

            ArgumentGuard.NotNull(checkSettingsDto, "checkSettings");

            ConfigurationDto configurationDto = ConfigurationMapper
                .toConfigurationDto(configuration, runner.isDontCloseBatches());
            commandExecutor.eyesCheck(eyesRef, driverTargetDto, checkSettingsDto, configurationDto);
        }

        private List<string> extractTextDto(ITargetDto target, List<OCRExtractSettingsDto> settings,
            ConfigurationDto config)
        {
            return commandExecutor.extractText(target, settings, config);
        }

        private Map<string, List<TextRegion>> locateTextDto(ITargetDto target, OCRSearchSettingsDto settings,
            ConfigurationDto config)
        {
            return commandExecutor.locateText(target, settings, config);
        }

        private Map<string, List<Region>> locateDto(ITargetDto target, VisualLocatorSettingsDto settings,
            ConfigurationDto config)
        {
            Map<string, List<RegionDto>> response = commandExecutor.locate(target, settings, config);
            return LocateMapper.toLocateFromDto(response);
        }

        private Refer GetRefer_()
        {
            return refer_;
            // try
            // {
            //     Method getRef = runner.getClass().getDeclaredMethod("getRefer");
            //     getRef.setAccessible(true);
            //     return (Refer)getRef.invoke(runner);
            // }
            // catch (IllegalAccessException |
            //
            // InvocationTargetException | NoSuchMethodException e) {
            //     string message =
            //         GeneralUtils.createErrorMessageFromExceptionWithText(e, "got an error trying to getRef!");
            //     throw new EyesException(message);
            // }
        }
    }
}