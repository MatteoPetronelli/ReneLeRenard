using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpObject : MonoBehaviour
{
    private Text interactUI;
    private bool isInRange;

    public Item item;

    private void Awake()
    {
        interactUI = GameObject.FindGameObjectWithTag("InteractUI").GetComponent<Text>();
        interactUI.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isInRange)
        {
            TakeItem();
        }
    }

    void TakeItem()
    {
        bool result = InventoryManager.instance.AddItem(item);
        if (!result) return;
        interactUI.enabled = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            interactUI.enabled = true;
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactUI.enabled = false;
            isInRange = false;
        }
    }
}
