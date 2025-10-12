namespace XooCreator.BA.Data
{
    public enum LanguageCode
    {
        RoRo, // ro-ro
        EnUs, // en-us
        HuHu, // hu-hu
    }

    public static class LanguageCodeExtensions
    {
        public static string ToFolder(this LanguageCode code)
        {
            return code switch
            {
                LanguageCode.RoRo => "ro-ro",
                LanguageCode.EnUs => "en-us",
                LanguageCode.HuHu => "hu-hu",
                _ => "ro-ro"
            };
        }

        public static string ToTag(this LanguageCode code)
        {
            return code switch
            {
                LanguageCode.RoRo => "ro-ro",
                LanguageCode.EnUs => "en-us",
                LanguageCode.HuHu => "hu-hu",
                _ => "ro-ro"
            };
        }

        public static IEnumerable<LanguageCode> All()
        {
            yield return LanguageCode.RoRo;
            yield return LanguageCode.EnUs;
            yield return LanguageCode.HuHu;
        }
    }
}
