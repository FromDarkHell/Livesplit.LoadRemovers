state("BorderlandsGOTY")
{
	// General data pointers that we use for if we're on the main menu
	bool isMainMenu : "BorderlandsGOTY.exe", 0x02544B18, 0xe8, 0x108, 0xd4,  0xfc;
	// If mainMenu2 is == 153 OR 139, we're [probably] on the main menu
	int  mainMenu2 : "BorderlandsGOTY.exe", 0x242478C;

	// A proper boolean that returns true when we're loading
	bool isLoading  : "BorderlandsGOTY.exe", 0x25C2A1C;
	bool isLoading2 : "BorderlandsGOTY.exe", 0x25C20FC;
	// An integer value that is == 46179 when we're saving our game
	int  isSavingGame : "BorderlandsGOTY.exe", 0x025489E0, 0x4c0, 0x760, 0x78;
}

// We wanna pause the IGT timer if we're loading, in the main menu, or doing a save & quit.
isLoading
{
	return (current.isLoading || current.isLoading2) || (current.isMainMenu || current.mainMenu2 == 153 || current.mainMenu2 == 139) || current.isSavingGame == 46179;
}

init
{
    timer.IsGameTimePaused = false;
    game.Exited += (s, e) => timer.IsGameTimePaused = true;
}

// Pause the timer whenever the game closes cause this game has memory leaks.
exit
{
	timer.IsGameTimePaused = true;
}
