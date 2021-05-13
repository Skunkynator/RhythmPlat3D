using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class GravityArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null)
            entity.addGravityArea(this);
    }
    private void OnTriggerExit(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null)
            entity.removeGravityArea(this);
    }
    abstract public Vector3 getGravity(Vector3 position);
}
