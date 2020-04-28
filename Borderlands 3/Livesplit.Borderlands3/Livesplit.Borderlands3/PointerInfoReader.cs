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

        public static Dictionary<string, DeepPointer> getPointersForVersion(string version, string storefront)
        {
            Debug.WriteLine($"Reading pointers for {storefront}/{version}");
            Dictionary<string, DeepPointer> addrs = new Dictionary<string, DeepPointer>();
            if (pointerDocument.XPathSelectElement($"/PointersRoot/{storefront}/{version}") == null)
            {
                Debug.WriteLine($"Unable to find pointers for: {storefront}/{version}");
                return addrs;
            }
            XElement versionNode = pointerDocument.XPathSelectElement($"/PointersRoot/{storefront}/{version}");
            if (!versionNode.Elements("loading").Elements("base").Any()) Debug.WriteLine($"Unable to find loading pointer for {version}");
            else  // This'll run whenever we actually have a proper loading pointer...
                addrs.Add("loading", ReadDeepPointer(versionNode.Element("loading")));

            if (!versionNode.Elements("mainMenu").Elements("base").Any()) Debug.WriteLine($"Unable to find main menu pointer for {version}");
            else addrs.Add("mainMenu", ReadDeepPointer(versionNode.Element("mainMenu")));

            return addrs;
        }
        private static DeepPointer ReadDeepPointer(XElement pointerNode)
        {
            string baseValue = pointerNode.Element("base").Value;
            string module = baseValue.Contains("+") ? baseValue.Split('+')[0] : "Borderlands3.exe";
            Debug.WriteLine(baseValue.Contains("+") ? baseValue.Split('+')[1] : baseValue);
            int baseAddress = Convert.ToInt32(baseValue.Contains("+") ? baseValue.Split('+')[1] : baseValue, 16);

            Debug.WriteLine($"Module + Address: {module},{baseAddress} | Offset Count: {pointerNode.Elements("offsets").Count()}");
            int[] offsets = new int[] { };
            if (pointerNode.Element("offsets").Nodes().Any())
            {
                Debug.WriteLine("Reading offsets...");
                offsets = pointerNode.Element("offsets").Elements().Select(x => Convert.ToInt32(x.Value, 16)).ToArray();
                Debug.WriteLine(string.Join(",", offsets));
            }
            else Debug.WriteLine($"No offsets found for: {baseValue}");

            return new DeepPointer(module, baseAddress, offsets);
        }

    }


}
