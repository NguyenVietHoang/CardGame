using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card Data")]
public class CardObject : ScriptableObject
{
    public Sprite Photo;
    public int ATK;
    public int DEF;
    public string Detail;
}
