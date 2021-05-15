using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboidGravityArea : GravityArea
{
    [SerializeField]
    float sharpness = 4;
    [SerializeField]
    Vector3 cuboidSize;
    override protected Vector3 getGravityDir(Vector3 position)
    {
        float minCoord = Mathf.Min(cuboidSize.x, cuboidSize.y, cuboidSize.z);
        cuboidSize.x -= minCoord;
        cuboidSize.y -= minCoord;
        cuboidSize.z -= minCoord;
        position = transform.worldToLocalMatrix * position;
        position.y -= Mathf.Clamp(position.y, -cuboidSize.y / 2, cuboidSize.y / 2);
        position.z -= Mathf.Clamp(position.z, -cuboidSize.z / 2, cuboidSize.z / 2);
        position.x -= Mathf.Clamp(position.x, -cuboidSize.x / 2, cuboidSize.x / 2);
        position = transform.localToWorldMatrix * position;
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
