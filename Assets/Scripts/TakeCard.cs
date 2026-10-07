using UnityEngine;

public class TakeCard : MonoBehaviour
{
    [Header("Screen Settings")]
    [SerializeField] private Vector3 screenOffset = new Vector3(0.4f, -0.2f, 1f);
    [SerializeField] private Vector3 holdRotation = new Vector3(0, 0, 0);
    [SerializeField] private float smoothSpeed = 10f;

    private Transform mainCameraTransform;
    private bool isBeingHeld = false;
    private Collider objCollider;
    private Rigidbody rb;

    private void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        TryGetComponent(out objCollider);
        TryGetComponent(out rb);
    }

    private void OnMouseDown()
    {
        // Toggle holding state when clicked
        isBeingHeld = !isBeingHeld;

        if (isBeingHeld)
        {
            PickUp();
        }
        else
        {
            Drop();
        }
    }

    public void PickUp()
    {
        isBeingHeld = true;

        // 1. Parent to camera
        if (mainCameraTransform != null)
        {
            transform.SetParent(mainCameraTransform);
        }

        // 2. Disable physics and collision so it smoothly tracks screen space
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        if (objCollider != null)
        {
            objCollider.enabled = false;
        }
    }

    private void Drop()
    {
        transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
        }
        if (objCollider != null)
        {
            objCollider.enabled = true;
        }
    }

    public void PlaceInSlot(Transform newParent)
    {
        isBeingHeld = false; // Stops Update() from pulling the card to screen[cite: 3]

        // Parent the card directly to whichever container transform was passed in!
        transform.SetParent(newParent);

        if (rb != null)
        {
            rb.isKinematic = true; // Stop physics movement[cite: 3]
        }

        if (objCollider != null)
        {
            objCollider.enabled = false; // Disable collider so it doesn't fight physics![cite: 3]
        }
    }

    private void Update()
    {
        if (isBeingHeld)
        {
            Vector3 targetPosition = mainCameraTransform.TransformPoint(screenOffset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

            Quaternion targetRotation = mainCameraTransform.rotation * Quaternion.Euler(holdRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }
}