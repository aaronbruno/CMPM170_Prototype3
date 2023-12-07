using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform orientation;

    public float sensX;
    public float sensY;

    float rotationX;
    float rotationY;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        rotationY += mouseX;
        rotationX += mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.rotation = Quaternion.Euler(0, rotationY, 0);
    }
}
