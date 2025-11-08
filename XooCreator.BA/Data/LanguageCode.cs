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

        /// <summary>
        /// Converts a language tag string (e.g., "en-us", "ro-ro", "hu-hu") to a LanguageCode enum.
        /// Returns LanguageCode.RoRo as default if the tag is null, empty, or unrecognized.
        /// </summary>
        public static LanguageCode FromTag(string? tag)
        {
            var t = (tag ?? "ro-ro").ToLowerInvariant();
            return t switch
            {
                "en-us" => LanguageCode.EnUs,
                "hu-hu" => LanguageCode.HuHu,
                _ => LanguageCode.RoRo
            };
        }
    }
}
