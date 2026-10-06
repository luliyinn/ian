using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float sensitivy;
    [SerializeField] private float verticalLimit;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform body;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private float currentX;
    private float currentY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        xRotation = angles.x;
        yRotation = angles.y;
        currentX = xRotation;
        currentY = yRotation;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivy;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivy;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalLimit, verticalLimit);

        currentX = Mathf.Lerp(currentX, xRotation, smoothSpeed * Time.deltaTime);
        currentY = Mathf.Lerp(currentY, yRotation, smoothSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentX, currentY, 0);

        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0, currentY, 0);

        if (body != null)
            body.rotation = Quaternion.Euler(0, currentY, 0);
    }
}