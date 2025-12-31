using LibObjectFile.Elf;

namespace ObjectModel;

public abstract class CustomSection(ElfFile file)
{
    protected ElfStreamSection Section;
    protected ElfFile File = file;

    public abstract string Name { get; }

    /// <summary>
    /// Prepares a new instance of the <see cref="CustomSection"/> class for writing.
    /// </summary>
    public void PrepareForWriting()
    {
        Section = new ElfStreamSection(ElfSectionSpecialType.Data, new MemoryStream())
        {
            Name = Name,
            Flags = ElfSectionFlags.None
        };
        File.Add(Section);
    }
    
    /// <summary>
    /// Prepares a new instance of the <see cref="CustomSection"/> class for reading
    /// </summary>
    public void PrepareForReading()
    {
        Section = (ElfStreamSection)file.Sections.First(_ => _.Name.Value == Name);
    }

    public void Write()
    {
        PrepareForWriting();
        var writer = new BinaryWriter(Section.Stream);
        Write(writer);
    }

    public void Read()
    {
        PrepareForReading();
        var reader = new BinaryReader(Section.Stream);
        Read(reader);
    }

    protected abstract void Write(BinaryWriter writer);
    protected abstract void Read(BinaryReader reader);
}