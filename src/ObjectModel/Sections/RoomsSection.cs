using System.Collections.Generic;
using System.IO;
using LibObjectFile.Elf;
using MessagePack;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class RoomsSection(ElfFile file) : ModelSection<RoomModel>(file)
{
    public override string Name => ".rooms";
}
