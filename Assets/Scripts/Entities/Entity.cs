using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    protected Vector3 forwards = Vector3.forward;
    [SerializeField]
    protected Vector3 up = new Vector3(0, -1, 0);
    [SerializeField]
    float gravitySharpness = 0.5f;

    private Vector3 gravityDir = new Vector3(0,-1,0);
    private float gravityStrength = 1;
    protected Vector3 left = Vector3.left;

    public UnityAction onUpdate;
    public UnityAction onPhysicsUpdate;

    public Vector3 Gravity
    {
        get
        {
            return -up;
        }
        set
        {
            gravityDir = value.normalized;
            gravityStrength = value.magnitude;
        }
    }

    void Update()
    {
        float blend = 1 - Mathf.Pow(1 - gravitySharpness, Time.deltaTime);
        up = Vector3.Slerp(up, -gravityDir, blend);
        transform.up = up;   
        onUpdate?.Invoke();
    }

    void FixedUpdate()
    {
        onPhysicsUpdate?.Invoke();
    }
}
