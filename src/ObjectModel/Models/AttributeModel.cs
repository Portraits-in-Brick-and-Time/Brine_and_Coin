using MessagePack;

namespace ObjectModel;

[MessagePackObject(AllowPrivate = true)]
internal class AttributeModel
{
    [Key(0)]
    public string Name { get; set; }

    [Key(1)]
    public string Description { get; set; }

    [Key(2)]
    public int Min { get; set; }

    [Key(3)]
    public int Max { get; set; }

    [Key(4)]
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