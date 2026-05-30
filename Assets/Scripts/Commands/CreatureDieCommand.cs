using UnityEngine;
using System.Collections;

public class CreatureDieCommand : Command 
{
    private Player p;
    private int DeadCreatureID;

    public CreatureDieCommand(int CreatureID, Player p)
    {
        this.p = p;
        this.DeadCreatureID = CreatureID;
    }

    public override void StartCommandExecution()
    {
        Debug.Log($"[CreatureDieCommand] Removing creature ID:{DeadCreatureID}");
        p.PArea.TableVisual.RemoveCreatureWithID(DeadCreatureID);
    }
}
