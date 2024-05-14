using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public Sprite image;
    public float HealingAmount;
    public float BoostJumpAmount;
    public ItemType type;
    public bool stackable = true;

    public enum ItemType
    {
        Ingredient,
        Consumable
    }
}
