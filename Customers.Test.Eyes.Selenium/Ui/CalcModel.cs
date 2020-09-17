namespace Applitools.Ui
{
    using System;
    using System.Collections.Generic;
    using Applitools.Ui.Pages;
    using OpenQA.Selenium;

    /// <summary>
    /// Calculator model info.
    /// </summary>
    public class CalcModel
    {
        private static readonly string Name_ = "Test.Applitools.Calc";

        public static ModelWindowPage Open(IWebDriver driver, Uri appBaseUrl)
        {
            return Open(driver, appBaseUrl, "1");
        }

        public static ModelWindowPage Open(IWebDriver driver, Uri appBaseUrl, string testKey)
        {
            return ModelWindowPage.Open(driver, appBaseUrl, Name_, testKey);
        }

        public static StandardWindow Standard()
        {
            return StandardWindow.Instance;
        }

        public static StandardViewWindow StandardView()
        {
            return StandardViewWindow.Instance;
        }

        public static StandardHelpWindow StandardHelp()
        {
            return StandardHelpWindow.Instance;
        }

        public static StandardEditWindow StandardEdit()
        {
            return StandardEditWindow.Instance;
        }

        public static ScientificWindow Scientific()
        {
            return ScientificWindow.Instance;
        }

        public static ScientificViewWindow ScientificView()
        {
            return ScientificViewWindow.Instance;
        }

        public abstract class Singletone<T> where T : class, new()
        {
            private static Dictionary<string, object> instances_ = 
                new Dictionary<string, object>();

            public static T Instance
            {
                get
                {
                    string name = typeof(T).Name;
                    if (!instances_.ContainsKey(name))
                    {
                        instances_.Add(name, new T());
                    }

                    return instances_[name] as T;
                }
            }
        }

        public class StandardWindow : Singletone<StandardWindow>
        {
            public readonly string ID = "6440df4e-e5ca-4657-a47c-65f5b2a1a267";

            public ModelControl View()
            {
                return new ModelControl(20, 10);
            }

            public ModelControl Edit()
            {
                return new ModelControl(65, 10);
            }
        }

        public class StandardViewWindow : Singletone<StandardViewWindow>
        {
            public readonly string ID = "556341aa-cd5c-468c-9b83-be65f684ca7d";

            public ModelControl View()
            {
                return CalcModel.Standard().View();
            }

            public ModelControl ViewScientific()
            {
                return new ModelControl(65, 60);
            }
        }

        public class StandardHelpWindow : Singletone<StandardHelpWindow>
        {
            public readonly string ID = "a7f734c4-7d09-4eb1-92e6-bcb96a15ac55";
        }

        public class StandardEditWindow : Singletone<StandardEditWindow>
        {
            public readonly string ID = "bfabcd6a-2579-4a3a-89c6-7543df173bd6";

            public ModelControl Edit()
            {
                return CalcModel.Standard().Edit();
            }
        }

        public class ScientificWindow : Singletone<ScientificWindow>
        {
            public readonly string ID = "f7202da5-d6e3-45fd-854e-d65027376dc5";

            public ModelControl View()
            {
                return CalcModel.Standard().View();
            }
        }

        public class ScientificViewWindow : Singletone<ScientificViewWindow>
        {
            public readonly string ID = "6eefd429-4724-41fd-9d0d-4e6a57e13c65";

            public ModelControl Standard()
            {
                return new ModelControl(65, 35);
            }
        }
    }
}
