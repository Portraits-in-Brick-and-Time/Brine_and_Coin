using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibObjectFile.Elf;
using NetAF.Assets;
using NetAF.Assets.Attributes;
using NetAF.Assets.Characters;
using NetAF.Assets.Locations;
using NetAF.Commands;
using NetAF.Commands.Persistence;
using ObjectModel.Evaluation;
using ObjectModel.IO;
using ObjectModel.Models;
using Splat;

namespace ObjectModel;

public class GameAssetLoader
{
    private readonly List<Attribute> _attributes = [];
    private readonly List<Item> _items = [];
    private readonly List<PlayableCharacter> _players = [];
    private readonly List<NonPlayableCharacter> _npcs = [];
    private readonly List<Room> _rooms = [];
    private readonly List<Region> _regions = [];
    private readonly CustomSections _customSections;
    private readonly ElfSymbolTable _symbolTable;

    private GameAssetLoader(CustomSections customSections, ElfSymbolTable symbolTable)
    {
        this._customSections = customSections;
        this._symbolTable = symbolTable;
    }

    public static Overworld LoadFile(out PlayableCharacter[] players)
    {
        const string path = "Assets/core_assets.elf";
        if (!File.Exists(path))
        {
            throw new System.Exception($"Cannot load assets from '{path}'");
        }

        var reader = new GameAssetReader(File.OpenRead(path));
        var loader = new GameAssetLoader(reader.CustomSections, reader.SymbolTable);

        loader.Load();
        players = [.. loader._players];

        return loader.BuildWorld();
    }

    private void Load()
    {
        // Build all independent components first
        LoadAttributes();
        LoadItems();
        LoadCharacters();

        // Then build dependent components
        LoadRooms();
        LoadRegions();
    }

    private Overworld BuildWorld()
    {
        var worldName = _customSections.MetaSection.Properties["world.name"];
        var worldDescription = _customSections.MetaSection.Properties["world.description"];

        var overworld = new Overworld(worldName.ToString(), worldDescription.ToString());

        foreach (var region in _customSections.RegionsSection.Elements)
        {
            overworld.AddRegion(GetRegionByRef(region.NameRef));
        }

        return overworld;
    }

    private CustomCommand[] CreatePersistentCommands()
    {
        var folder = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "BrineAndCoin");
        var path = Path.Combine(
            folder,
            "savegame.json"
        );

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return [
            ConvertCommand(new Load(path)),
            ConvertCommand(new Save(path)),
        ];
    }

    private static CustomCommand ConvertCommand(ICommand command)
    {
        return new CustomCommand(command.Help, true, true, (game, args) =>
        {
            return command.Invoke(game);
        });
    }

    private Region GetRegionByRef(IndexedRef @ref)
    {
        if (@ref.Index < 0 || @ref.Index > _regions.Count)
        {
            throw new System.IndexOutOfRangeException($"Region {@ref.Index} not found.");
        }

        return _regions[@ref.Index];
    }

    private void LoadAttributes()
    {
        foreach (var attrModel in _customSections.AttributesSection.Elements)
        {
            var symbol = GetSymbolByRef(attrModel.NameRef, _customSections.AttributesSection);
            _attributes.Add(new Attribute(symbol.Name.Value, attrModel.Description, attrModel.Min, attrModel.Max, attrModel.Visible));
        }
    }

    ElfSymbol GetSymbolByRef(IndexedRef @ref, CustomSection section)
    {
        var sectionSymbols = _symbolTable.Entries.Where(entry => entry.SectionLink.Section == section.Section).ToArray();

        return sectionSymbols[@ref.Index];
    }

    private void LoadItems()
    {
        foreach (var itemModel in _customSections.ItemsSection.Elements)
        {
            var symbol = GetSymbolByRef(itemModel.NameRef, _customSections.ItemsSection);
            var item = new Item(symbol.Name.Value, itemModel.Description, commands: GetCommands(itemModel));
            ApplyAttributes(item, itemModel);

            _items.Add(item);
        }
    }

    private void LoadRegions()
    {
        foreach (var regionModel in _customSections.RegionsSection.Elements)
        {
            var symbol = GetSymbolByRef(regionModel.NameRef, _customSections.RegionsSection);
            var region = new Region(symbol.Name.Value, regionModel.Description, commands: GetCommands(regionModel));
            foreach (var (roomRef, (x, y, z)) in regionModel.Rooms)
            {
                region.AddRoom(_rooms.Find(r => r.Identifier.Name == roomRef.Name), x, y, z);
            }

            if (regionModel.StartRoom is not null)
            {
                region.SetStartRoom(_rooms.Find(r => r.Identifier.Name == regionModel.StartRoom?.Name));
            }

            ApplyAttributes(region, regionModel);
            _regions.Add(region);
        }
    }

    private void LoadRooms()
    {
        foreach (var roomModel in _customSections.RoomsSection.Elements)
        {
            var symbol = GetSymbolByRef(roomModel.NameRef, _customSections.ItemsSection);
            var room = new Room(symbol.Name.Value, roomModel.Description,
                commands: GetCommands(roomModel),
                exits: GetExits(roomModel),
                enterCallback: ApplyRoomTransition(roomModel.OnEnter),
                exitCallback: ApplyRoomTransition(roomModel.OnExit)
            );
            ApplyAttributes(room, roomModel);
            AddItems(room, roomModel);
            AddNpcs(room, roomModel);

            _rooms.Add(room);
        }
    }

    private Exit[] GetExits(RoomModel model)
    {
        var exits = new List<Exit>();

        foreach (var exitModel in model.Exits)
        {
            var exit = new Exit(exitModel.Direction, exitModel.IsLocked, new(exitModel.NameRef),
                new Description(exitModel.Description));
            exits.Add(exit);
        }

        return [.. exits];
    }

    private RoomTransitionCallback ApplyRoomTransition(List<IEvaluable> code)
    {
        if (code.Count == 0)
        {
            return null;
        }

        return new(transition =>
        {
            Evaluator evaluator = Locator.Current.GetService<Evaluator>();
            var reaction = evaluator.Evaluate<Reaction>(code, evaluator.RootScope);
            return new(reaction, true);
        });
    }

    private void AddNpcs(Room target, RoomModel model)
    {
        foreach (var characterRef in model.NPCS)
        {
            var character = GetByRef(characterRef, _npcs);
            target.AddCharacter(character);
        }
    }

    private void LoadCharacters()
    {
        foreach (var charModel in _customSections.CharactersSection.Elements)
        {
            var symbol = GetSymbolByRef(charModel.NameRef, _customSections.CharactersSection);
            var commands = GetCommands(charModel);
            Character character;
            if (charModel.IsNPC)
            {
                character = new NonPlayableCharacter(symbol.Name.Value, charModel.Description, commands: commands);
                _npcs.Add((NonPlayableCharacter)character);
            }
            else
            {
                character = new PlayableCharacter(symbol.Name.Value, charModel.Description, commands: CreatePersistentCommands().Concat(commands).ToArray());
                _players.Add((PlayableCharacter)character);
            }

            ApplyAttributes(character, charModel);
            AddItems(character, charModel);
        }
    }

    private void ApplyAttributes(IExaminable target, GameObjectModel model)
    {
        foreach (var (name, value) in model.Attributes)
        {
            target.Attributes.Add(GetAttributeByRef(name), value);
        }
    }

    private static CustomCommand[] GetCommands(GameObjectModel model)
    {
        var cmds = new List<CustomCommand>();
        foreach (var @ref in model.Commands)
        {
            if (CommandStore.TryGet(@ref.Name, out var cmd))
            {
                cmds.Add(cmd);
            }
            else
            {
                throw new KeyNotFoundException($"Command '{@ref.Name}' not found.");
            }
        }

        return [.. cmds];
    }

    private void AddItems(IItemContainer target, IItemModel model)
    {
        foreach (var itemRef in model.Items)
        {
            var item = GetByRef(itemRef, _items);
            target.AddItem(item);
        }
    }

    Attribute GetAttributeByRef(NamedRef @ref)
    {
        foreach (var attribute in _attributes)
        {
            if (attribute.Name == @ref.Name)
            {
                return attribute;
            }
        }

        throw new KeyNotFoundException($"Attribute '{@ref}' not found.");
    }

    T GetByRef<T>(NamedRef @ref, IList<T> collection)
        where T : IExaminable
    {
        foreach (var element in collection)
        {
            if (element.Identifier.Name == @ref.Name)
            {
                return element;
            }
        }

        throw new KeyNotFoundException($"Item '{@ref}' not found.");
    }

}