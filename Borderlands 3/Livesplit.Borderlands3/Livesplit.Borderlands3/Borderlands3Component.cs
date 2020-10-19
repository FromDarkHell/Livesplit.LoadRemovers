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
using System.Xml.XPath;
using System.Linq;

namespace Livesplit.Borderlands3
{
    class Borderlands3Component : LogicComponent
    {
        public override string ComponentName => "Borderlands 3 Load Removal";
        private readonly TimerModel timerModel;
        private readonly Borderlands3Settings settings;
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

            timerModel = new TimerModel() { CurrentState = state };
            settings = new Borderlands3Settings();
            memReader = new MemoryReader(timerModel, settings);

            state.OnStart += memReader.OnTimerStart;
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
                MessageBox.Show("Pointer Download failed...\n Error message: " + ex.Message, "Presets update failed",
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
                Version serverVersion = Version.Parse(doc.XPathSelectElement("/PointersRoot").Attributes().First().Value);

                XDocument localDoc = XDocument.Load("Components\\Livesplit.Borderlands3.xml");
                Version localVersion;
                if (localDoc.XPathSelectElement("/PointersRoot").HasAttributes) localVersion = Version.Parse(localDoc.XPathSelectElement("/PointersRoot").Attributes().First().Value);
                else { localVersion = new Version("0.0.0"); }

                if(serverVersion > localVersion)
                {
                    if(MessageBox.Show("There is an update available for Borderlands 3 Pointers...\nDo you want to update?", "Update available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        File.WriteAllText(pointerFilePath, response);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Pointer Update failed...\n Error message: " + ex.Message, "Presets update failed",
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
            return settings;
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return settings.GetSettings(document);
        }

        public override void SetSettings(XmlNode settings)
        {
            this.settings.SetSettings(settings);
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
