using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    /// <summary>
    /// The cards currently in the player's hand.
    /// This list can be modified by adding or removing CardLogic instances as the game progresses.
    /// </summary>
    public List<CardLogic> CardsInHand { get; } = new List<CardLogic>();
}
