using MessagePack;

namespace ObjectModel.Models;

[MessagePackObject(AllowPrivate = true)]
internal class CharacterModel : GameObjectModel
{
    public CharacterModel(string name, string description, bool isNPC)
    {
        Name = name;
        Description = description;
        IsNPC = isNPC;
    }
    public CharacterModel()
    {
    }

    [Key(3)]
    public bool IsNPC { get; set; }
}