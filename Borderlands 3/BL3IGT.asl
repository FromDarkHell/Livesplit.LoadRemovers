state("Borderlands3") {}

init {
	timer.IsGameTimePaused = false;
	print("[BL3 ASL] Initialized");
	vars.game = null;
	vars.ptr = null;
	foreach(Process p in Process.GetProcessesByName("Borderlands3")) {
		if(p.MainModule.FileName.Contains("OakGame")) {
			vars.game = p;
			vars.ptr = p.Modules[0].BaseAddress;
			print(String.Format("[BL3 ASL] Found correct game! Located at: {0}", p.MainModule.FileName).ToString() );
		}
	}

	if(vars.ptr == IntPtr.Zero) {
        Thread.Sleep(1000);
        throw new Exception();
	}

	vars.isMainMenuState = new MemoryWatcher<int>(vars.ptr + 0x603CDA8);
	vars.isLoadingState = new MemoryWatcher<bool>(vars.ptr + 0x5D45740);
	vars.isFirstGameLaunchState = new MemoryWatcher<bool>(vars.ptr + 0x603AD78);

	vars.watchers = new MemoryWatcherList() {
		vars.isMainMenuState,
		vars.isLoadingState,
		vars.isFirstGameLaunchState
	};
}

update {
	vars.watchers.UpdateAll(vars.game);
}

exit
{
    timer.IsGameTimePaused = true;
}

isLoading
{
    return (vars.isMainMenuState.Current == 0) || (!vars.isLoadingState.Current) || (vars.isFirstGameLaunchState.Current);
}
