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
    private float gravityFieldStrength = 1;
    protected Vector3 left = Vector3.left;
    private List<GravityArea> currentGravs = new List<GravityArea>();

    public UnityAction onUpdate;
    public UnityAction onPhysicsUpdate;
    
    private Vector3 quatForwards = Vector3.forward;

    public Vector3 Gravity
    {
        get
        {
            return -up;
        }
        set
        {
            gravityDir = value.normalized;
            gravityFieldStrength = value.magnitude;
        }
    }

    void Update()
    {
        UpdateGravity();
        float blend = 1 - Mathf.Pow(1 - gravitySharpness, Time.deltaTime);
        up = Vector3.Slerp(up, -gravityDir, blend).normalized;
        quatForwards = -Vector3.Cross(up,quatForwards).normalized;
        quatForwards =  Vector3.Cross(up,quatForwards).normalized;
        transform.rotation = Quaternion.LookRotation(quatForwards, up);
        onUpdate?.Invoke();
    }

    void FixedUpdate()
    {
        onPhysicsUpdate?.Invoke();
    }
    public void addGravityArea(GravityArea area)
    {
        currentGravs.Add(area);
    }
    public void removeGravityArea(GravityArea area)
    {
        currentGravs.Remove(area);
    }
    void UpdateGravity()
    {
        if(currentGravs.Count == 0)
            return;
        Vector3 curr = Vector3.zero;
        foreach (GravityArea gravityArea in currentGravs)
        {
            curr += gravityArea.getGravity(transform.position); 
        }
        this.Gravity = curr.normalized;
    }
}
