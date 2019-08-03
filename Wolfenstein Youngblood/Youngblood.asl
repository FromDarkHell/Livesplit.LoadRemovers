state("Youngblood_x64vk", "bnet") {
	// Pointers provided by FromDarkHell
	bool isLoading  : "Youngblood_x64vk.exe", 0x2FE5978;
	bool isMainMenu : "Youngblood_x64vk.exe", 0x3BF9094;
}

state("Youngblood_x64vk", "steam") {
	// Pointers courtesy of TheFuncannon
	bool isLoading : "Youngblood_x64vk.exe", 0x2FDD988;
	bool isMainMenu : "Youngblood_x64vk.exe", 0x3BF1994;
}

init {
	var fileInfo = new FileInfo(modules.First().FileName);
	// The file size is one of the easiest ways to detect the type of game
	switch(fileInfo.Length) {
		case 346594304:
			version = "bnet";
			break;
		case 368997888:
			version = "steam";
			break;
		default:
			print("[YOUNGBLOOD] NEW UPDATE BROKE: " + fileInfo.Length.ToString());
			break;
	}
}

isLoading {
	return (current.isLoading || current.isMainMenu);
}
