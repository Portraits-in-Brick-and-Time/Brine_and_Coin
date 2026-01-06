using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class ItemsSection(ElfFile file) : ModelSection<ItemModel>(file)
{
    public override string Name => ".items";
}
