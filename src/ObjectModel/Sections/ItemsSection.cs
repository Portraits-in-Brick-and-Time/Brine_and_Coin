using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class ItemsSection(ElfFile file) : CustomSection(file)
{
    public List<ItemModel> Items { get; } = [];

    public override string Name => ".items";
    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Items.Count);
        foreach (var item in Items)
        {
            var start = (ulong)writer.BaseStream.Position;
            writer.Write(MessagePackSerializer.Serialize(item));

            AddSymbol(item.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<ItemModel>(reader.BaseStream);
            Items.Add(model);
        }
    }

    public int IndexOf(string name)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            if (Items[i].Name == name)
            {
                return i;
            }
        }

        throw new KeyNotFoundException($"Item '{name}' not found.");
    }
}
