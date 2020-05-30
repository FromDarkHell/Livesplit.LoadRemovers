using System;
using System.IO;
using System.Net;
using System.Xml;
using LiveSplit.UI;
using LiveSplit.Model;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using LiveSplit.UI.Components;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Livesplit.Borderlands3
{
    class Borderlands3Component : LogicComponent
    {
        public override string ComponentName => "Borderlands 3 Load Removal";
        private readonly MemoryReader memReader;

        private const string pointerFileName = "LiveSplit.Borderlands3.xml";
        private readonly string pointerFilePath = "";

        public Borderlands3Component(LiveSplitState state)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(BL3TraceListener.Instance);

            pointerFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + pointerFileName;
            if (!File.Exists(pointerFilePath)) DownloadPointerFile();
            else CheckPointerFileForUpdate();

            PointerInfoReader.Initialize();

            memReader = new MemoryReader();
        }


        #region Updating
        private bool DownloadPointerFile()
        {
            var client = new WebClient();
            var url = "https://raw.githubusercontent.com/FromDarkHell/Livesplit.LoadRemovers/master/Borderlands%203/Livesplit.Borderlands3/Components/Livesplit.Borderlands3.xml";
            Debug.WriteLine($"Downloading at: {url}");
            try { client.DownloadFile(url, pointerFilePath); }
            catch (Exception ex)
            {
                MessageBox.Show("Pointer downloaded failed...\n Error message: " + ex.Message, "Presets update failed",
    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine(ex.StackTrace);
                return false;
            }
            finally { client.Dispose(); }

            MessageBox.Show("Pointers successfully updated.", "Pointers updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private bool CheckPointerFileForUpdate()
        {
            var client = new WebClient();
            var url = "https://raw.githubusercontent.com/FromDarkHell/Livesplit.LoadRemovers/master/Borderlands%203/Livesplit.Borderlands3/Components/Livesplit.Borderlands3.xml";
            Debug.WriteLine($"Updating : {url}");
            try { 
                string response = client.DownloadString(url);
                Debug.WriteLine(response);
                XDocument doc = XDocument.Parse(response);
                MessageBox.Show(doc.Root.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Pointer downloaded failed...\n Error message: " + ex.Message, "Presets update failed",
    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine(ex.StackTrace);
                return false;
            }

            return true;
        }
        #endregion

        #region Component Definitions
        public override Control GetSettingsControl(LayoutMode mode)
        {
            return null;
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return document.CreateElement("Settings");
        }

        public override void SetSettings(XmlNode settings)
        {
            return;
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            try
            {
                memReader.Update(state);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public override void Dispose() { }
        #endregion
    }


    public class BL3TraceListener : DefaultTraceListener
    {
        private static BL3TraceListener _instance;
        public static BL3TraceListener Instance => _instance ?? (_instance = new BL3TraceListener());

        private BL3TraceListener() { }

        public override void WriteLine(string message)
        {
            base.WriteLine("[BL3 Plugin]: " + message);
        }
    }
}
