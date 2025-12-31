using System.Security;
using LibObjectFile.Elf;
using MessagePack;

namespace ObjectModel.Sections;

public class AttributesSection(ElfFile file) : CustomSection(file)
{
    public Dictionary<string, NetAF.Assets.Attributes.Attribute> Attributes { get; } = [];

    public override string Name => ".attributes";

    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Attributes.Count);
        foreach (var (_, attribute) in Attributes)
        {
            var model = AttributeModel.FromAttribute(attribute);
            writer.Write(MessagePackSerializer.Serialize(model));
        }
    }

    protected override void Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<AttributeModel>(reader.BaseStream);
            Attributes.Add(model.Name, model.ToAttribute());
        }
    }
}