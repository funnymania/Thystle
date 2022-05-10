using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Commands are executed and also recorded here.
/// </summary>
public class CommandCommander
{
    public long nextId;

    private List<ICommand> activeCommands;
    private List<long> commandsToRemove;

    public CommandCommander()
    {
        activeCommands = new List<ICommand>();
        commandsToRemove = new List<long>();
        nextId = 0;
    }

    // This is called as a subscription everytime the FixedUpdate in ReplayPlayer or a real match is called.
    // Calling is done at the end of the loop.
    public void Execution()
    {
        for (var k = 0; k < activeCommands.Count; k += 1)
        {
            if (activeCommands[k].Execute())
            {
                commandsToRemove.Add(activeCommands[k].id);
            }

            activeCommands[k].Record();
        }

        // Both lists are sorted by default. o(n).
        // we could search by ID, which is less performant @ o(n^2)
        int account = 0;
        var i = 0;
        var j = 0;
        while (j < commandsToRemove.Count)
        {
            if (activeCommands[i - account].id == commandsToRemove[j])
            {
                activeCommands.RemoveAt(i - account);
                account += 1;
                j += 1;
            }

            i += 1;
        }

        commandsToRemove.Clear();
    }

    public void AddCommand(ICommand command)
    {
        activeCommands.Add(command);
    }

    public void ClearCommands()
    {
        activeCommands.Clear();
        commandsToRemove.Clear();
    }
}
