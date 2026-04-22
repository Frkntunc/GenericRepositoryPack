namespace Shared.Options
{
    public class FileLoggingOptions
    {
        public string InfoLogPath { get; set; } = "Logs/Info/log-.txt";
        public string WarningLogPath { get; set; } = "Logs/Warning/log-.txt";
        public string ErrorLogPath { get; set; } = "Logs/Error/log-.txt";
    }
}
