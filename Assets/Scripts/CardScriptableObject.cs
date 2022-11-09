using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Card", menuName = "Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    public string cardName = "";

    [TextArea]
    public string actionDescription = "";
    [TextArea]
    public string cardLore = "";

    public int currentHealth, attackPower, manaCost = 0;

    public Sprite characterSprite, bgSprite;

    

}
