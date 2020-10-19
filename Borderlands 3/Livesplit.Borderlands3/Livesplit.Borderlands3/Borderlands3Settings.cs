using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace Livesplit.Borderlands3
{
    public partial class Borderlands3Settings : UserControl
    {
        public Borderlands3Settings()
        {
            InitializeComponent();

            AllowLevelSplits = AllowLevelSplits_Default;
            AllowSQCounter = AllowSQCounter_Default;
            SQCounterText = SQCounterText_Default;
        }

        public const bool AllowLevelSplits_Default = false;
        public bool AllowLevelSplits 
        {
            get => levelSplitsCheckbox.Checked;
            set {
                levelSplitsCheckbox.Checked = value;
            }
        }

        public const bool AllowSQCounter_Default = false;
        public bool AllowSQCounter
        {
            get => sqCounterCheckbox.Checked;
            set
            {
                sqCounterCheckbox.Checked = value;
            }
        }

        public const string SQCounterText_Default = "SQs:";
        public string SQCounterText
        {
            get => sqCounterTextBox.Text;
            set
            {
                sqCounterTextBox.Text = value;
            }
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement xmlSettings = document.CreateElement("Settings");

            XmlElement levelSplits = document.CreateElement("LevelSplits");
            levelSplits.InnerText = AllowLevelSplits.ToString();
            xmlSettings.AppendChild(levelSplits);

            XmlElement sqCounter = document.CreateElement("SQCounter");
            XmlElement sqCounterEnabled = document.CreateElement("Enabled");
            XmlElement sqCounterText = document.CreateElement("CounterText");
            sqCounterEnabled.InnerText = AllowSQCounter.ToString();
            sqCounterText.InnerText = SQCounterText;
            sqCounter.AppendChild(sqCounterEnabled);
            sqCounter.AppendChild(sqCounterText);
            xmlSettings.AppendChild(sqCounter);

            return xmlSettings;

        }
        public void SetSettings(XmlNode settings)
        {
            XmlNode levelSplits = settings.SelectSingleNode(".//LevelSplits");
            AllowLevelSplits = !string.IsNullOrEmpty(levelSplits?.InnerText) ? bool.Parse(levelSplits.InnerText) : AllowLevelSplits_Default;

            XmlNode sqCounterEnabled = settings.SelectSingleNode(".//SQCounter/Enabled");
            XmlNode sqCounterText = settings.SelectSingleNode(".//SQCounter/CounterText");
            AllowSQCounter = !string.IsNullOrEmpty(sqCounterEnabled?.InnerText) ? bool.Parse(levelSplits.InnerText) : AllowSQCounter_Default;
            SQCounterText = !string.IsNullOrEmpty(sqCounterText?.InnerText) ? sqCounterText.InnerText : SQCounterText_Default;
        }

        public void SetGameVersion(string version) => versionLabel.Text = "Game Version: " + version;

        private void sqCounterCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            sqCounterTextBox.Enabled = sqCounterCheckbox.Checked;
        }
    }
}
