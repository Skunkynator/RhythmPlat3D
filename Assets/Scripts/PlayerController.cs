using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float mouseSensitivity = 1;
    [SerializeField]
    float speed = 10;
    [SerializeField]
    float gravityStrength = 9.89f;

    Camera playerCamera;
    Rigidbody playerRigid;
    Vector3 forwards = Vector3.forward;
    Vector3 up = Vector3.up;
    Vector3 left = Vector3.left;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = GetComponentInChildren<Camera>();
        playerRigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxisRaw("Mouse Y");
        float mouseX = Input.GetAxisRaw("Mouse X");
        Vector3 camEuler = playerCamera.transform.eulerAngles;
        Vector3 dirControlls = Vector3.zero;

        mouseY *= Time.deltaTime * mouseSensitivity;
        mouseX *= Time.deltaTime * mouseSensitivity;
        //Debug.Log(euler.x);
        camEuler.x = Mathf.Clamp(fixDegree(camEuler.x - mouseY), -87, 87);
        camEuler.y += mouseX;
        forwards.x = Mathf.Sin(camEuler.y * Mathf.Deg2Rad);
        forwards.z = Mathf.Cos(camEuler.y * Mathf.Deg2Rad);
        left = Vector3.Cross(forwards, up);

        dirControlls += Input.GetAxis("Vertical")   * forwards;
        dirControlls -= Input.GetAxis("Horizontal") * left;
        dirControlls = dirControlls.normalized * Mathf.Clamp01(dirControlls.magnitude);
        dirControlls *= Time.deltaTime * speed;
        playerRigid.MovePosition(playerRigid.position + dirControlls);

        
        playerCamera.transform.eulerAngles = camEuler;
    }

    void FixedUpdate()
    {
        playerRigid.AddForce(-up * gravityStrength,ForceMode.Acceleration);
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
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }
}
