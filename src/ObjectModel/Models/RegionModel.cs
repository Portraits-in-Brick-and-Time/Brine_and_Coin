using System.Collections.Generic;
using MessagePack;

namespace ObjectModel.Models;

#nullable enable

[MessagePackObject(AllowPrivate = true)]
internal class RegionModel : GameObjectModel
{
    [Key(3)]
    public Dictionary<NamedRef, Position> Rooms { get; set; }

    [Key(4)]
    public NamedRef? StartRoom { get; set; }

    public RegionModel()
    {
        Rooms = [];
    }

    public RegionModel(string name, string description, Dictionary<NamedRef, Position> rooms, NamedRef? startRoom = null)
    {
        Name = name;
        Description = description;
        Rooms = rooms;
        StartRoom = startRoom;
    }
}