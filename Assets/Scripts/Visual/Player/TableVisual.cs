using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class TableVisual : MonoBehaviour
{
    [Header("Settings")]
    public AreaPosition owner;
    public SameDistanceChildren slots;

    private List<GameObject> creaturesOnTable = new List<GameObject>();

    private static TableVisual[] allTables;

    #region Accessors
    /// <summary>
    /// Returns true if the cursor is currently over any player's table.
    /// Cached on first access to avoid repeated FindObjectsByType calls.
    /// </summary>
    public static bool CursorOverSomeTable
    {
        get
        {
            if (allTables == null)
                allTables = FindObjectsByType<TableVisual>(FindObjectsSortMode.None);
            return allTables[0].CursorOverThisTable || allTables[1].CursorOverThisTable;
        }
    }

    /// <summary>
    /// Set by CardInputManager each frame based on raycast results.
    /// </summary>
    public bool CursorOverThisTable { get; set; }
    #endregion

    private void Awake()
    {
        allTables = null; // reset cache on scene load
    }

    /// <summary>
    /// Creates a new creature at the given index and adds it to the table.
    /// </summary>
    public void AddCreatureAtIndex(CardAsset ca, int uniqueID, int index)
    {
        GameObject creature = Instantiate(
            GlobalSettings.Instance.CreaturePrefab,
            slots.Children[index].transform.position,
            Quaternion.identity
        );

        // Creature prefab uses OneCreatureManager, not CardInfoManager
        CreatureInfoManager manager = creature.GetComponent<CreatureInfoManager>();
        manager.LoadCreature(ca);

        foreach (Transform t in creature.GetComponentsInChildren<Transform>())
            t.tag = $"{owner}Creature";

        creature.transform.SetParent(slots.transform);
        creaturesOnTable.Insert(index, creature);

        WhereIsTheCardOrCreature w = creature.GetComponent<WhereIsTheCardOrCreature>();
        w.Slot = index;
        w.VisualState = owner == AreaPosition.Low ? VisualStates.LowTable : VisualStates.TopTable;

        creature.AddComponent<IDHolder>().UniqueID = uniqueID;

        ShiftSlotsGameObjectAccordingToNumberOfCreatures();
        PlaceCreaturesOnNewSlots();

        Command.CommandExecutionComplete();
    }

    /// <summary>
    /// Returns the table index a new creature should be inserted at based on mouse X position.
    /// </summary>
    public int TablePosForNewCreature(float mouseX)
    {
        if (creaturesOnTable.Count == 0 || mouseX > slots.Children[0].transform.position.x)
            return 0;

        if (mouseX < slots.Children[creaturesOnTable.Count - 1].transform.position.x)
            return creaturesOnTable.Count;

        for (int i = 0; i < creaturesOnTable.Count; i++)
        {
            if (mouseX < slots.Children[i].transform.position.x &&
                mouseX > slots.Children[i + 1].transform.position.x)
                return i + 1;
        }

        Debug.Log("Suspicious behavior. Reached end of TablePosForNewCreature. Returning 0.");
        return 0;
    }

    /// <summary>
    /// Removes the creature with the given ID from the table and updates slot positions.
    /// </summary>
    public void RemoveCreatureWithID(int idToRemove)
    {
        GameObject creatureToRemove = IDHolder.GetGameObjectWithID(idToRemove);
        creaturesOnTable.Remove(creatureToRemove);
        Destroy(creatureToRemove);

        ShiftSlotsGameObjectAccordingToNumberOfCreatures();
        PlaceCreaturesOnNewSlots();
        Command.CommandExecutionComplete();
    }

    #region Helper Methods
    /// <summary>
    /// Shifts the slots container to keep creatures centred on the table.
    /// </summary>
    private void ShiftSlotsGameObjectAccordingToNumberOfCreatures()
    {
        float posX = creaturesOnTable.Count > 0
            ? (slots.Children[0].transform.localPosition.x - slots.Children[creaturesOnTable.Count - 1].transform.localPosition.x) / 2f
            : 0f;

        slots.gameObject.transform.DOLocalMoveX(posX, 0.3f);
    }

    /// <summary>
    /// Moves all creatures to their correct slot positions after a change in table state.
    /// </summary>
    private void PlaceCreaturesOnNewSlots()
    {
        foreach (GameObject g in creaturesOnTable)
            g.transform.DOLocalMoveX(slots.Children[creaturesOnTable.IndexOf(g)].transform.localPosition.x, 0.3f);
    }
    #endregion
}