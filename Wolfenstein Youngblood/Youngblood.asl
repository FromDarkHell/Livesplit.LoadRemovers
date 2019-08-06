state("Youngblood_x64vk", "bnet") {
	// Pointers provided by FromDarkHell
	bool isLoading  : "Youngblood_x64vk.exe", 0x2FE5978;
	bool isMainMenu : "Youngblood_x64vk.exe", 0x3BF9094;
}

state("Youngblood_x64vk", "steam") {
	// Pointers courtesy of TheFuncannon
	bool isLoading : "Youngblood_x64vk.exe", 0x2FDF98C;
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
	return (current.isLoading || current.isMainMenu);
}
