using System.Linq;
using System.Xml.Linq;

namespace LightBlue.Infrastructure
{
    public static class ConfigurationManipulation
    {
        public static void RemoveAzureTraceListenerFromConfiguration(string configurationFile)
        {
            var xdocument = XDocument.Load(configurationFile);
            if (xdocument.Root == null)
            {
                return;
            }

            var diagnosticsElement = xdocument.Root.Element("system.diagnostics");
            if (diagnosticsElement == null)
            {
                return;
            }

            var listenersElement = diagnosticsElement.Descendants("listeners")
                .FirstOrDefault(l => l.Parent != null && l.Parent.Name == "trace");
            if (listenersElement == null)
            {
                return;
            }

            foreach (var addElement in listenersElement.Descendants("add")
                .Where(a => a.Attribute("type").Value.StartsWith("Microsoft.WindowsAzure"))
                .ToList())
            {
                addElement.Remove();
            }

            xdocument.Save(configurationFile);
        }
    }
}