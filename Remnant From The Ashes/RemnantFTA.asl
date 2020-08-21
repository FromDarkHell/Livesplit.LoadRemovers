// Remnant From The Ashes Autosplitter 1.0
// Initially setup by FromDarkHell


state("Remnant-Win64-Shipping", "EGS") // Pointers by FromDarkHell
{
	bool isLoading  : 0x03351208, 0x38;
	bool isMainMenu : 0x03576220, 0x30, 0x834;
}

 
state("Remnant-Win64-Shipping", "STEAM") // Pointers by Ramnerock
{
	bool isLoading  : 0x034A78E0, 0x28, 0x3E8, 0x10, 0x2B8, 0xF0, 0x588, 0x500;
	bool isMainMenu : 0x03576190, 0xF8, 0x408, 0x238, 0x10;
}

init
{
	vars.isLoading = false;
	vars.gameModule = modules.First();
	// Default Value is something like: `K:\RemnantFromTheAshes\Remnant\Binaries\Win64\Remnant-Win64-Shipping.exe`
	vars.gameStorefront = Directory.Exists(vars.gameModule.FileName + "/../../../../.egstore") ? "EGS" : "STEAM";

	print("Game Storefront: " + vars.gameStorefront.ToString());
	version = vars.gameStorefront;
}

update {
	vars.isLoading = current.isLoading || current.isMainMenu;
}


isLoading
{
	return vars.isLoading;
}
