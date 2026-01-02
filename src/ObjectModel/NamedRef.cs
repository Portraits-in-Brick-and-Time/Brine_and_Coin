using MessagePack;

namespace ObjectModel;

[MessagePackObject]
public record NamedRef([property: Key(0)] string Name);