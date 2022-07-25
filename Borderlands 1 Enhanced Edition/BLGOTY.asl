state("BorderlandsGOTY")
{
	bool isMainMenu   : "BorderlandsGOTY.exe", 0x02544B18, 0xe8, 0x108, 0xd4,  0xfc;
	int  isLoading    : "BorderlandsGOTY.exe", 0x242478C;
	int  isLoading2  :  "BorderlandsGOTY.exe", 0x025C20C8, 0x04, 0x3d8, 0x278, 0x4c0, 0x3c0;
	int  isSavingGame : "BorderlandsGOTY.exe", 0x025489E0, 0x4c0, 0x760, 0x78;
}

// We wanna pause the IGT timer if we're loading, in the main menu, or doing a save & quit.
isLoading
{
	bool load = (current.isLoading == 2 || current.isLoading == 1 || current.isLoading == 0 || current.isLoading == 14);
	bool mapMatch = (current.isLoading != 5336 && current.isLoading != 3849 && current.isLoading != 2009);
	return (current.isLoading2 == 0 && (load ^ mapMatch)) || load || (current.isMainMenu || current.isLoading == 153 || current.isLoading == 139) || (current.isSavingGame == 46179);
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
