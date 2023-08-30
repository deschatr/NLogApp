using System.Text;
using NLog;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLogApp
{
    [LayoutRenderer("myLayout")]
    public class CustomLayoutRenderer : LayoutRenderer
    {
        private string SplunkString(string input)
        {
            if (input is null) return "NULL";
            return "\"" + input.Replace("\\", "%5C").Replace("\"", "%22") + "\"";
            // return input is null ? "NULL" : input.Replace("\\","%5C").Replace("\"","%22");
            // return input?.Replace("\\","%5C").Replace("\"","%22");
        }
        private string QuotedSplunkString(object input)
        {
            if (input is null) return "NULL";

            // adds quotes if the object is a string
            if (input is string s) return $"{SplunkString(s)}";

            return input.ToString();
        }
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append("Hello. ");
            builder.Append($"Message={SplunkString(logEvent.Message)}");
            foreach (var item in logEvent.Properties)
            {
                builder.Append($" {item.Key}={QuotedSplunkString(item.Value)}");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Console.WriteLine("Hello, World!");

            ConfigurationItemFactory.Default.LayoutRenderers
                .RegisterDefinition("mylayout", typeof(NLogApp.CustomLayoutRenderer));

            var logger = LogManager.GetCurrentClassLogger();

            var logEvent = new LogEventInfo(LogLevel.Error, "JB", "This is an error")
            {
                Properties =
                {
                    ["code"] = "007",
                    ["name"] = "Bond, \"James\" Bond"
                }
            };

            logger.Error("Error please");

            logger.Log(logEvent);

        }
    }
}