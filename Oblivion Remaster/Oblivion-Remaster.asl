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

    settings.Add("Auto-Splitting", true, "Auto-Splitting");
    
    vars.WorldTransitions = new Dictionary<string, string>() {
        { "Sewers Finish", "L_ImperialSewers03" }
    };

    foreach (var item in vars.WorldTransitions)
    {
        settings.Add(item.Value, false, item.Key, "Auto-Splitting");
    }

    settings.Add("end_game", true, "Main Game Finish", "Auto-Splitting");

    // Parent Setting
    settings.Add("Variable Information", true, "Variable Information");
    settings.Add("World", false, "Current World Name", "Variable Information");
    settings.Add("player_pos", false, "Player Position", "Variable Information");
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

    // Shoutouts to AngelicKnight :)
    // Both of these following properties are not actually exposed via UProperty in UE5
    // These are all C++ / internal engine objects
    // They helped me so they get bonus points :)
    var currentPlayerTrg = new SigScanTarget(3, "48 8B 05 ?? ?? ?? ?? 48 8B 88 A0 08 00 00") { OnFound = (p, s, ptr) => ptr + 0x4 + game.ReadValue<int>(ptr) };
    vars.currentPlayer = scn.Scan(currentPlayerTrg);

    var TESFORMListTrg = new SigScanTarget(9, "41 56 48 83 EC 30 4C 8B 35 ?? ?? ?? ?? 49 81 C6 08 01 00 00") { 
        OnFound = (p, s, ptr) => {
            var basePtr = (ptr + 0x4 + game.ReadValue<int>(ptr));
            return game.ReadValue<IntPtr>(basePtr);
        }
    };
    vars.TESFORMList = scn.Scan(TESFORMListTrg);

    // Throwing in case any base pointers can't be found (yet, hopefully)
    if(uWorld == IntPtr.Zero || gameEngine == IntPtr.Zero || fNamePool == IntPtr.Zero || vars.currentPlayer == IntPtr.Zero || vars.TESFORMList == IntPtr.Zero)
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
        // Presumably, the first `???` is an instance of `VAltarUISubsystem`
        //  - If this is the case, the second `???` will be an instance of an `AVAltarHud`
        // Regardless, this is used for main menu detection
        //  - If the watched value is NULLPTR, then we're *NOT* on the main menu
        new MemoryWatcher<ulong>(new DeepPointer(uWorld, 0x1B8, 0x108, 0x38, 0x30, 0x28, 0x340, 0x100)) { Name = "VMainMenuViewModel.OnSettingsMenuOpen"},

        // UWorld.OwningGameInstance.SubsystemCollection.???.???.???.VMessageMenuViewModel
        new MemoryWatcher<ulong>(new DeepPointer(uWorld, 0x1B8, 0x108, 0x38, 0x30, 0x28, 0x370, 0x100)) { Name = "VMessageMenuViewModel.Message"},
        new MemoryWatcher<byte>(new DeepPointer(uWorld, 0x1B8, 0x108, 0x38, 0x30, 0x28, 0x370, 0xF8)) { Name = "VMessageMenuViewModel.MenuType"},

        // UWorld.NavigationSystem.MainNavData.Parent.Parent
        // This is used for detecting what "original game" world the player is in
        new MemoryWatcher<ulong>(new DeepPointer(uWorld, 0x148, 0x28, 0x20, 0x20)) { 
            Name = "OblivionWorld",
            FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull
        },

        // UWorld.OwningGameInstance.???
        // I'd love to figure out what this actually points to
        // But in order to do that, I gotta pop the game into Ghidra and everything
        // This is some sort of internal C++-allocated (or private) property
        // Nothing exposed via a UProperty
        new MemoryWatcher<uint>(new DeepPointer(uWorld, 0x1B8, 0x1C0)) { 
            Name = "GameInstance.UNK1",
        },

        new MemoryWatcher<ulong>(new DeepPointer(vars.currentPlayer)) { 
            Name = "CurrentPlayer",
            FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull
        },
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
    
    vars.ReadFVector = (Func<ulong, Dictionary<string, double>>)(start => {
        // If the FVector is pointing to NULL, just return a blank dictionary
        if(start == 0x00) 
            return new Dictionary<string, double>() { };

        var X = game.ReadValue<double>((IntPtr)start + (0x08 * 0));
        var Y = game.ReadValue<double>((IntPtr)start + (0x08 * 1));
        var Z = game.ReadValue<double>((IntPtr)start + (0x08 * 2));

        return new Dictionary<string, double>() {
            { "X", X },
            { "Y", Y },
            { "Z", Z }
        };
    });

    vars.ReadTESVector = (Func<ulong, Dictionary<string, float>>)(start => {
        // If the vector is pointing to NULL, just return a blank dictionary
        if(start == 0x00) 
            return new Dictionary<string, float>() { };
        
        var X = game.ReadValue<float>((IntPtr)start + (0x04 * 0));
        var Y = game.ReadValue<float>((IntPtr)start + (0x04 * 1));
        var Z = game.ReadValue<float>((IntPtr)start + (0x04 * 2));

        return new Dictionary<string, float>() {
            { "X", X },
            { "Y", Y },
            { "Z", Z }
        };
    });

    // This caches all of the Quest IDs -> memory addresses for all of the quests, this way we're not doing unnecessary pointer redirections every refresh
    var questMapping = new Dictionary<string, IntPtr>() { }; 

    // This returns a dictionary used for storing all the quest states
    // It maps a QuestID to it's various properties (i.e. stage + status)
    vars.UpdateQuestStates = (Func<IntPtr, Dictionary<string, Dictionary<string, object>>>)(TESFormMapping => {
        if(TESFormMapping == IntPtr.Zero) 
            return new Dictionary<string, Dictionary<string, object>>() { };

        var result = new Dictionary<string, Dictionary<string, object>>() { };

        if(questMapping.Count == 0) {
            print("Filling Quest ID -> Address Mapping...");

            var startTime = DateTime.Now.Ticks;
            
            var questPtr = TESFormMapping + 0x108;
            do {
                var nextQuest = game.ReadValue<IntPtr>(questPtr);
                
                var questID = game.ReadString(
                    game.ReadValue<IntPtr>(nextQuest + 0xC0), 
                    0xFF // Note: idk how long this actually is, but it felt good enough :)
                );

                // The previous quest stores the pointer to the next quest directly after
                questPtr = game.ReadValue<IntPtr>(questPtr + 0x08);

                questMapping[questID] = nextQuest;
            }
            while(questPtr != IntPtr.Zero);
            
            var endTime = DateTime.Now.Ticks;
            var durationMS = (endTime - startTime) / TimeSpan.TicksPerMillisecond;
            print("Completed filling Quest ID -> Address Mapping in " + (durationMS.ToString() + "ms"));
        }

        foreach(var mapping in questMapping) {
            var questID = mapping.Key;
            var questPtr = mapping.Value;

            result[questID] = new Dictionary<string, object>() {
                { "status", game.ReadValue<byte>(questPtr + 0x78) },
                { "stage", game.ReadValue<byte>(questPtr + 0xB8) }
            };
        }

        return result;
    });


    vars.Watchers.UpdateAll(game);
    vars.EModalMenuLayoutType = new Dictionary<byte, string>() {
        { 0x00, "Default"        },
        { 0x01, "QuestAdded"     },
        { 0x02, "QuestUpdated"   },
        { 0x03, "SkillIncreased" },
        { 0x04, "OutOfPrison"    },
        { 0x05, "SellBuy"        },
        { 0x06, "LoadSave"       },
        { 0x07, "RaceSex"        },
        { 0x08, "Recharge"       },
        { 0x09, "Repair"         }
    };

    // Note: Unlike `EModalMenuLayoutType`, this enum is actually a flag
    // So in order to check if the HUDVisibility is Reticle, you need to do & 0x01
    vars.EHUDVisibility = new Dictionary<string, byte>() {
        { "None",      0x00 },
        { "Main",      0x01 },
        { "Info",      0x02 },
        { "Reticle",   0x04 },
        { "Subtitle",  0x08 },
        { "Breath",    0x16 },
        { "MapPage" ,  0x32 },
        { "QuickKeys", 0x64 }
    };

    print("Game Engine: " + gameEngine.ToString("X"));
    print("World: " + uWorld.ToString("X"));
    print("CurrentPlayer: " + vars.currentPlayer.ToString("X"));
    print("PlayerController: " + vars.Watchers["CurrentPlayer"].Current.ToString("X"));
    print("TESFORMList: " + vars.TESFORMList.ToString("X"));

    // Sets the var world from the memory watcher
    current.world = old.world = vars.FNameToString(vars.Watchers["worldFName"].Current);
    current.isLoading = false;
    current.questStates = old.QuestStates = vars.UpdateQuestStates(vars.TESFORMList);
}

update
{
    vars.Watchers.UpdateAll(game);
        
    // Get the current world name as string, only if *UWorld isnt null
    var worldFName = vars.Watchers["OblivionWorld"].Current;
    current.world = worldFName != 0x0 ? vars.ReadFNameOfObject(worldFName) : "None";

    // Read the current player position
    current.playerPosition = vars.ReadTESVector(vars.Watchers["CurrentPlayer"].Current + 0x64);
    current.questStates = vars.UpdateQuestStates(vars.TESFORMList);

    var gameInstanceState = vars.Watchers["GameInstance.UNK1"].Current;
    var bIsVisibleGlobal = vars.Watchers["VUIStateSubsystem.bIsVisibleGlobal"].Current;
    if(bIsVisibleGlobal == 0x00) {
        // This property also gets set to true for the main menu
        // So first, we gotta do some checking to see if we're on the main menu...

        var onSettingsMenuOpen = vars.Watchers["VMainMenuViewModel.OnSettingsMenuOpen"].Current;
        if(onSettingsMenuOpen == 0x00) {
            current.isLoading = true;
        }
    }
    else {
        current.isLoading = (gameInstanceState == 1);
    }

    // Prints the current map to the Livesplit layout if the setting is enabled
    if(settings["World"]) 
    {
        vars.SetTextComponent("World:", current.world.ToString());
        if (old.world != current.world) print("World: " + current.world.ToString());
    }

    if(settings["player_pos"]) 
    {
        var positionString = "(" + (Math.Round(current.playerPosition["X"], 2) + "," + (Math.Round(current.playerPosition["Y"], 2) + ",") + (Math.Round(current.playerPosition["Z"], 2))) + ")";
        vars.SetTextComponent("Location:", positionString);
    }
}

isLoading
{
    return current.isLoading;
}

start 
{
    // First we're gonna check if the current open menu (if any exist) is a RaceSex menu
    // If it is, let's check whether or not the message has changed from valid to NULLPTR
    var menuType = vars.EModalMenuLayoutType[vars.Watchers["VMessageMenuViewModel.MenuType"].Current];
    if(menuType == "RaceSex") {
        var message = vars.Watchers["VMessageMenuViewModel.Message"];

        if(message.Current == 0x00 && message.Old != 0x00) {
            var HUDVisibility = vars.Watchers["VUIStateSubsystem.HUDVisibility"].Current;
            // Check and see if the HUDVisibility flags were changed (at all)
            if(HUDVisibility != vars.EHUDVisibility["Reticle"]) {
                return true;
            }
        }
    }
}

split
{
    if(current.world != old.world && current.world == "None") {
        if(settings.ContainsKey(old.world) && settings[old.world] == true) {
            return true;
        }
    }
    
    if(settings["end_game"]) {
        // First, we wanna check the current map
        // `L_ICTempleDistrictTempleOfTheOneMQ16` corresponds to the final quest-stage version of Temple of the One
        if(current.world == "L_ICTempleDistrictTempleOfTheOneMQ16") {
            // Now that we know we're in the final quest stage version of the Temple of the One
            // Let's start checking the quest state
            var MQ16Status = current.questStates["MQ16"];
            
            // This checks whether or not the MQ16 is currently "on"
            if((MQ16Status["status"] & 1) != 0x00) {
                // Then we're gonna check the stage
                // Realistically, the stage can't be set without the quest being *on*
                // But *shrug*
                var bIsPlayerInDialog = vars.Watchers["VUIStateSubsystem.bIsPlayerInDialog"].Current == 0x01;
                var bWasPlayerInDialog = vars.Watchers["VUIStateSubsystem.bIsPlayerInDialog"].Old == 0x01;

                var bJustLeftDialog = (bIsPlayerInDialog == false && bWasPlayerInDialog == true);
                if(MQ16Status["stage"] >= 46 && bJustLeftDialog) {
                    return true;
                }
            }
        }
    }
}

exit
{
    // Pause the timer if the game closes
    timer.IsGameTimePaused = true;
}