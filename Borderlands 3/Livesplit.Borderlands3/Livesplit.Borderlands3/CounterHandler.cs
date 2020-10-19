using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LiveSplit.Model;

namespace Livesplit.Borderlands3
{
    // The counter is an optional component, not shipped in the main LiveSplit zip, so we don't
    //  want to rely on an assembly reference, instead we'll use reflection
    static class CounterHandler
    {
        private static bool initalized = false;

        private static Type componentType;

        private static PropertyInfo counterProperty;
        private static PropertyInfo settingsProperty;
        private static PropertyInfo textProperty;

        private static MethodInfo incrementMethod;
        private static MethodInfo resetMethod;

        private static Borderlands3Settings settings;

        public static void Init(Borderlands3Settings settings)
        {
            CounterHandler.settings = settings;

            Assembly counterAsm;
            try
            {
                counterAsm = Assembly.Load("LiveSplit.Counter");
                if (counterAsm is null) throw new FileNotFoundException();
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("Unable to load 'LiveSplit.Counter', counter features will be disabled.");
                settings.SetSupportsCounter(false);
                return;
            }

            componentType = counterAsm.GetType("LiveSplit.UI.Components.CounterComponent");
            Type counterType = counterAsm.GetType("LiveSplit.UI.Components.Counter");
            Type settingsType = counterAsm.GetType("LiveSplit.UI.Components.CounterComponentSettings");

            counterProperty = componentType.GetProperty("Counter");
            settingsProperty = componentType.GetProperty("Settings");
            textProperty = settingsType.GetProperty("CounterText");

            incrementMethod = counterType.GetMethod("Increment");
            resetMethod = counterType.GetMethod("Reset");

            initalized = true;
            settings.SetSupportsCounter(true);
        }

        public static void Increment(LiveSplitState state)
        {
            if (initalized) {
                incrementMethod.Invoke(SelectCounter(state), new object[0]);
            }            
        }

        public static void Reset(LiveSplitState state)
        {
            if (initalized)
            {
                resetMethod.Invoke(SelectCounter(state), new object[0]);
            }
        }

        private static object SelectCounter(LiveSplitState state)
        {
            object component = state.Layout.Components.Where(
                cpnt => {
                    if (cpnt.GetType() != componentType) return false;

                    object counterSettings = settingsProperty.GetValue(cpnt);
                    string counterText = (string) textProperty.GetValue(counterSettings);

                    return counterText == settings.SQCounterText;
                }
            ).FirstOrDefault();

            return counterProperty.GetValue(component);
        }
    }
}
