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
        private bool hasStateChanged = false;

        public MemoryReader()
        {

        }

        public void Update(LiveSplitState state)
        {
            // Hopefully get our process from the memory, as well as initialize our MemoryDefinition for reading
            if (gameProcess == null || gameProcess.HasExited)
                if (!this.TryGetGameProcess(state)) return;

            versionDefinition.UpdateAll(gameProcess);

            if (!hasStateChanged)
            {
                bool bPauseTimer = false;
                if (versionDefinition.isLoadingState != null) bPauseTimer = (bool)versionDefinition.isLoadingState.Current;
                if (versionDefinition.isMainMenuState != null) bPauseTimer |= (bool)versionDefinition.isMainMenuState.Current;
                state.IsGameTimePaused = bPauseTimer;
            }

            if ((versionDefinition.isLoadingState != null && versionDefinition.isLoadingState.Changed) || (versionDefinition.isMainMenuState != null && versionDefinition.isMainMenuState.Changed))
            {
                bool bPauseTimer = false;
                Debug.WriteLine("State changed...");

                if (versionDefinition.isLoadingState != null)
                {
                    Debug.WriteLine(string.Format("{0}ausing timer for loading: {1}", (bool)versionDefinition.isLoadingState.Current ? "P" : "Unp", (bool)versionDefinition.isLoadingState.Current));
                    bPauseTimer |= (bool)versionDefinition.isLoadingState.Current;

                }

                if (versionDefinition.isMainMenuState != null)
                {
                    Debug.WriteLine(string.Format("{0}ausing timer for main menu: {1}", (bool)versionDefinition.isMainMenuState.Current ? "P" : "Unp", (bool)versionDefinition.isMainMenuState.Current));
                    bPauseTimer |= (bool)versionDefinition.isMainMenuState.Current;
                }
                state.IsGameTimePaused = bPauseTimer;
                hasStateChanged = true;
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
        public MemoryWatcher isMainMenuState { get; } = null;
        public MemoryWatcher isLoadingState { get; } = null;

        public bool bKnownVersion;
        public Dictionary<string, DeepPointer> pointers = new Dictionary<string, DeepPointer>();
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
                isLoadingState = new MemoryWatcher<bool>(pointers["loading"]);
            if (pointers.ContainsKey("mainMenu"))
                isMainMenuState = new MemoryWatcher<bool>(pointers["mainMenu"]);


            this.AddRange(this.GetType().GetProperties().Where(p => !p.GetIndexParameters().Any()).Select(p => p.GetValue(this, null) as MemoryWatcher).Where(p => p != null));
        }
    }
}