using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPlat.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : Entity
    {
        [SerializeField]
        float mouseSensitivity = 1;
        [SerializeField]
        float speed = 10;
        [SerializeField]
        float accelaration = 10;
        [SerializeField]
        float gravityStrength = 9.89f;
        [SerializeField]
        float jumpStrength = 10;
        [SerializeField]
        int jumpRefillAmount = 1;
        [SerializeField]
        float jumpRefillAngle = 0.75f;
        [SerializeField]
        float minWallSpeed = 0.5f;
        [SerializeField]
        float speedSmoothnes = 20;
        [SerializeField]
        float noWallJumpControllTime = 0.75f;

        Camera playerCamera;
        Rigidbody playerRigid;

        PlayerState state = PlayerState.Grounded;
        Vector3 wallNormal = Vector3.left;
        Vector3 controlVelocity = Vector3.zero;
        Vector3 horizontalVel, horizontalModVel;
        float noControlTime = 0;

        float speedLimiter1;
        float speedLimiter2;

        int jumpCount = 1;

        void Start()
        {
            speedLimiter1 = speed / speedSmoothnes + 1 - 1 / (speed / speedSmoothnes + 2);
            speedLimiter2 = 1 - 1 / (speedLimiter1 + 1);
            speedLimiter1 = 1 / speedLimiter1;
            playerCamera = GetComponentInChildren<Camera>();
            playerRigid = GetComponent<Rigidbody>();
            onUpdate += handleMouseInput;
            onPhysicsUpdate += handleMovementInput;
        }

        // Update is called once per frame
        void handleMouseInput()
        {
            float mouseY = Input.GetAxisRaw("Mouse Y");
            float mouseX = Input.GetAxisRaw("Mouse X");
            Vector3 camEuler = playerCamera.transform.localEulerAngles;

            mouseY *= Time.deltaTime * mouseSensitivity;
            mouseX *= Time.deltaTime * mouseSensitivity;
            //Debug.Log(euler.x);
            camEuler.x = Mathf.Clamp(fixDegree(camEuler.x - mouseY), -87, 87);
            camEuler.y += mouseX;
            forwards.x = Mathf.Sin(camEuler.y * Mathf.Deg2Rad);
            forwards.z = Mathf.Cos(camEuler.y * Mathf.Deg2Rad);
            forwards = playerCamera.transform.rotation * Vector3.forward;
            left = Vector3.Cross(forwards, up).normalized;
            forwards = Vector3.Cross(up, left).normalized;

            playerCamera.transform.localEulerAngles = camEuler;
        }

        private void handleMovementInput()
        {
            /*Vector3 hVelocity = Vector3.ProjectOnPlane(playerRigid.velocity, up);
            if(hVelocity.magnitude < controlVelocity.magnitude && Vector3.Dot(hVelocity.normalized, -controlVelocity.normalized) < -0.5f)
            {
                hVelocity = controlVelocity;
            }*/

            transform.up = up;
            playerRigid.velocity -= Vector3.ProjectOnPlane(playerRigid.velocity, up);
            Vector3 dirControlls = Vector3.zero;
            Vector3 direction = Vector3.zero;

            dirControlls += Input.GetAxis("Vertical") * forwards;
            dirControlls -= Input.GetAxis("Horizontal") * left;
            direction += Input.GetAxisRaw("Vertical") * forwards;
            direction -= Input.GetAxisRaw("Horizontal") * left;
            direction = direction.normalized * Mathf.Clamp01(direction.magnitude);
            dirControlls = fixDir(dirControlls) * Mathf.Clamp01(dirControlls.magnitude) * speed;

            if (state != PlayerState.WallSlide)
            {
                playerRigid.AddForce(-up * gravityStrength, ForceMode.Acceleration);
            }
            else if (dirControlls.magnitude < minWallSpeed)
            {
                playerRigid.AddForce(-up * gravityStrength * 0.25f, ForceMode.Acceleration);
            }
            //playerRigid.MovePosition(playerRigid.position + dirControlls);
            //playerRigid.AddForce(dirControlls, ForceMode.VelocityChange);
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
            {
                playerRigid.velocity = Vector3.zero;
                playerRigid.AddForce(up * jumpStrength, ForceMode.VelocityChange);
                if (state == PlayerState.WallSlide)
                {
                    float wallModifier = 3;
                    bool strongJump = Mathf.Abs(Vector3.Dot(wallNormal, direction.normalized)) > 0.5f &&
                        direction.magnitude > 0.2f;
                    //playerRigid.AddForce(wallNormal * jumpStrength/wallModifier, ForceMode.VelocityChange);
                    horizontalVel = wallNormal * jumpStrength / (strongJump ? 1 : wallModifier);
                    horizontalModVel = Vector3.zero;
                    noControlTime = noWallJumpControllTime * Mathf.Sqrt(strongJump ? wallModifier : 1);
                }
                jumpCount--;
                state = PlayerState.Jumping;
            }
            //horizontalVel = Vector3.SmoothDamp(horizontalVel, Vector3.zero, ref horizontalModVel, 0.5f, float.MaxValue, Time.fixedDeltaTime);
            Vector3 velChange = (1 - 1 / (horizontalVel.magnitude / speedSmoothnes + speedLimiter2) + speedLimiter1) * -horizontalVel.normalized;
            noControlTime -= Time.fixedDeltaTime;
            if (noControlTime > 0)
            {
                dirControlls *= 0.3f;
                direction *= 0.3f;
            }
            horizontalVel = (velChange * speed + dirControlls) * Time.fixedDeltaTime * accelaration * (direction.magnitude * 0.8f + 0.2f) + horizontalVel;
            horizontalVel = Vector3.ProjectOnPlane(horizontalVel, up);
            playerRigid.velocity += horizontalVel;
            //controlVelocity = dirControlls;
        }

        static private float fixDegree(float degree)
        {
            return ((degree + 180) % 360) - 180;
        }

        private Vector3 fixDir(Vector3 dir)
        {
            dir = dir.normalized;
            if (state == PlayerState.WallSlide)
            {
                dir = Vector3.Cross(dir, wallNormal);
                dir = -Vector3.Cross(dir, wallNormal);
            }
            return dir;
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
            PlayerState previous = state;
            foreach (ContactPoint contact in collision.contacts)
            {
                if (previous != PlayerState.Grounded)
                {
                    checkGrounded(contact);
                    if (previous != PlayerState.WallSlide)
                    {
                        checkWallrunning(contact);
                    }
                }
            }
            if (state == PlayerState.Grounded)
            {
                playerRigid.velocity = Vector3.zero;
            }
            horizontalVel = Vector3.zero;
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
            if (Vector3.Dot(contact.normal, up) >= jumpRefillAngle)
            {
                state = PlayerState.Grounded;
                jumpCount = Mathf.Max(jumpCount, jumpRefillAmount);
            }
        }
        void checkWallrunning(ContactPoint contact)
        {
            if (Mathf.Abs(Vector3.Dot(contact.normal, up)) <= 0.1f)
            {
                state = PlayerState.WallSlide;
                jumpCount = Mathf.Max(jumpCount, jumpRefillAmount);
                wallNormal = contact.normal;
                playerRigid.velocity = Vector3.zero;
            }
        }
    }
}
