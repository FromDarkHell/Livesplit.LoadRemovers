using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Windows.Forms;

namespace Livesplit.Borderlands3
{
    class Borderlands3Component : LogicComponent
    {
        public override string ComponentName => "Borderlands 3";
        public Borderlands3Settings Settings { get; set; }
        private TimerModel timerModel;
        private MemoryReader memReader;
        public Borderlands3Component(LiveSplitState state)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(TimedTraceListener.Instance);


            JSONReader.Initialize();
            this.Settings = new Borderlands3Settings();
            timerModel = new TimerModel { CurrentState = state };
            timerModel.CurrentState.OnStart += gameTimer_OnStart;

            memReader = new MemoryReader();
        }

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
