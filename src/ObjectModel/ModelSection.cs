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
        if (IsCompressed)
        {
            serializationOptions = serializationOptions.WithCompression(MessagePackCompression.Lz4Block);
        }

        for (int i = 0; i < Elements.Count; i++)
        {
            T element = Elements[i];
            var start = (ulong)writer.BaseStream.Position;
            writer.Write(MessagePackSerializer.Serialize(element, options: serializationOptions));

            if (AddElementsToSymbolTable())
            {
                AddSymbol(element.Name, start, (ulong)i);
            }
        }
    }

    protected override void Read(BinaryReader reader)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var model = MessagePackSerializer.Deserialize<T>(reader.BaseStream, options: serializationOptions);

            Elements.Add(model);
        }
    }
}
