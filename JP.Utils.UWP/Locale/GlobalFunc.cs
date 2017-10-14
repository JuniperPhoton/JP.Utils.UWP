using Windows.Globalization;
using Windows.System.UserProfile;

namespace JP.Utils.Global
{
    public static class LocaleUtils
    {
        public static string GetLanguage()
        {
            var lang = GlobalizationPreferences.Languages[0];
            return lang;
        }

        public static string GetLocale()
        {
            GeographicRegion userRegion = new GeographicRegion();
            string regionCode = userRegion.CodeTwoLetter;
            return regionCode;
        }
    }
}