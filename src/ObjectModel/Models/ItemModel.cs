using MessagePack;

namespace ObjectModel.Models;

[MessagePackObject(AllowPrivate = true)]
internal class ItemModel : GameObjectModel
{
    [Key(4)]
    public bool IsPlayerVisible { get; set; }

    public ItemModel(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public ItemModel()
    {
    }
}