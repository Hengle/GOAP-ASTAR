using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sadfasddf : MonoBehaviour {
    public Vector3 movePos;
    public Vector3 rotatePos;
    public float multiplier;
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Update()
    {
        movePos.x += Input.GetAxis("Horizontal");
        movePos.z += Input.GetAxis("Vertical");
        rotatePos.x -= Input.GetAxis("Mouse Y");
        rotatePos.y += Input.GetAxis("Mouse X");
        transform.Translate(movePos * multiplier * Time.deltaTime);
        transform.Rotate(rotatePos * multiplier * Time.deltaTime);
    }
}
