using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Text;

namespace Applitools.VisualGrid
{
    internal class DashSeparatedNamingStrategy : NamingStrategy
    {
        internal enum SnakeCaseState
        {
            Start,
            Lower,
            Upper,
            NewWord
        }

        protected override string ResolvePropertyName(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            StringBuilder stringBuilder = new StringBuilder();
            SnakeCaseState snakeCaseState = SnakeCaseState.Start;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (snakeCaseState != 0)
                    {
                        snakeCaseState = SnakeCaseState.NewWord;
                    }
                }
                else if (char.IsUpper(s[i]))
                {
                    switch (snakeCaseState)
                    {
                        case SnakeCaseState.Upper:
                            {
                                bool flag = i + 1 < s.Length;
                                if ((i > 0) & flag)
                                {
                                    char c = s[i + 1];
                                    if (!char.IsUpper(c) && c != '-')
                                    {
                                        stringBuilder.Append('-');
                                    }
                                }
                                break;
                            }
                        case SnakeCaseState.Lower:
                        case SnakeCaseState.NewWord:
                            stringBuilder.Append('-');
                            break;
                    }
                    char value = char.ToLower(s[i], CultureInfo.InvariantCulture);
                    stringBuilder.Append(value);
                    snakeCaseState = SnakeCaseState.Upper;
                }
                else if (s[i] == '_')
                {
                    stringBuilder.Append('-');
                    snakeCaseState = SnakeCaseState.Start;
                }
                else
                {
                    if (snakeCaseState == SnakeCaseState.NewWord)
                    {
                        stringBuilder.Append('-');
                    }
                    stringBuilder.Append(s[i]);
                    snakeCaseState = SnakeCaseState.Lower;
                }
            }
            return stringBuilder.ToString();
        }
    }
}