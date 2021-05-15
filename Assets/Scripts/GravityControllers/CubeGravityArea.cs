using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGravityArea : GravityArea
{
    [SerializeField]
    float sharpness = 4;
    override protected Vector3 getGravityDir(Vector3 position)
    {
        Vector3 direction = transform.position - position;
        direction = transform.rotation * direction;
        float magnitude = direction.magnitude;
        direction.x = Mathf.Pow(Mathf.Abs(direction.x), sharpness) * Mathf.Sign(direction.x);
        direction.y = Mathf.Pow(Mathf.Abs(direction.y), sharpness) * Mathf.Sign(direction.y);
        direction.z = Mathf.Pow(Mathf.Abs(direction.z), sharpness) * Mathf.Sign(direction.z);
        direction = direction.normalized * magnitude;
        direction = Quaternion.Inverse(transform.rotation) * direction;
        return direction;
    }
}
