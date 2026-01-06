using MessagePack;
using ObjectModel.Models;

namespace ObjectModel;

[MessagePackObject(AllowPrivate = true)]
internal class AttributeModel : GameObjectModel
{
    [Key(3)]
    public int Min { get; set; }

    [Key(4)]
    public int Max { get; set; }

    [Key(5)]
    public bool Visible { get; set; }

    public AttributeModel(string name, string description, int min, int max, bool visible)
    {
        Name = name;
        Description = description;
        Min = min;
        Max = max;
        Visible = visible;
    }

    public AttributeModel()
    {
        
    }
}