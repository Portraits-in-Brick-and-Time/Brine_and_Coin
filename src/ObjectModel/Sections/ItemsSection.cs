using LibObjectFile.Elf;
using MessagePack;
using NetAF.Assets;
using NetAF.Assets.Characters;
using ObjectModel.Models;

namespace ObjectModel.Sections;

public class ItemsSection(ElfFile file) : CustomSection(file)
{
    public List<Item> Items { get; } = [];

    public override string Name => ".items";
    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Items.Count);
        foreach (var item in Items)
        {
            var start = (ulong)writer.BaseStream.Position;
            var model = ItemModel.FromItem(item);
            writer.Write(MessagePackSerializer.Serialize(model));
            AddSymbol(item.Identifier.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<ItemModel>(reader.BaseStream);
            var instance = model.ToItem();
            model.InstanciateAttributesTo(instance, CustomSections.AttributesSection);
            Items.Add(instance);
        }
    }

    public int IndexOf(string name)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            if (Items[i].Identifier.Name == name)
            {
                return i;
            }
        }

        throw new KeyNotFoundException($"Item '{name}' not found.");
    }
}

public class CharactersSection(ElfFile file) : CustomSection(file)
{
    public List<Character> Characters { get; } = [];

    public IReadOnlyList<PlayableCharacter> GetPlayers() => [.. Characters.OfType<PlayableCharacter>()];
    public IReadOnlyList<NonPlayableCharacter> GetNPCs() => [.. Characters.OfType<NonPlayableCharacter>()];

    public override string Name => ".characters";
    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Characters.Count);
        foreach (var character in Characters)
        {
            var start = (ulong)writer.BaseStream.Position;
            var model = CharacterModel.FromCharacter(character);
            writer.Write(MessagePackSerializer.Serialize(model));
            AddSymbol(character.Identifier.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<CharacterModel>(reader.BaseStream);
            var instance = model.ToCharacter();
            model.InstanciateAttributesTo(instance, CustomSections.AttributesSection);
            Characters.Add(instance);
        }
    }
}