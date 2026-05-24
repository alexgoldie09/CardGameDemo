using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetingOptions
{
    NoTarget,
    AllCreatures, 
    EnemyCreatures,
    YourCreatures, 
    AllCharacters, 
    EnemyCharacters,
    YourCharacters
}

[CreateAssetMenu(fileName = "Card_", menuName = "Cards/CardAsset")]
public class CardAsset : ScriptableObject 
{
    // this object will hold the info about the most general card
    [Header("General info")]
    public CharacterAsset CharacterAsset;  // if this is null, it`s a neutral card
    [TextArea(2,3)]
    public string Description;  // Description for spell or character
	public Sprite CardImage;
    public int ManaCost;
    public bool IsSpell;

    [Header("Creature Info")]
    public int MaxHealth;   // =0 => spell card
    public int Attack;
    public int AttacksForOneTurn = 1;
    public bool Charge;
    public string CreatureScriptName;
    public int SpecialCreatureAmount;

    [Header("Spell Info")]
    public string SpellScriptName;
    public int SpecialSpellAmount;
    public TargetingOptions Targets;

}
