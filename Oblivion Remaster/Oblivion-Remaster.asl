state("OblivionRemastered-Win64-Shipping")
{

}

state("OblivionRemastered-WinGDK-Shipping")
{
}


startup
{
    // Creates text components for variable information
	vars.SetTextComponent = (Action<string, string>)((id, text) =>
	{
        var textSettings = timer.Layout.Components.Where(x => x.GetType().Name == "TextComponent").Select(x => x.GetType().GetProperty("Settings").GetValue(x, null));
        var textSetting = textSettings.FirstOrDefault(x => (x.GetType().GetProperty("Text1").GetValue(x, null) as string) == id);
        if (textSetting == null)
        {
            var textComponentAssembly = Assembly.LoadFrom("Components\\LiveSplit.Text.dll");
            var textComponent = Activator.CreateInstance(textComponentAssembly.GetType("LiveSplit.UI.Components.TextComponent"), timer);
            timer.Layout.LayoutComponents.Add(new LiveSplit.UI.Components.LayoutComponent("LiveSplit.Text.dll", textComponent as LiveSplit.UI.Components.IComponent));

            textSetting = textComponent.GetType().GetProperty("Settings", BindingFlags.Instance | BindingFlags.Public).GetValue(textComponent, null);
            textSetting.GetType().GetProperty("Text1").SetValue(textSetting, id);
        }

        if (textSetting != null)
        textSetting.GetType().GetProperty("Text2").SetValue(textSetting, text);
    });

    // Parent Setting
	settings.Add("Variable Information", true, "Variable Information");
    settings.Add("World", false, "Current World Name", "Variable Information");
}

init
{
    // You need to do this in order to unpause the timer if the game closes mid-run
    timer.IsGameTimePaused = false;

    // Scanning the MainModule for static pointers to GSyncLoadCount, UWorld, UEngine and FNamePool
    var scn = new SignatureScanner(game, game.MainModule.BaseAddress, game.MainModule.ModuleMemorySize);


    var uWorldTrg = new SigScanTarget(3, "48 8b 1d ?? ?? ?? ?? 48 85 db 74 ?? 41 b0") { OnFound = (p, s, ptr) => ptr + 0x4 + game.ReadValue<int>(ptr) };
    var uWorld = scn.Scan(uWorldTrg);

    var gameEngineTrg = new SigScanTarget(3, "48 89 05 ?? ?? ?? ?? 48 85 c9 74 ?? e8 ?? ?? ?? ?? 48 8d 4d") { OnFound = (p, s, ptr) => ptr + 0x4 + game.ReadValue<int>(ptr) };
    var gameEngine = scn.Scan(gameEngineTrg);
    var fNamePoolTrg = new SigScanTarget(3, "48 8d 05 ?? ?? ?? ?? eb ?? 48 8d 0d ?? ?? ?? ?? e8 ?? ?? ?? ?? c6 05 ?? ?? ?? ?? ?? 0f 10 07") { OnFound = (p, s, ptr) => ptr + 0x4 + game.ReadValue<int>(ptr) };
    var fNamePool = scn.Scan(fNamePoolTrg);

    // Throwing in case any base pointers can't be found (yet, hopefully)
    if(uWorld == IntPtr.Zero || gameEngine == IntPtr.Zero || fNamePool == IntPtr.Zero)
    {
        throw new Exception("One or more base pointers not found - retrying");
    }

	vars.Watchers = new MemoryWatcherList
    {
        // UWorld
        new MemoryWatcher<ulong>(new DeepPointer(uWorld)) { Name = "GWorld"},
        new MemoryWatcher<ulong>(new DeepPointer(gameEngine)) { Name = "GameEngine"},

        new MemoryWatcher<ulong>(new DeepPointer(uWorld, 0x18)) { Name = "worldFName"},
        new MemoryWatcher<ulong>(new DeepPointer(uWorld, 0x1B8)) { Name = "GameInstance"},

        // For the following, the last offset on the pointer is used for determining the actual property value
        // The comments above it represent the path to get to the UObject holding said property.

        // UWorld.OwningGameInstance.SubsystemCollection.???.VUIStateSubsystem
        new MemoryWatcher<byte>(new DeepPointer(uWorld, 0x1B8, 0x108, 0x2D8, 0x50)) { Name = "VUIStateSubsystem.HUDVisibility"},
        new MemoryWatcher<byte>(new DeepPointer(uWorld, 0x1B8, 0x108, 0x2D8, 0x51)) { Name = "VUIStateSubsystem.bIsVisibleGlobal"},
        new MemoryWatcher<byte>(new DeepPointer(uWorld, 0x1B8, 0x108, 0x2D8, 0x52)) { Name = "VUIStateSubsystem.bIsPlayerInDialog"},

        // UWorld.OwningGameInstance.SubsystemCollection.???.???.???.VMainMenuViewModel
        // This is used for main menu detection -- If the value is NULLPTR, then we're *NOT* on the main menu
        new MemoryWatcher<ulong>(new DeepPointer(uWorld, 0x1B8, 0x108, 0x38, 0x30, 0x28, 0x340, 0x100)) { Name = "VMainMenuViewModel.OnSettingsMenuOpen"},
    };

    // Translating FName to String, this *could* be cached
    var cachedFNames = new Dictionary<ulong, string>();
    vars.FNameToString = (Func<ulong, string>)(fName =>
    {
        string name;
        if (cachedFNames.TryGetValue(fName, out name))
            return name;

        var number   = (fName & 0xFFFFFFFF00000000) >> 0x20;
        var chunkIdx = (fName & 0x00000000FFFF0000) >> 0x10;
        var nameIdx  = (fName & 0x000000000000FFFF) >> 0x00;
        var chunk = game.ReadPointer(fNamePool + 0x10 + (int)chunkIdx * 0x8);
        var nameEntry = chunk + (int)nameIdx * 0x2;
        var length = game.ReadValue<short>(nameEntry) >> 6;
        name = game.ReadString(nameEntry + 0x2, length);

        cachedFNames[fName] = name;
        return name;
    });
    vars.ReadFNameOfObject = (Func<ulong, string>)(obj => vars.FNameToString(game.ReadValue<ulong>((IntPtr)obj + 0x18)));

    vars.Watchers.UpdateAll(game);
    
    print("Game Engine: " + gameEngine.ToString("X"));

    // Sets the var world from the memory watcher
    current.world = old.world = vars.FNameToString(vars.Watchers["worldFName"].Current);
    current.isLoading = false;
}

update
{
    vars.Watchers.UpdateAll(game);

    // Get the current world name as string, only if *UWorld isnt null
    var worldFName = vars.Watchers["worldFName"].Current;
    current.world = worldFName != 0x0 ? vars.FNameToString(worldFName) : old.world;
    
    var bIsVisibleGlobal = vars.Watchers["VUIStateSubsystem.bIsVisibleGlobal"].Current;
    if(bIsVisibleGlobal == 0x00) {
        // This property also gets set to true for the main menu
        // So first, we gotta do some checking to see if we're on the main menu...

        var onSettingsMenuOpen = vars.Watchers["VMainMenuViewModel.OnSettingsMenuOpen"].Current;
        current.isLoading = (onSettingsMenuOpen == 0x00);
    }
    else {
        current.isLoading = false;
    }

    // Prints the current map to the Livesplit layout if the setting is enabled
    if(settings["World"]) 
    {
        vars.SetTextComponent("World:",current.world.ToString());
        if (old.world != current.world) print("World:" + current.world.ToString());
    }
}

isLoading
{
    return current.isLoading;
}

exit
{
    // Pause the timer if the game closes
    timer.IsGameTimePaused = true;
}
