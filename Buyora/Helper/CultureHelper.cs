using System.Globalization;

namespace Buyora.Helper
{
    public static class CultureHelper
    {
        public static bool IsEnglish()
        {
            CultureInfo currendCulture = Thread.CurrentThread.CurrentCulture;
            return currendCulture.TwoLetterISOLanguageName.ToLower()=="en";
        }
        public static bool IsRightToLeft()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            return currentCulture.TextInfo.IsRightToLeft;
        }
    }
}
