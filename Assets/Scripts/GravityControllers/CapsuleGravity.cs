using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleGravity : GravityArea
{
    [SerializeField]
    float height = 1;
    override protected Vector3 getGravityDir(Vector3 position)
    {
        position = transform.worldToLocalMatrix * position;
        position.y -= Mathf.Clamp(position.y, -height / 2, height / 2);
        position = transform.localToWorldMatrix * position;
        Vector3 direction = transform.position - position;
        return direction;
    }
}
