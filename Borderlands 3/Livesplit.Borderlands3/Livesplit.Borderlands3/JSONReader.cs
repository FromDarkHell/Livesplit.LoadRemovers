using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LiveSplit.ComponentUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Livesplit.Borderlands3
{
    /// <summary>
    /// This function reads our `Livesplit.Borderlands3.json` file, and it gathers the respective pointers for a specific version.
    /// </summary>
    public static class JSONReader
    {
        private static JObject pointerDictionary = null;

        public static void Initialize()
        {
            try
            {
                pointerDictionary = (JObject)JsonConvert.DeserializeObject(File.ReadAllText("Components\\Livesplit.Borderlands3.json"));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public static Dictionary<string, DeepPointer> getPointersForVersion(string version)
        {
            Dictionary<string, DeepPointer> addrs = new Dictionary<string, DeepPointer>();
            if (!pointerDictionary.ContainsKey(version))
            {
                Debug.WriteLine($"Unable to find pointers for: {version}");
                return addrs;
            }
            JObject versionDictionary = (JObject)pointerDictionary[version];

            if (!versionDictionary.ContainsKey("loading")) Debug.WriteLine($"Unable to find loading pointer for {version}");
            else // This'll run whenever we actually have a proper loading pointer...
                addrs.Add("loading", ReadDeepPointer(versionDictionary["loading"]));

            if (!versionDictionary.ContainsKey("mainMenu")) Debug.WriteLine($"Unable to find main menu pointer for {version}");
            else if (versionDictionary["mainMenu"].Contains("base")) addrs.Add("mainMenu", ReadDeepPointer(versionDictionary["mainMenu"]));

            return addrs;
        }

        private static DeepPointer ReadDeepPointer(JToken pointerDef)
        {
            string baseValue = pointerDef["base"].ToString();
            string module = baseValue.Contains("+") ? baseValue.Split('+')[0] : "Borderlands3.exe";
            int baseAddress = Convert.ToInt32(baseValue.Contains("+") ? baseValue.Split('+')[1] : baseValue, 16);

            Debug.WriteLine($"Module + Address: {module},{baseAddress}");

            JArray jsonOffset = (JArray)pointerDef["offsets"];
            List<int> offsetsList = new List<int>();
            foreach (JToken jToken in jsonOffset.Children())
            {
                string offset = jToken.ToString();
                Debug.WriteLine($"Reading offset: {offset}");
                offsetsList.Add(Convert.ToInt32(jToken.ToString(), 16));
            }
            int[] offsets = offsetsList.ToArray();

            return new DeepPointer(module, baseAddress, offsets);
        }
    }
}
