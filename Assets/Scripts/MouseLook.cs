using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    //Set the sensitivity at which the mouse moves the player
    public float mouseSensitivity = 100f;
    //Set the Root object of the player
    public Transform playerBody;
    //Float for x axis rotation
    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        //Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {
        //Get X axis Mouse input multiplying by sensitivity and Time to make it not framerate dependant
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        //Get Y axis Mouse Input multiplying by sensitivity and Time to make it not framerate dependant
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Substract mouseY from xRotation. If added the up and down controlls will be inverted
        xRotation -= mouseY;
        //Clamping of the Up and down movement to prevent over rotation. 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Set xRotation to the X axis of the contained gameObject while setting other axis to 0.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //Rotation of root ubject in the Y axis using the mouse input x axis
        playerBody.Rotate(Vector3.up * mouseX);
    }

}
