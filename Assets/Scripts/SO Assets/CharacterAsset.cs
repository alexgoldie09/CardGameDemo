using UnityEngine;
using System.Collections;

public enum CharClass
{
	Elf, 
	Monk, 
	Warrior
}

[CreateAssetMenu(fileName = "Character_", menuName = "Characters/CharacterAsset")]
public class CharacterAsset : ScriptableObject 
{
	[Header("Character Properties")]
	public CharClass Class;
	public string ClassName;
	public int MaxHealth = 30;
	
	[Header("Character Power Properties")]
	public string HeroPowerName;
	public Sprite HeroPowerIconImage;
	public Sprite HeroPowerBGImage;
	public Color HeroPowerBGTint;
	
	[Header("Character Avatar Properties")]
	public Sprite AvatarImage;
    public Sprite AvatarBGImage;
    public Color AvatarBGTint;
    
    [Header("Character Card Properties")]
    public Color ClassCardTint;
    public Color ClassRibbonsTint;
}
