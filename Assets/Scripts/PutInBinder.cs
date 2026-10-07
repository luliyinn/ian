using UnityEngine;

public class PutInBinder : MonoBehaviour
{
    private Vector3 cardSlot;
    private GameObject currentCardInSlot = null;

    private void Start()
    {
        cardSlot = transform.position;
    }

    public GameObject GetCameraChildWithTag(string tag)
    {
        Transform camTransform = Camera.main.transform;

        foreach (Transform child in camTransform)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private void OnMouseDown()
    {
        // 1. Check if player is holding a card on the camera
        GameObject heldObject = GetCameraChildWithTag("Card");

        // SCENARIO A: Holding a card AND slot is empty -> PUT CARD IN SLOT
        if (heldObject != null && currentCardInSlot == null)
        {
            if (heldObject.TryGetComponent(out TakeCard cardScript))
            {
                cardScript.PlaceInSlot();
            }

            heldObject.transform.position = cardSlot;
            heldObject.transform.rotation = transform.rotation;

            currentCardInSlot = heldObject; // Remember this card is in the slot
            return;
        }

        // SCENARIO B: Not holding a card AND slot has a card -> TAKE CARD OUT OF SLOT
        if (heldObject == null && currentCardInSlot != null)
        {
            // Trigger TakeCard script to pick it up
            if (currentCardInSlot.TryGetComponent(out TakeCard cardScript))
            {
                // Call OnMouseDown or a manual pick-up method on TakeCard
                cardScript.SendMessage("OnMouseDown");
            }

            currentCardInSlot = null; // Clear the slot
        }
    }
}