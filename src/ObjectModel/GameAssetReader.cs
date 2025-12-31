namespace ObjectModel;

using System;
using System.IO;
using System.Linq;
using LibObjectFile.Elf;
using MessagePack;
using NetAF.Assets;
using ObjectModel.Models;

public class GameAssetReader
{
    private ElfStreamSection _objectsSection;
    private ElfFile _file;
    private Stream _strm;
    private readonly ElfSymbolTable _symTable;
    private readonly CustomSections _customSections;

    public GameAssetReader(Stream strm)
    {
        _file = ElfFile.Read(strm);
        _strm = strm;

        _symTable = (ElfSymbolTable)_file.Sections.First(_ => _ is ElfSymbolTable);
        _objectsSection = (ElfStreamSection)_file.Sections.First(_ => _.Name.Value == ".objects");

        _customSections = new(_file);
        _customSections.Read();
    }

    public IExaminable ReadObject()
    {
        if (_strm!.Position == _strm.Length)
        {
            return null;
        }

        var obj = MessagePackSerializer.Deserialize<GameObject>(_objectsSection!.Stream);

        var instance = obj.Instanciate();
        obj.InstanciateAttributesTo(instance, _customSections.AttributesSection);
        
        return instance;
    }
}
