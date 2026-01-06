using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel;

internal abstract class ModelSection<T>(ElfFile file) : CustomSection(file)
    where T : GameObjectModel
{
    public List<T> Elements { get; } = [];

    protected virtual bool AddElementsToSymbolTable() => true;

    protected override void Write(BinaryWriter writer)
    {
        foreach (var element in Elements)
        {
            var start = (ulong)writer.BaseStream.Position;
            writer.Write(MessagePackSerializer.Serialize(element));

            if (AddElementsToSymbolTable())
            {
                AddSymbol(element.Name, start, (ulong)writer.BaseStream.Position - start);
            }
        }
    }

    protected override void Read(BinaryReader reader)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var model = MessagePackSerializer.Deserialize<T>(reader.BaseStream);

            Elements.Add(model);
        }
    }
}
