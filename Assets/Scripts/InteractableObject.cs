using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractableObject : MonoBehaviour
{
    public ShowISpyMessage showISpyMessage;
    public bool playerInRange;
    public string ItemName;

    public string GetItemName()
    {
        return ItemName;
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.Instance.onTarget)
        {

            if (!InventorySystem.Instance.CheckIfFull())
            {
                // Debug.Log("item added to inventory");
                InventorySystem.Instance.AddToInventory(ItemName);
                if (CollectManager.Instance != null) CollectManager.Instance.OnItemCollected();
                Destroy(gameObject);
            }
            else
            {

                Debug.Log("inventory is full");
            }

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}