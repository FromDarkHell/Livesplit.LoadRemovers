state("Youngblood_x64vk", "bnet") {
	// Pointers provided by FromDarkHell
	bool isLoading  : "Youngblood_x64vk.exe", 0x3FBF180;
	int isMainMenu : "Youngblood_x64vk.exe", 0x3EA1970;
}

state("Youngblood_x64vk", "steam") {
	// Pointers courtesy of TeaFC
	bool isLoading  : "Youngblood_x64vk.exe", 0x3FB7A80;
	bool isMainMenu : "Youngblood_x64vk.exe", 0x3BF3A14;
}

init {
	var directoryInfo = new DirectoryInfo(modules.First().FileName.Replace("Youngblood_x64vk.exe","") + "\\base");
	// The standalonefiles.json is one of the easiest ways to detect versions...
	foreach (FileInfo file in directoryInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly))
    {
       if (file.FullName.Contains("standalonefiles.json")) { version = "bnet"; break; }
       else version = "steam";
    }

}

isLoading {
	return (current.isLoading || ((version == "steam" && current.isMainMenu) || (version == "bnet" && current.isMainMenu == 1) ));
}

// Pause the timer whenever the game closes cause video games.
exit
{
	timer.IsGameTimePaused = true;
}
