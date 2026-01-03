using System.Collections.Generic;
using MessagePack;

namespace ObjectModel.Models;

[MessagePackObject(AllowPrivate = true)]
internal class RoomModel : GameObjectModel
{
    [Key(3)]
    public List<NamedRef> Items { get; set; }

    public RoomModel(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
