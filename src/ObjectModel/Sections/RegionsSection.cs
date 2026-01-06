using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class RegionsSection(ElfFile file) : ModelSection<RegionModel>(file)
{
    public override string Name => ".regions";
}