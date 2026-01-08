using System.Collections.Generic;
using MessagePack;

namespace ObjectModel.Models;

[MessagePackObject(AllowPrivate = true)]
internal class RoomModel : GameObjectModel, IItemModel
{
    [Key(3)]
    public List<NamedRef> Items { get; set; } = [];

    [Key(4)]
    public List<NamedRef> NPCS { get; set; } = [];

    public RoomModel(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
