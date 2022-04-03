using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform centralPoint;

    public Transform targetObiect;


    public float cameraRotatingSpeed = 1;

    public float cameraSmoothMove = 0.125f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        transform.localEulerAngles = new Vector3(20, 0, 0);
    }

    void Update()
    {
        Vector3 cameraDrag= new Vector3(0, Input.GetAxis("Mouse X"), 0);
        //Vector3 cameraDrag2 = new Vector3(-Input.GetAxis("Mouse Y"), 0, 0);

        centralPoint.position = Vector3.Lerp(centralPoint.position, targetObiect.position, cameraSmoothMove * Time.deltaTime);

        centralPoint.Rotate(cameraDrag * cameraRotatingSpeed * Time.deltaTime);
        //transform.Rotate(cameraDrag2 * cameraRotatingSpeed * Time.deltaTime);
    }
}
