using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    /// <summary>
    /// List of creatures currently on the table.
    /// The index of the creature in the list corresponds to its position on the table.
    /// </summary>
    public List<CreatureLogic> CreaturesOnTable { get; } = new List<CreatureLogic>();

    /// <summary>
    /// Places a creature at the specified index on the table.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="creature"></param>
    public void PlaceCreature(int index, CreatureLogic creature)
    {
        CreaturesOnTable.Insert(index, creature);
    }
}
