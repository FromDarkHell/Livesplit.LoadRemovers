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
        }

        public const bool AllowLevelSplits_Default = false;
        public bool AllowLevelSplits 
        {
            get => levelSplitsCheckbox.Checked;
            set {
                levelSplitsCheckbox.Checked = value;
            }
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement xmlSettings = document.CreateElement("Settings");

            XmlElement levelSplits = document.CreateElement("LevelSplits");
            levelSplits.InnerText = AllowLevelSplits.ToString();
            xmlSettings.AppendChild(levelSplits);

            return xmlSettings;

        }
        public void SetSettings(XmlNode settings)
        {
            XmlNode levelSplits = settings.SelectSingleNode(".//LevelSplits");
            AllowLevelSplits = !string.IsNullOrEmpty(levelSplits?.InnerText) ? bool.Parse(levelSplits.InnerText) : AllowLevelSplits_Default;
        }

        public void SetGameVersion(string version) => versionLabel.Text = "Game Version: " + version;
    }
}
