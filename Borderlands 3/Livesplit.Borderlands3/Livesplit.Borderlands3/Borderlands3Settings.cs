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

namespace Livesplit.Borderlands3
{
    public partial class Borderlands3Settings : UserControl
    {
        public Borderlands3Settings()
        {
            InitializeComponent();
        }

        public XmlNode GetSettings(XmlDocument doc)
        {
            XmlElement settingsNode = doc.CreateElement("Settings"); ;
            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            return;
        }
    }
}
