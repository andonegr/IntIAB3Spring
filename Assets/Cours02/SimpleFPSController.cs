using UnityEngine;

public class SimpleFPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    private float rotationX = 0f;
    private bool mouseLookActive = false;

    void Start()
    {
        UnlockMouse();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            mouseLookActive = !mouseLookActive;

            if (mouseLookActive)
                LockMouse();
            else
                UnlockMouse();
        }

        // Movement (arrows / WASD)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        transform.position += move * moveSpeed * Time.deltaTime;

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        // Horizontal rotation on the Player
        transform.Rotate(Vector3.up * mouseX);

        // Vertical rotation on the Camera child
        Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}