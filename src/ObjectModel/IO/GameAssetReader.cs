namespace ObjectModel.IO;

using System.IO;
using System.Linq;
using LibObjectFile.Elf;

internal class GameAssetReader
{
    private ElfFile _file;

    public readonly ElfSymbolTable SymbolTable;
    public readonly CustomSections CustomSections;

    public ElfFile File { get => _file; }

    public GameAssetReader(Stream strm)
    {
        _file = ElfFile.Read(strm);

        SymbolTable = (ElfSymbolTable)File.Sections.First(_ => _ is ElfSymbolTable);

        CustomSections = new(File);
        CustomSections.Read();
    }
}
