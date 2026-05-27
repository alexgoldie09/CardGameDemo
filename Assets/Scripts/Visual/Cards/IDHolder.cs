using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IDHolder : MonoBehaviour 
{
    [SerializeField, Tooltip("Unique ID for this GameObject.")]
    private int uniqueID;
    
    /// <summary>
    /// List of all IDHolders in the scene. Used to find the GameObject with a specific ID.
    /// </summary>
    private static List<IDHolder> allIDHolders = new List<IDHolder>();

    /// <summary>
    /// Adds this IDHolder to the list of all IDHolders in the scene.
    /// This allows us to find the GameObject with a specific ID later.
    /// </summary>
    private void Awake()
    {
        allIDHolders.Add(this);   
    }

    /// <summary>
    /// Finds the GameObject with the specified ID by searching through all IDHolders in the scene.
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static GameObject GetGameObjectWithID(int ID)
    {
        foreach (IDHolder i in allIDHolders)
        {
            if (i.uniqueID == ID)
                return i.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Clears the list of all IDHolders in the scene.
    /// This should be called when changing scenes to avoid keeping references to destroyed GameObjects.
    /// </summary>
    public static void ClearIDHoldersList()
    {
        allIDHolders.Clear();
    }
    
    public int UniqueID
    {
        get => uniqueID;
        set => uniqueID = value;
    }
}