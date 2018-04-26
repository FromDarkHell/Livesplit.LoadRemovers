state("Borderlands", "1.0")
{
	bool isLoading1 : "Borderlands.exe", 0x1B98A74;
	bool isLoading2 : "Borderlands.exe", 0x1B97E38;
}


state("Borderlands", "1.4.1")
{
	bool isLoading1 : "Borderlands.exe", 0x00480AF0, 0;
	bool isLoading2 : "Borderlands.exe", 0x001E1F1C, 0;
}

init
{
	vars.doStart = false;
	vars.isLoading = false;
	switch (modules.First().FileVersionInfo.FileVersion)	
	{
		// Detects if game running is 1.0 patch due to the running exe's file version.
		case "1.0.0.0":
			version = "1.0";
			break;
		// Detects if game running is 1.5 patch due to the running exe's file version.
		case "1.5.0.0":
			version = "1.4.1";
			break;
	}
}

update
{
	vars.isLoading = false;
	if(current.isLoading1 || current.isLoading2) {
    // If the loading memory is true, change loading to true.
		vars.isLoading = true;
	}
}

isLoading
{
	return vars.isLoading;
}

exit
{
	// If the game is exited, pause IGT
    timer.IsGameTimePaused = true;
}
