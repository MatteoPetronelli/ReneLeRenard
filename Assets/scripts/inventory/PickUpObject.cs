using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpObject : MonoBehaviour
{
    public Item item;

    void TakeItem()
    {
        bool result = InventoryManager.instance.AddItem(item);
        if (!result) return;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            TakeItem();
        }
    }
}
