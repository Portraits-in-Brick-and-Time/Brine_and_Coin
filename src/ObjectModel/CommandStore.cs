using System.Collections.Generic;
using NetAF.Commands;

namespace ObjectModel;

public static class CommandStore
{
    private static Dictionary<string, ICommand> _commands = [];

    public static void Add(string name, ICommand command)
    {
        _commands.Add(name, command);
    }

    public static bool TryGet(string name, out ICommand command)
    {
        return _commands.TryGetValue(name, out command);
    }
}