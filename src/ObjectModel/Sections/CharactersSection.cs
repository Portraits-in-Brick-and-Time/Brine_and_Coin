using LibObjectFile.Elf;
using ObjectModel.Models;

namespace ObjectModel.Sections;

internal class CharactersSection(ElfFile file) : ModelSection<CharacterModel>(file)
{
    public override string Name => ".characters";
}