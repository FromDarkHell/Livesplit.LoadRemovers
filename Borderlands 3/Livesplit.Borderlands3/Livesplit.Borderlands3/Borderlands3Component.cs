using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.UI;
using System;
using System.Diagnostics;
using UpdateManager;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Net;

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

            PointerInfoReader.Initialize();

            memReader = new MemoryReader();
        }


        #region Updating
        private bool DownloadPointerFile()
        {
            var client = new WebClient();
            var url = "https://raw.githubusercontent.com/FromDarkHell/Livesplit.LoadRemovers/master/Borderlands%203/Livesplit.Borderlands3/presets/Livesplit.Borderlands3.xml";
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
