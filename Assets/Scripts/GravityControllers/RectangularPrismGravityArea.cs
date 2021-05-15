using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangularPrismGravityArea : GravityArea
{
    [SerializeField]
    float sharpness = 4;
    [SerializeField]
    Vector2 prismArea;
    override protected Vector3 getGravityDir(Vector3 position)
    {
        float minCoord = Mathf.Min(prismArea.x, prismArea.y);
        prismArea.x -= minCoord;
        prismArea.y -= minCoord;
        position = transform.worldToLocalMatrix * position;
        position.y = 0;
        position.z -= Mathf.Clamp(position.z, -prismArea.y / 2, prismArea.y / 2);
        position.x -= Mathf.Clamp(position.x, -prismArea.x / 2, prismArea.x / 2);
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
