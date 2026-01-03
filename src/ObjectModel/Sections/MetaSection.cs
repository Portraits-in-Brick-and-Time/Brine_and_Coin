using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;

namespace ObjectModel.Sections;

internal class MetaSection(ElfFile file) : CustomSection(file)
{
    public override string Name => ".meta";

    public Dictionary<string, object> Properties { get; set; } = [];

    protected override void Read(BinaryReader reader)
    {
        Properties = MessagePackSerializer.Deserialize<Dictionary<string, object>>(reader.BaseStream);
    }

    protected override void Write(BinaryWriter writer)
    {
        writer.Write(MessagePackSerializer.Serialize(Properties));
    }
}