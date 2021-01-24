state("The Spy Who Shrunk Me") { }

startup {
	// 	public static long magicNumber = 0x1337133742069;
	//  A fairly janky sig scan, it actually points into the inGameTime variable of the struct but meh
	vars.AutoSplitterData = new SigScanTarget("00 00 37 13 37 13 37 13 37 13 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00");
}

init {
	Thread.Sleep(2500);

	vars.timerFound = false;
	vars.modifiedDLL = false;
	vars.gamePtr = IntPtr.Zero;

	vars.gameModule = modules.First();

	var dllInfo = new FileInfo(vars.gameModule.FileName + "/../The Spy Who Shrunk Me_Data/Managed/Assembly-CSharp.dll");

	// A comparatively janky fix, but I don't think its entirely worth it to spend the processing time making a hash.
	vars.modifiedDLL = dllInfo.Length == 749568;

	if(!vars.modifiedDLL) {
		print("[Spy Who Shrunk Me ASL]: Modified DLL not installed...");
		MessageBox.Show("Modified DLL not installed...\nGo to speedrun.com/swsm and install it!", "SWSM ASL", MessageBoxButtons.OK, MessageBoxIcon.Error);
		return false;
	}

	// Loop until we find the proper address for the AutoSplitterData class
	for(;;) {
		foreach(var page in game.MemoryPages()) {
			var scanner = new SignatureScanner(game, page.BaseAddress, (int)page.RegionSize);
			if(vars.gamePtr == IntPtr.Zero) vars.gamePtr = scanner.Scan(vars.AutoSplitterData);
			
			if(vars.gamePtr != IntPtr.Zero) {
				break;
			}
		}
		if(vars.gamePtr != IntPtr.Zero) {
			vars.timerFound = true;
			break;
		}
	}

	if(!vars.timerFound) return false;

	print("AutoSplitterData pointer found...");
	print("Pointer: 0x" + vars.gamePtr.ToString("X"));
	vars.gamePtr += 2 + sizeof(long);

	vars.watchers = new MemoryWatcherList();


	// public static double inGameTime = 0;
	print("inGameTime: 0x" + vars.gamePtr.ToString("X"));
	vars.watchers.Add(new MemoryWatcher<double>(vars.gamePtr) { Name = "inGameTime" });
	vars.gamePtr += sizeof(double);

	// public static int isRunning;
	print("isRunning: 0x" + vars.gamePtr.ToString("X"));
	vars.watchers.Add(new MemoryWatcher<int>(vars.gamePtr) { Name = "isRunning" });
	vars.gamePtr += sizeof(int);

	print("isLoading: 0x" + vars.gamePtr.ToString("X"));
	// public static bool isLoading;
	vars.watchers.Add(new MemoryWatcher<bool>(vars.gamePtr) { Name = "isLoading" });
	vars.gamePtr += sizeof(bool);

	print("isOnMainMenu: 0x" + vars.gamePtr.ToString("X"));
	// public static bool isOnMainMenu;
	vars.watchers.Add(new MemoryWatcher<bool>(vars.gamePtr) { Name = "isOnMainMenu" });
	vars.gamePtr += sizeof(bool);

	vars.gamePtr += 2; // Padding?

	print("levelID: 0x" + vars.gamePtr.ToString("X"));
	// public static int levelID;
	vars.watchers.Add(new MemoryWatcher<int>(vars.gamePtr) { Name = "levelID"});
	vars.gamePtr += sizeof(int);

	print("completedCurrentLevel: 0x" + vars.gamePtr.ToString("X"));
	// public static bool completedCurrentLevel;
	vars.watchers.Add(new MemoryWatcher<bool>(vars.gamePtr) { Name = "missionReport"});
	vars.gamePtr += sizeof(bool);

	vars.watchers.UpdateAll(game);
}

update {
	if(vars.timerFound) {
		vars.watchers.UpdateAll(game);
		// print(vars.watchers["isLoading"].Current.ToString());
	}
}

isLoading {
	return vars.timerFound; 
}

/* 
Level IDs:
	"Rails To Russia": 0x00
	"Software Company Shenanigans": 0x01
	"Operation: Improbable": 0x02
	"A Mind of Madness": 0x03
	"Beyond The Books": 0x04
	"Artifical Insanity": 0x05
	"A Mind of Madness: Redux": 0x06
	"Person of Importance": 0x07
	"Rocket Jockey": 0x08
	"Sayonara Smoothspy": 0x09
	"Get Comfy": 0x0A
	"Quaternion Identity": 0x0B
*/

start {
	if(vars.timerFound)
		return ( (vars.watchers["isLoading"].Current == false && vars.watchers["isLoading"].Old == true) && vars.watchers["levelID"].Current == 0);
	else 
		return false;
}

reset {
	if(vars.timerFound)
		return (vars.watchers["levelID"].Current == -1 && vars.watchers["isOnMainMenu"].Current);
	return false;
}

split {
	if(vars.timerFound)
		return vars.watchers["missionReport"].Current == true && vars.watchers["missionReport"].Old == false;
	return false;
}


gameTime {
	return TimeSpan.FromSeconds(vars.watchers["inGameTime"].Current);
}

/*
using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal static class AutoSplitterData
{
	static AutoSplitterData()
	{
		AutoSplitterData.isRunning = 0;
		AutoSplitterData.isOnMainMenu = false;
	}
	public static long magicNumber = 338036264280169L;
	public static double inGameTime = 0.0;
	public static int isRunning;
	public static bool isLoading;
	public static bool isOnMainMenu;
	public static int levelID = -1;
	public static bool completedCurrentLevel;
}
*/