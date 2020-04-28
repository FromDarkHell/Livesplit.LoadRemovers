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
using UpdateManager;

namespace Livesplit.Borderlands3
{
    class Borderlands3Component : LogicComponent
    {
        public override string ComponentName => "Borderlands 3";
        public Borderlands3Settings Settings { get; set; }
        private TimerModel timerModel;
        private MemoryReader memReader;

        private const string pointerFileName = "LiveSplit.Borderlands3.xml";
        private string pointerFilePath = "";

        public Borderlands3Component(LiveSplitState state)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(TimedTraceListener.Instance);

            pointerFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + pointerFileName;
            UpdatePointerFile();

            PointerInfoReader.Initialize();
            this.Settings = new Borderlands3Settings();
            timerModel = new TimerModel { CurrentState = state };
            timerModel.CurrentState.OnStart += gameTimer_OnStart;

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

            return true;
        }

        void UpdatePointerFile()
        {
            if (CheckForComponentUpdate())
            {

            }
        }

        /// <summary>
        /// Code courtesy of Dalet/Drtchops at https://github.com/drtchops/LiveSplit.Skyrim/blob/master/SkyrimSettings.cs
        /// </summary>
        /// <returns></returns>
        bool CheckForComponentUpdate()
        {
            bool updateAvailable = true;
            var bl3Factory = new Borderlands3Factory();
            updateAvailable = bl3Factory.CheckForUpdate();
            if (!updateAvailable)
            {
                try
                {
                    using (XmlReader reader = XmlReader.Create(bl3Factory.XMLURL))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(reader);
                        foreach (XmlNode updateNode in doc.DocumentElement.ChildNodes)
                        {
                            Update update = UpdateManager.Update.Parse(updateNode);
                            if (update.Version > bl3Factory.Version)
                            {
                                updateAvailable = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                }
            }

            return updateAvailable;
        }
        #endregion

        void gameTimer_OnStart(object sender, EventArgs e)
        {
            timerModel.InitializeGameTime();
        }



        #region Component Definitions
        public override Control GetSettingsControl(LayoutMode mode)
        {
            return this.Settings;
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return this.Settings.GetSettings(document);
        }

        public override void SetSettings(XmlNode settings)
        {
            this.Settings.SetSettings(settings);
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

        public override void Dispose()
        {
            timerModel.CurrentState.OnStart -= gameTimer_OnStart;
        }
        #endregion
    }


    // Code courtesy of Fatalis (https://github.com/fatalis)
    public class TimedTraceListener : DefaultTraceListener
    {
        private static TimedTraceListener _instance;
        public static TimedTraceListener Instance => _instance ?? (_instance = new TimedTraceListener());

        private TimedTraceListener() { }

        public int UpdateCount
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public override void WriteLine(string message)
        {
            base.WriteLine("[BL3 Plugin]: " + message);
        }
    }
}
