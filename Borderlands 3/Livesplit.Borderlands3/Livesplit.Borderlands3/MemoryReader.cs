using System;
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
        private string gameStorefront;
        private MemoryDefinition versionDefinition;
        private readonly List<int> pidsToIgnore = new List<int>();
        private bool initalUpdate = false;

        public void Update(LiveSplitState state)
        {
            // Hopefully get our process from the memory, as well as initialize our MemoryDefinition for reading
            if (gameProcess == null || gameProcess.HasExited || versionDefinition == null)
                if (!this.TryGetGameProcess(state)) return;

            if (state.CurrentPhase == TimerPhase.NotRunning) initalUpdate = true;
            // No need to evaluate anything if not running, paused, or ended
            if (state.CurrentPhase != TimerPhase.Running) return;

            versionDefinition.UpdateAll(gameProcess);

            if ((versionDefinition.isLoadingState?.Changed ?? false)
                || (versionDefinition.isMainMenuState?.Changed ?? false)
                || initalUpdate)
            {
                bool bPauseTimer = false;
                Debug.WriteLine("State changed...");

                if (versionDefinition.isLoadingState != null)
                {
                    bool pauses = versionDefinition.isLoadingInfo.ShouldPauseOnValue(versionDefinition.isLoadingState.Current);
                    bPauseTimer |= pauses;

                    int currentValue = versionDefinition.isLoadingState.Current;
                    int wantedValue = versionDefinition.isLoadingInfo.value;
                    Debug.WriteLine(string.Format(
                        "Loading: 0x{0:X} {1} 0x{2:X}, should{3} pause",
                        currentValue,
                        currentValue == wantedValue ? "==" : "!=",
                        wantedValue,
                        pauses ? "" : "n't"
                    ));
                }

                if (versionDefinition.isMainMenuState != null)
                {
                    bool pauses = versionDefinition.isMainMenuInfo.ShouldPauseOnValue(versionDefinition.isMainMenuState.Current);
                    bPauseTimer |= pauses;

                    int currentValue = versionDefinition.isMainMenuState.Current;
                    int wantedValue = versionDefinition.isMainMenuInfo.value;
                    Debug.WriteLine(string.Format(
                        "Main Menu: 0x{0:X} {1} 0x{2:X}, should{3} pause",
                        currentValue,
                        currentValue == wantedValue ? "==" : "!=",
                        wantedValue,
                        pauses ? "" : "n't"
                    ));
                }

                state.IsGameTimePaused = bPauseTimer;

                if (initalUpdate && bPauseTimer)
                    state.SetGameTime(TimeSpan.Zero);
                initalUpdate = false;
            }
        }

        private bool TryGetGameProcess(LiveSplitState state)
        {
            Process possibleProcess = Process.GetProcessesByName("Borderlands3").FirstOrDefault(p => p.MainModule.FileName.Contains("OakGame") && !pidsToIgnore.Contains(p.Id)); // Find a running version of BL3 (proper) without an improper version

            if (possibleProcess == null) return false; // If we were unable to find a version of BL3, might as well stop now.

            string possibleVersion = VersionHelper.GetProductVersion(possibleProcess.MainModule.FileName); // A string for our currently possible version
            string possibleStorefront = VersionHelper.GetStorefront(possibleProcess);

            Debug.WriteLine("Possible Version: " + possibleVersion);

            MemoryDefinition possibleDef = new MemoryDefinition(possibleVersion, possibleStorefront);

            if (!possibleDef.bKnownVersion || possibleDef == null)
            {
                pidsToIgnore.Add(possibleProcess.Id);
                MessageBox.Show($"Unexpected game version: {possibleVersion} on {possibleStorefront}", "LiveSplit.Borderlands3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Initialize all of our variables since we have a proper game version now.
            gameProcess = possibleProcess;
            gameStorefront = possibleStorefront;
            gameVersion = possibleVersion;
            versionDefinition = possibleDef;

            Debug.WriteLine("Base Address: " + possibleProcess.Modules[0].BaseAddress.ToString("X"));

            state.IsGameTimeInitialized = true;
            return true;
        }
    }


    class MemoryDefinition : MemoryWatcherList
    {
        public MemoryWatcher<int> isMainMenuState { get; } = null;
        public PointerInfo isMainMenuInfo { get; } = null;

        public MemoryWatcher<int> isLoadingState { get; } = null;
        public PointerInfo isLoadingInfo { get; } = null;

        public bool bKnownVersion;
        public Dictionary<string, PointerInfo> pointers = new Dictionary<string, PointerInfo>();
        public MemoryDefinition(string gameVersion, string gameStorefront)
        {
            bKnownVersion = true;
            pointers = PointerInfoReader.getPointersForVersion(gameVersion, gameStorefront);
            Debug.WriteLine("Done reading XML pointers...");


            if (pointers.Count == 0) // If we're using an unknown version, or atleast one in which we don't have the pointers.
            {
                Debug.WriteLine($"Unknown pointers for: {gameVersion}");
                bKnownVersion = false;
                return;
            }

            if (pointers.ContainsKey("loading"))
            {
                isLoadingInfo = pointers["loading"];
                isLoadingState = new MemoryWatcher<int>(isLoadingInfo.ptr);
            }

            if (pointers.ContainsKey("mainMenu"))
            {
                isMainMenuInfo = pointers["mainMenu"];
                isMainMenuState = new MemoryWatcher<int>(isMainMenuInfo.ptr);
            }


            this.AddRange(this.GetType().GetProperties().Where(p => !p.GetIndexParameters().Any()).Select(p => p.GetValue(this, null) as MemoryWatcher).Where(p => p != null));
        }
    }
}