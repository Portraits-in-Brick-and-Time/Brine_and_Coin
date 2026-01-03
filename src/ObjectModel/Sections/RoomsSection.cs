using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class RoomsSection(ElfFile file) : CustomSection(file)
{
    public List<RoomModel> Rooms { get; } = [];

    public override string Name => ".rooms";
    protected override void Write(BinaryWriter writer)
    {
        writer.Write(Rooms.Count);
        foreach (var room in Rooms)
        {
            var start = (ulong)writer.BaseStream.Position;
            writer.Write(MessagePackSerializer.Serialize(room));

            AddSymbol(room.Name, start, (ulong)writer.BaseStream.Position - start);
        }
    }

    protected override void Read(BinaryReader reader)
    {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var model = MessagePackSerializer.Deserialize<RoomModel>(reader.BaseStream);
            Rooms.Add(model);
        }
    }

    public RoomModel GetByName(string name)
    {
        return Rooms.FirstOrDefault(r => r.Name == name);
    }
}
