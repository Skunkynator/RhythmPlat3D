using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderGravityArea : GravityArea
{
    override protected Vector3 getGravityDir(Vector3 position)
    {
        position = transform.worldToLocalMatrix * position;
        position.y = 0;
        position = transform.localToWorldMatrix * position;
        Vector3 direction = transform.position - position;
        return direction;
    }
}
