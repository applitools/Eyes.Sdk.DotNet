namespace Applitools.Selenium.Tests
{
    class ConfigurationProviderForTesting : ISeleniumConfigurationProvider
    {
        private readonly Configuration configuration_ = new Configuration();

        Configuration ISeleniumConfigurationProvider.GetConfiguration() { return configuration_; }
    }
}
