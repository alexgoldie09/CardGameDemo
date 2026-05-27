using UnityEngine;
using System.Collections;

public class StartATurnCommand : Command 
{

    private Player p;
    
    /// <summary>
    /// This command is used to start a turn for a player.
    /// </summary>
    /// <param name="p"></param>
    public StartATurnCommand(Player p)
    {
        this.p = p;
    }

    /// <summary>
    /// This method is called to execute the command.
    /// It will set the TurnManager's WhoseTurn property to the player passed in the constructor, which will trigger the necessary events and UI updates for the start of a turn.
    /// </summary>
    public override void StartCommandExecution()
    {
        TurnManager.Instance.WhoseTurn = p;
        // this command is completed instantly
        CommandExecutionComplete();
    }
}
