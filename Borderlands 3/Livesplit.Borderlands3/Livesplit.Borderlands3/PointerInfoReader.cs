using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LiveSplit.ComponentUtil;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Livesplit.Borderlands3
{
    /// <summary>
    /// This function reads our `Livesplit.Borderlands3.json` file, and it gathers the respective pointers for a specific version.
    /// </summary>
    
    public class PointerInfo
    {
        public DeepPointer ptr = null;
        public int value = 0;
        public bool activeWhenValue = false;

        public bool ShouldPauseOnValue(int actualValue) => (value == actualValue) == activeWhenValue;
    }
    
    public static class PointerInfoReader
    {
        private static XDocument pointerDocument = null;

        public static void Initialize()
        {
            try
            {
                pointerDocument = XDocument.Load("Components\\Livesplit.Borderlands3.xml");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public static Dictionary<string, PointerInfo> getPointersForVersion(string version, string storefront)
        {
            Debug.WriteLine($"Reading pointers for {storefront}/{version}");
            Dictionary<string, PointerInfo> addrs = new Dictionary<string, PointerInfo>();

            if (pointerDocument.XPathSelectElement($"/PointersRoot/{storefront}/{version}") == null)
            {
                Debug.WriteLine($"Unable to find pointers for: {storefront}/{version}");
                return addrs;
            }
            XElement versionNode = pointerDocument.XPathSelectElement($"/PointersRoot/{storefront}/{version}");

            if (versionNode.Elements("loading").Elements("base").Any())
                addrs.Add("loading", ReadPointerInfo(versionNode.Element("loading")));
            else
                Debug.WriteLine($"Unable to find loading pointer for {version}");

            if (versionNode.Elements("mainMenu").Elements("base").Any())
                addrs.Add("mainMenu", ReadPointerInfo(versionNode.Element("mainMenu")));
            else
                Debug.WriteLine($"Unable to find main menu pointer for {version}");

            return addrs;
        }

        private static PointerInfo ReadPointerInfo(XElement pointerNode)
        {
            PointerInfo info = new PointerInfo();

            string baseValue = pointerNode.Element("base").Value;
            string module = baseValue.Contains("+") ? baseValue.Split('+')[0] : "Borderlands3.exe";
            int baseAddress = Convert.ToInt32(baseValue.Contains("+") ? baseValue.Split('+')[1] : baseValue, 16);

            Debug.WriteLine($"Module + Address: {module},0x{baseAddress:X} | Offset Count: {pointerNode.Elements("offsets").Count()}");
            int[] offsets = new int[] { };
            XElement offsetsNode = pointerNode.Element("offsets");
            if (offsetsNode?.Nodes()?.Any() ?? false)
            {
                Debug.WriteLine("Reading offsets...");
                offsets = offsetsNode.Elements().Select(x => Convert.ToInt32(x.Value, 16)).ToArray();
                Debug.WriteLine(string.Join(",", offsets.Select(v => $"0x{v:X}")));
            } else Debug.WriteLine($"No offsets found for: {baseValue}");

            info.ptr = new DeepPointer(module, baseAddress, offsets);

            XElement valueNode = pointerNode.Element("value");
            if (valueNode != null)
            {
                info.value = Convert.ToInt32(valueNode.Value, 16);
                Debug.WriteLine($"Using custom value 0x{info.value:X}");

                if (valueNode.Attribute("activeWhen")?.Value?.ToLower() == "equal")
                {
                    Debug.WriteLine("Active when equal to value");
                    info.activeWhenValue = true;
                } else Debug.WriteLine("Active when not equal to value");
            }

            return info;
        }
    }
}
