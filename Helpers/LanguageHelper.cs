namespace AvansApp.Helpers
{
    public enum Language
    {
        Dutch,
        English,
        Spanish,    // Todo
        French      // Todo
    }

    public class LanguageHelper
    {

        public static Language GetLanguage()
        {
            var topUserLanguage = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            //var language = new Windows.Globalization.Language(topUserLanguage);

            switch (topUserLanguage.ToLower())
            {
                case "nl-nl": // Dutch
                    return Language.Dutch;

                case "en-au":   // English - Australia
                case "en-bz":   // English - Belize
                case "en-ca":   // English - Canada
                case "en-029":  // English - Caribbean
                case "en-in":   // English - India
                case "en-ie":   // English - Ireland
                case "en-jm":   // English - Jamaica
                case "en-my":   // English - Malaysia
                case "en-nz":   // English - New Zeeland
                case "en-ph":   // English - Philippines
                case "en-sg":   // English - Singapore
                case "en-za":   // English - South Africa
                case "en-tt":   // English - Trinidad
                case "en-gb":   // English - Great Britain
                case "en-us":   // English - United States
                case "en-zw":   // English - Zimbabwe
                    return Language.English;

                default:
                    return Language.English;
            }
        }
    }
}
