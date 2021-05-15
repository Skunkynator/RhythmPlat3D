using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalGravityArea : GravityArea
{
    override protected Vector3 getGravityDir(Vector3 position)
    {
        Vector3 direction = transform.position - position;
        return direction;
    }
}
