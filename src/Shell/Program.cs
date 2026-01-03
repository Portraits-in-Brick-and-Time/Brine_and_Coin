using System.Reflection;
using LibObjectFile.Elf;
using NetAF.Assets;
using NetAF.Assets.Characters;
using NetAF.Assets.Locations;
using NetAF.Logic;
using NetAF.Rendering.FrameBuilders;
using NetAF.Targets.Console;
using NetAF.Utilities;
using ObjectModel;
using ObjectModel.IO;
using Shell.Core;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Structs;
using Splat;
using Velopack;
using Velopack.Sources;

namespace BrineAndCoin;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "--version")
        {
            var gitVersionInformationType = Assembly.GetEntryAssembly()!.GetType("GitVersionInformation");
            var field = gitVersionInformationType!.GetField("MajorMinorPatch");

            Console.WriteLine(field!.GetValue(null));
            return;
        }

        VelopackApp
                   .Build()
#if WINDOWS
                       .OnAfterInstallFastCallback(v => new Shortcuts().CreateShortcutForThisExe(ShortcutLocation.Desktop))
#endif
                   .Run();

#if RELEASE
        await CheckForUpdatesAsync();
#endif

        var reader = new GameAssetReader(File.OpenRead("Assets/core_assets.elf"));
        reader.File.Print(Console.Out);

        var engine = new MiniAudioEngine();
        var playbackDevice = engine.InitializePlaybackDevice(null, AudioFormat.DvdHq);
        playbackDevice.Start();

        var writer = new TypeWriter(engine, playbackDevice);
        Locator.CurrentMutable.RegisterConstant(engine);
        Locator.CurrentMutable.RegisterConstant(playbackDevice);
        Locator.CurrentMutable.RegisterConstant(writer);

        //await writer.PlayAsync("Assets/Texts/intro.txt", "Assets/Voice/intro.mp3");

        InitAndExecuteGame(reader);
        Console.Clear();
    }

    private static PlayableCharacter CreatePlayer()
    {
        return new PlayableCharacter("Dave", "A young boy on a quest to find the meaning of life.");
    }

    private static EndCheckResult IsGameComplete(Game game)
    {
        if (!game.Player.FindItem("Holy Grail", out _))
            return EndCheckResult.NotEnded;

        return new EndCheckResult(true, "Game Complete", "You have the Holy Grail!");
    }

    private static EndCheckResult IsGameOver(Game game)
    {
        if (game.Player.IsAlive)
            return EndCheckResult.NotEnded;

        return new EndCheckResult(true, "Game Over", "You died!");
    }

    static void InitAndExecuteGame(GameAssetReader reader)
    {
        var gameCreator = Game.Create(
                        new GameInfo("Portraits in Brick and Time - Brine and Coin", "Brine and Coin is an open source text adventure where you experience the history of Schwäbisch Hall.", "Chris Anders"),
                        "",
                        AssetGenerator.Retained(CreateWorld(reader), CreatePlayer()),
                        new GameEndConditions(IsGameComplete, IsGameOver),
                        new GameConfiguration(new ConsoleAdapter(), FrameBuilderCollections.Console, new(90, 30), StartModes.Scene));

        GameExecutor.Execute(gameCreator, new ConsoleExecutionController());
    }

    private static Overworld CreateWorld(GameAssetReader reader)
    {
        var worldName = reader.CustomSections.MetaSection.Properties["world.name"];
        var worldDescription = reader.CustomSections.MetaSection.Properties["world.description"];

        var overworld = new Overworld(worldName.ToString(), worldDescription.ToString());

        foreach (var region in reader.CustomSections.RegionsSection.Regions)
        {
            overworld.AddRegion(region);
        }

        return overworld;
    }

    private static async Task CheckForUpdatesAsync()
    {
        var mgr = new UpdateManager(new GithubSource("https://github.com/Portraits-in-Brick-and-Time/Brine-and-Coin", null, false));

        Console.WriteLine("Checking for updates...");
        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            Console.Clear();
            return;
        }

        await mgr.DownloadUpdatesAsync(newVersion);

        mgr.ApplyUpdatesAndRestart(newVersion);
    }
}