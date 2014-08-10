using System;
using System.Globalization;
using System.Text;

namespace LightBlue.Infrastructure
{
    public static class ExceptionExtensions
    {
        public static string ToTraceMessage(this Exception ex)
        {
            var messageBuilder = new StringBuilder();

            AddExceptionDetails(messageBuilder, ex);

            var exception = ex.InnerException;
            while (exception != null)
            {
                messageBuilder.AppendLine("----------------------------------------------------------------");

                AddExceptionDetails(messageBuilder, exception);

                exception = exception.InnerException;
            }

            return messageBuilder.ToString();
        }

        private static void AddExceptionDetails(StringBuilder messsageBuilder, Exception exception)
        {
            messsageBuilder.AppendLine(exception.Message);
            if (exception.TargetSite != null)
            {
                messsageBuilder.AppendLine(exception.TargetSite.ToString());
            }
            messsageBuilder.AppendLine(exception.StackTrace);
            if (!string.IsNullOrWhiteSpace(exception.Source))
            {
                messsageBuilder.Append("Source: ");
                messsageBuilder.AppendLine(exception.Source);
            }
            if (!string.IsNullOrWhiteSpace(exception.HelpLink))
            {
                messsageBuilder.Append("Help Link: ");
                messsageBuilder.AppendLine(exception.HelpLink);
            }
            messsageBuilder.Append("HResult: ");
            messsageBuilder.AppendLine(exception.HResult.ToString(CultureInfo.InvariantCulture));
        }
    }
}