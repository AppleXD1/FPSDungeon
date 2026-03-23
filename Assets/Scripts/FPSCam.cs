using Unity.VisualScripting;
using UnityEngine;

public class FPSCam : MonoBehaviour
{
    public float sensitivity = 2f;
    private float xRotation = 0f;
    public bool camLock;
    void Start()
    {
        camLock = true;
       
    }

    void Update()
    {
        CamLock();
        if (camLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }


    void CamLock()
    {
        if (Input.GetKeyDown(KeyCode.R) && camLock)
        {
            camLock = false;
        }
        else if (Input.GetKeyDown(KeyCode.R) && !camLock)
        {
            camLock = true;
        }
    }
}
