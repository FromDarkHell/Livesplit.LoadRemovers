// Remnant From The Ashes Autosplitter 1.1.2
// Created by FromDarkHell

// == Version States ==

// Use an empty state descriptor instead for whenever we find an unsupported version
state("Remnant-Win64-Shipping") {}

// == Game Version: 248,851DP ==
state("Remnant-Win64-Shipping", "EGS-248851")
{
	bool isLoading  : 0x033511F0, 0x7C;
	bool isMainMenu : 0x03576220, 0x30, 0x834;
}

state("Remnant-Win64-Shipping", "STEAM-248851")
{
	bool isLoading  : 0x034A78E0, 0x28, 0x3E8, 0x10, 0x2B8, 0xF0, 0x588, 0x500;
	bool isMainMenu : 0x03576190, 0xF8, 0x408, 0x238, 0x10;
}

// == Game Version: 249,276DP ==
state("Remnant-Win64-Shipping", "EGS-249276")
{
	bool isLoading  : 0x033512D0, 0x4;
	bool isMainMenu : 0x035B2030, 0x140, 0xC68;
}

// == Game Version: 250,802DP ==
state("Remnant-Win64-Shipping", "EGS-250802") {
	bool isLoading  : 0x03352310, 0x0;
	bool isMainMenu : 0x031CBBC0, 0x7B0, 0x68, 0x98, 0x80, 0x40, 0xC8;
}
// =============================

startup {

	/*
	if(timer.CurrentTimingMethod == TimingMethod.RealTime)
	{
		var timingMessage = MessageBox.Show("This game uses Game Time (time without loads) as the main timing method.\nLiveSplit is currently set to show Real Time (time INCLUDING loads).\nWould you like the timing method to be set to Game Time for you?", "Remnant From The Ashes ASL | LiveSplit", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
		if (timingMessage == DialogResult.Yes) timer.CurrentTimingMethod = TimingMethod.GameTime;
	}
	*/

	// NOTE: IF YOU'RE GOING TO ADD A NEW VERSION, PUT THE HASH IN THIS DICTIONARY!
	// You'll also want to add the steam version of the hash if you've got the access to it

	vars.hashToVersion = new Dictionary<string, string> {
		// == Epic Games ==
		{ "E9A7DB969B885D91CCD13AD61DDF7390", "248851" },
		{ "22FDCA89829A39D637E3316C4DC40D6C", "249276" },
		{ "D24A54AD8E92573D7692400CC552ABB9", "250802" }
		// == Steam ==
	};

}

init
{
	vars.isLoading = false;
	vars.gameModule = modules.First();
	// Default Value is something like: `K:\RemnantFromTheAshes\Remnant\Binaries\Win64\Remnant-Win64-Shipping.exe`
	// Technically you can (easily) have an Epic install without the .egstore due to the way Epic launches their games but y'know *meh*
	vars.gameStorefront = Directory.Exists(vars.gameModule.FileName + "/../../../../.egstore") ? "EGS" : "STEAM";
	
	// Creating a hash of the file seems to be a relatively *ok* way of detecting the version.
	// For some reason getting the product version from the exe itself, doesn't work, and it just returns an empty string
	// You could fix this by creating a DLL Component instead of an ASL, which is alot of effort and I don't feel like doing that.
	using (var stream = new FileStream(vars.gameModule.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 16 * 1024 * 1024))
	{
	    byte[] checksum = System.Security.Cryptography.MD5.Create().ComputeHash(stream);
	    vars.gameHash = BitConverter.ToString(checksum).Replace("-", String.Empty);
	}

	if(!vars.hashToVersion.ContainsKey(vars.gameHash)) {
		print("[Remnant ASL]: Unknown/Unsupported Game Hash: " + vars.gameHash.ToString());
		MessageBox.Show("Unknown Game Hash: \"" + vars.gameHash.ToString() + "\" \n Contact the developers for help!", "Remnant ASL", MessageBoxButtons.OK, MessageBoxIcon.Error);
		return;
	}

	version = (vars.gameStorefront + "-" + vars.hashToVersion[vars.gameHash]);
	print("[Remnant ASL]: Game Storefront: " + vars.gameStorefront.ToString());
	print("[Remnant ASL]: Game Hash: " + vars.gameHash.ToString());
	print("[Remnant ASL]: ASL Version: " + version.ToString());
}

update {
	vars.isLoading = current.isLoading || current.isMainMenu;
}

isLoading
{
	return vars.isLoading;
}

// Pause the IGT timer when the game closes
exit
{
	timer.IsGameTimePaused = true;
}
