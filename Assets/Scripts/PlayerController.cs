using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float mouseSensitivity = 1;
    Camera playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxisRaw("Mouse Y");
        float mouseX = Input.GetAxisRaw("Mouse X");
        Vector3 camEuler = playerCamera.transform.eulerAngles;

        mouseY *= Time.deltaTime * mouseSensitivity;
        mouseX *= Time.deltaTime * mouseSensitivity;
        //Debug.Log(euler.x);
        camEuler.x = Mathf.Clamp(fixDegree(camEuler.x - mouseY), -87, 87);
        camEuler.y += mouseX;

        playerCamera.transform.eulerAngles = camEuler;
    }

    static private float fixDegree(float degree)
    {
        return ((degree + 180) % 360) - 180;
    }

    void OnValidate()
    {
        Camera playCam = GetComponentInChildren<Camera>();
        if (playCam == null)
        {
            GameObject child = new GameObject("Player Camera");
            child.AddComponent<Camera>();
            child.transform.SetParent(transform);
        }
    }
}
