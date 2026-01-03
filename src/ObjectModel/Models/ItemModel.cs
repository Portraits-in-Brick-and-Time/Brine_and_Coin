using MessagePack;

namespace ObjectModel.Models;

[MessagePackObject(AllowPrivate = true)]
internal class ItemModel : GameObjectModel
{
    public ItemModel(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public ItemModel()
    {
    }
}