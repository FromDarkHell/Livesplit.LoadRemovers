using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;

namespace Livesplit.Borderlands3
{
    class MemoryReader
    {
        private Process gameProcess;
        private string gameVersion;
        private MemoryDefinition versionDefinition;
        private List<int> pidsToIgnore = new List<int>();

        public MemoryReader()
        {

        }

        public void Update(LiveSplitState state)
        {
            if (gameProcess == null || gameProcess.HasExited)
            {
                if (!this.TryGetGameProcess(state)) return;
            }
            versionDefinition.UpdateAll(gameProcess);
            bool bPauseTimer = false;
            bool bChangedValue = false;

            if (versionDefinition.isMainMenuState != null && versionDefinition.isMainMenuState.Changed)
            {
                Debug.WriteLine("Main Menu State Changed...");
                bool mainMenuState = (bool)versionDefinition.isMainMenuState.Current;

                bPauseTimer |= mainMenuState;
                bChangedValue = true;
            }

            if (versionDefinition.isLoadingState != null && versionDefinition.isLoadingState.Changed)
            {
                Debug.WriteLine("Loading State Changed...");
                bool loadingState = (bool)versionDefinition.isLoadingState.Current;
                bPauseTimer |= loadingState;
                bChangedValue = true;
            }


            if (bChangedValue && bPauseTimer)
            {
                Debug.WriteLine($"Pausing timer: {bPauseTimer}");
                state.IsGameTimePaused = bPauseTimer;
            }
            else if (bChangedValue)
            {
                Debug.WriteLine($"Unpausing timer: {bPauseTimer}");
                state.IsGameTimePaused = bPauseTimer;
            }
        }

        private bool TryGetGameProcess(LiveSplitState state)
        {
            Process possibleProcess = Process.GetProcessesByName("Borderlands3").FirstOrDefault(p => p.MainModule.FileName.Contains("OakGame") && !pidsToIgnore.Contains(p.Id)); // Find a running version of BL3 (proper) without an improper version

            if (possibleProcess == null) return false; // If we were unable to find a version of BL3, might as well stop now.

            string possibleVersion = VersionHelper.GetProductVersion(possibleProcess.MainModule.FileName); // A string for our currently possible version
            Debug.WriteLine("Possible Version: " + possibleVersion);

            MemoryDefinition possibleDef = new MemoryDefinition(possibleVersion, possibleProcess.Modules[0].BaseAddress);

            if (!possibleDef.bKnownVersion || possibleDef == null)
            {
                pidsToIgnore.Add(possibleProcess.Id);
                MessageBox.Show("Unexpected game version: " + gameVersion, "LiveSplit.Borderlands3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Initialize all of our variables since we have a proper game version now.
            gameProcess = possibleProcess;
            gameVersion = possibleVersion;
            versionDefinition = possibleDef;


            Debug.WriteLine("Base Address: " + possibleProcess.Modules[0].BaseAddress.ToString("X"));

            state.IsGameTimeInitialized = true;
            return true;
        }
    }


    class MemoryDefinition : MemoryWatcherList
    {
        public MemoryWatcher isMainMenuState { get; } = null;
        public MemoryWatcher isLoadingState { get; } = null;

        public bool bKnownVersion;
        public Dictionary<string, DeepPointer> pointers = new Dictionary<string, DeepPointer>();
        public MemoryDefinition(string gameVersion, IntPtr baseAddr)
        {
            bKnownVersion = true;
            pointers = JSONReader.getPointersForVersion(gameVersion);
            Debug.WriteLine("Done reading JSON pointers...");
            if (pointers.Count == 0) // If we're using an unknown version, or atleast one in which we don't have the pointers.
            {
                bKnownVersion = false;
                return;
            }

            if (pointers.ContainsKey("loading")) isLoadingState = new MemoryWatcher<bool>(pointers["loading"]);
            if (pointers.ContainsKey("mainMenu")) isMainMenuState = new MemoryWatcher<bool>(pointers["mainMenu"]);


            this.AddRange(this.GetType().GetProperties().Where(p => !p.GetIndexParameters().Any()).Select(p => p.GetValue(this, null) as MemoryWatcher).Where(p => p != null));
        }
    }
}