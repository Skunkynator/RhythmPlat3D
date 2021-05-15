using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class GravityArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null)
            entity.addGravityArea(this);
        Debug.Log("Grav ENTER");
    }
    private void OnTriggerExit(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null)
            entity.removeGravityArea(this);
    }
    public Vector3 getGravity(Vector3 position)
    {
        Vector3 direction = getGravityDir(position);
        direction = direction.normalized * (1 / direction.magnitude);
        return direction;
    }
    abstract protected Vector3 getGravityDir(Vector3 position);
}
