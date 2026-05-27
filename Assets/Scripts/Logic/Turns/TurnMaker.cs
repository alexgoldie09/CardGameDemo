using UnityEngine;
using System.Collections;

public abstract class TurnMaker : MonoBehaviour 
{
    protected Player p;

    /// <summary>
    /// Initializes the TurnMaker by getting the Player component attached to the same GameObject.
    /// </summary>
    private void Awake()
    {
        p = GetComponent<Player>();
    }

    /// <summary>
    /// Called at the start of the turn.
    /// </summary>
    public virtual void OnTurnStart()
    {
        // add one mana crystal to the pool;
        p.OnTurnStart();
    }
}