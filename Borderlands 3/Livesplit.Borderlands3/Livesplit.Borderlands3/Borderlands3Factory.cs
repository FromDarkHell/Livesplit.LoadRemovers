using LiveSplit.Model;
using LiveSplit.UI.Components;
using Livesplit.Borderlands3;
using System;

using System.Reflection;

[assembly: ComponentFactory(typeof(Borderlands3Factory))]

namespace Livesplit.Borderlands3
{
    class Borderlands3Factory : IComponentFactory
    {
        public string ComponentName => "Borderlands 3 Load Removal";
        public string Description => "Automatic load removal for Borderlands 3";
        public ComponentCategory Category => ComponentCategory.Control;
        public IComponent Create(LiveSplitState state)
        {
            return new Borderlands3Component(state);
        }

        public string UpdateName => ComponentName;
        public string UpdateURL => "https://raw.githubusercontent.com/FromDarkHell/Livesplit.LoadRemovers/master/Borderlands%203/Livesplit.Borderlands3/";

        public string XMLURL => UpdateURL + "Components/update.Livesplit.Borderlands3.xml";

        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    }
}
