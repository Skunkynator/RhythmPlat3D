using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPlat.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        float mouseSensitivity = 1;
        [SerializeField]
        float speed = 10;
        [SerializeField]
        float gravityStrength = 9.89f;
        [SerializeField]
        float jumpStrength = 10;
        [SerializeField]
        int jumpRefillAmount = 1;
        [SerializeField]
        float jumpRefillAngle = 0.75f;

        Camera playerCamera;
        Rigidbody playerRigid;

        PlayerState state = PlayerState.Grounded;

        Vector3 forwards = Vector3.forward;
        Vector3 up = Vector3.up;
        Vector3 left = Vector3.left;

        int jumpCount = 1;
        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerCamera = GetComponentInChildren<Camera>();
            playerRigid = GetComponent<Rigidbody>();

            /*TESTING*/
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
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
            forwards.x = Mathf.Sin(camEuler.y * Mathf.Deg2Rad);
            forwards.z = Mathf.Cos(camEuler.y * Mathf.Deg2Rad);
            left = Vector3.Cross(forwards, up);




            playerCamera.transform.eulerAngles = camEuler;
        }

        void FixedUpdate()
        {
            Vector3 dirControlls = Vector3.zero;

            dirControlls += Input.GetAxis("Vertical") * forwards;
            dirControlls -= Input.GetAxis("Horizontal") * left;
            dirControlls = dirControlls.normalized * Mathf.Clamp01(dirControlls.magnitude);

            dirControlls *= Time.deltaTime * speed;
            playerRigid.MovePosition(playerRigid.position + dirControlls);

            playerRigid.AddForce(-up * gravityStrength, ForceMode.Acceleration);
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
            {
                playerRigid.velocity = Vector3.zero;
                playerRigid.AddForce(up * jumpStrength, ForceMode.VelocityChange);
                jumpCount--;
                state = PlayerState.Jumping;
            }
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
            if (state != PlayerState.Grounded)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    checkGrounded(contact);
                }
                if (state == PlayerState.Grounded)
                {
                    playerRigid.velocity = Vector3.zero;
                }
            }

        }

        void OnCollisionStay(Collision collision)
        {
            if (state != PlayerState.Grounded)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    checkGrounded(contact);
                }
            }
        }

        void OnCollisionExit(Collision collision)
        {
            if (jumpCount == jumpRefillAmount)
                state = PlayerState.Falling;
        }

        void checkGrounded(ContactPoint contact)
        {
            Debug.Log(Vector3.Dot(contact.normal, up));
            if (Vector3.Dot(contact.normal, up) >= jumpRefillAngle)
            {
                state = PlayerState.Grounded;
                jumpCount = Mathf.Max(jumpCount, jumpRefillAmount);
            }
        }
    }
}