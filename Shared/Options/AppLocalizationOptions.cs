namespace Shared.Options
{
    public class AppLocalizationOptions
    {
        public string DefaultCulture { get; set; } = "en";
        public string[] SupportedCultures { get; set; } = ["en", "tr"];
    }
}
