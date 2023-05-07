namespace Applitools.Playwright.VisualGrid
{
    /**
 * Used to manage multiple Eyes sessions when working with the Ultrafast Grid
 */
    public class VisualGridRunner : EyesRunner {
    /**
     * name of the client sdk
     */
    protected static string BASE_AGENT_ID = "Eyes.Playwright.DotNet";

    /**
     * version of the client sdk
     */
    protected static string VERSION = ClassVersionGetter.CURRENT_VERSION;

    /**
     * spec-driver commands
     */
    protected static string[] COMMANDS = SpecDriverPlaywright.getMethodNames();

    /**
     * webSocket listener
     */
    private static readonly PSDKListener listener = PSDKListener.Instance;
    static readonly int DEFAULT_CONCURRENCY = 5;
    private bool isDisabled_;
    private RunnerOptions runnerOptions;

    public VisualGridRunner():
        this(new RunnerOptions().testConcurrency(DEFAULT_CONCURRENCY))
    {
    }

    public VisualGridRunner(int testConcurrency)
    {
        super(BASE_AGENT_ID, VERSION, null, COMMANDS, listener);
        this.runnerOptions = new RunnerOptions().testConcurrency(testConcurrency);
        EyesManagerSettings managerSettings = new EyesManagerSettings(null, testConcurrency, null);
        managerRef = commandExecutor.coreMakeManager(ManagerType.VISUAL_GRID.value, managerSettings);
    }

    public VisualGridRunner(RunnerOptions runnerOptions)
    {
        super(BASE_AGENT_ID, VERSION, null, COMMANDS, listener, runnerOptions);
        this.runnerOptions = runnerOptions;
        int testConcurrency = runnerOptions.getTestConcurrency() == null
            ? DEFAULT_CONCURRENCY
            : runnerOptions.getTestConcurrency();
        EyesManagerSettings managerSettings = new EyesManagerSettings(testConcurrency, null, null);
        managerRef = commandExecutor.coreMakeManager(ManagerType.VISUAL_GRID.value, managerSettings);
    }

    public StaleElementReferenceException getStaleElementException()
    {
        return new PlaywrightStaleElementReferenceException();
    }

    @Override

    public AbstractProxySettings getProxy()
    {
        return this.runnerOptions.getProxy();
    }

    public void setLogger(Logger logger)
    {
        this.logger = logger;
    }

    public void setProxy(AbstractProxySettings proxySettings)
    {
        if (proxySettings != null)
            this.runnerOptions = this.runnerOptions.proxy(proxySettings);
    }

    public void setAutProxy(AutProxySettings autProxy)
    {
        this.runnerOptions = this.runnerOptions.autProxy(autProxy);
    }

    public AutProxySettings getAutProxy()
    {
        return this.runnerOptions.getAutProxy();
    }

    public void setIsDisabled(boolean isDisabled)
    {
        this.isDisabled_ = isDisabled;
    }

    public bool getIsDisabled()
    {
        return this.isDisabled_;
    }

    override Refer getRefer()
    {
        return listener.getRefer();
    }
    }
}