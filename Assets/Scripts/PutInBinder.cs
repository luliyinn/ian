using UnityEngine;

public class PutInBinder : MonoBehaviour
{
    [SerializeField] private Transform cardsContainer;
    private Vector3 cardSlot;
    private GameObject currentCardInSlot = null;

    // Allows BinderManager to see what card is in this slot
    public GameObject CurrentCardInSlot => currentCardInSlot;

    private void Start()
    {
        cardSlot = transform.position;
    }

    // Helper method for BinderManager to assign a loaded card into this slot
    public void AssignCardToSlot(GameObject card, TakeCard cardScript)
    {
        currentCardInSlot = card;

        card.SetActive(true);

        // 1. Store the card's original scale before parenting
        Vector3 originalScale = card.transform.localScale;

        // 2. Parent to container via TakeCard
        Transform parentTarget = cardsContainer != null ? cardsContainer : transform;
        cardScript.PlaceInSlot(parentTarget);

        // 3. Snap position & rotation to slot
        card.transform.position = transform.position;
        card.transform.rotation = transform.rotation;

        // 4. Preserve original scale relative to parent scale
        card.transform.localScale = new Vector3(
            originalScale.x / parentTarget.lossyScale.x,
            originalScale.y / parentTarget.lossyScale.y,
            originalScale.z / parentTarget.lossyScale.z
        );

        foreach (var renderer in card.GetComponentsInChildren<Renderer>(true))
        {
            renderer.enabled = true;
        }
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
        GameObject heldObject = GetCameraChildWithTag("Card");

        // PUT CARD IN SLOT
        if (heldObject != null && currentCardInSlot == null)
        {
            if (heldObject.TryGetComponent(out TakeCard cardScript))
            {
                cardScript.PlaceInSlot(cardsContainer);
            }

            heldObject.transform.position = cardSlot;
            heldObject.transform.rotation = transform.rotation;

            currentCardInSlot = heldObject;
            return;
        }

        // TAKE CARD OUT OF SLOT
        if (heldObject == null && currentCardInSlot != null)
        {
            if (currentCardInSlot.TryGetComponent(out TakeCard cardScript))
            {
                cardScript.PickUp();
            }

            currentCardInSlot = null;
        }
    }
}