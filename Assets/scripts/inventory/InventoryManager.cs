using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public HeroEntity _entity;
    public HeroLife _entityLife;
    public InventorySlot[] inventorySlots;
    public GameObject InventoryItemPrefab;
    public int maxStackedItems = 5;

    int selectedSlot = -1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ChangeSelectedSlot(18);
    }

    private void Update()
    {
        if (Input.inputString != null)
        {
            if (Input.inputString == "&")
            {
                ChangeSelectedSlot(18);
                ConsumeItem();
            }
            else if (Input.inputString == "é")
            {
                ChangeSelectedSlot(19);
                ConsumeItem();
            }
            else if (Input.inputString == "\"")
            {
                ChangeSelectedSlot(20);
                ConsumeItem();
            }
            else if (Input.inputString == "'")
            {
                ChangeSelectedSlot(21);
                ConsumeItem();
            }
            else if (Input.inputString == "(")
            {
                ChangeSelectedSlot(22);
                ConsumeItem();
            }
            else if (Input.inputString == "-")
            {
                ChangeSelectedSlot(23);
                ConsumeItem();
            }
        }
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && 
                itemInSlot.item == item && 
                itemInSlot.count < maxStackedItems &&
                itemInSlot.item.stackable == true)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }
    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(InventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
    }
    private void ChangeSelectedSlot(int newValue)
    {
        if (selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }
        
        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    private void ConsumeItem()
    {
        InventoryItem itemInSlot = inventorySlots[selectedSlot].GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
        {
            if (itemInSlot.item.type == Item.ItemType.Consumable)
            {
                if (_entityLife.hps == _entityLife.hpsMax && itemInSlot.item.HealingAmount > 0)
                    return;
                _entityLife.Heal(itemInSlot.item.HealingAmount);
                if (itemInSlot.count > 1)
                {
                    itemInSlot.count--;
                    itemInSlot.RefreshCount();
                }
                else
                    Destroy(itemInSlot.gameObject);

            }
            else
            {
                Debug.Log("The Item is an Ingredient");
            }
        }
    }
}
