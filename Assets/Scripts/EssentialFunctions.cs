using UnityEngine;

public class EssentialFunctions : MonoBehaviour
{
    public static Transform FindDescendants(Transform t, string name)
    {
        foreach(Transform child in t)
        {
            if (child.name == name)
            {
                return child;
            }
            else
            {
                if (FindDescendants(child, name)!=null)
                {
                    return FindDescendants(child, name);
                }
            }
        }
        return null;
    }

    public static void AimForTarget(Transform ownerTransform, Transform targetTransform, float rotationSpeed)
    {
        Vector3 desiredDirection = (targetTransform.position - ownerTransform.position).normalized;
        Vector3 newDirection = Vector3.RotateTowards(ownerTransform.forward,desiredDirection,rotationSpeed*Time.deltaTime,0f);
        ownerTransform.rotation = Quaternion.LookRotation(newDirection);
    }

    public static Vector3 TransformWorldCoordsToScreen(Vector3 objectPosition,Camera camera)
    {
        Vector3 screenCoords = camera.WorldToScreenPoint(objectPosition);
        return screenCoords - new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2);
    }

    public static void OnSuccessfulHit(Entity killer,Entity target, bool ignoreCooldown, float decreaseHealth, string weaponName)
    {
        if (!target.isDisabled)
        {
            target.DecreaseHealth(ignoreCooldown,decreaseHealth);

            if (target.health <= 0)
            {
                if(!(target is Flare))
                {
                    Debug.Log(killer.killCreditName + " eliminated " + target.killCreditName + " using: " + weaponName);
                }

                //reward killer

                //play sound for all, if player, display on screen.
            }
            else
            {
                if (!(target is Flare))
                {
                    Debug.Log(killer.killCreditName + " eliminated " + target.killCreditName + " using: " + weaponName);
                }
            }
        }
    }

    public static Entity EntityInFront(Transform owner,float radius, float distance)
    {
        if (Physics.SphereCast(owner.position, radius, owner.forward, out RaycastHit hit, distance))
        {
            if (hit.collider.transform.root.TryGetComponent<Entity>(out Entity e))
            {
                return e;
            }
        }
        return null;
    }
}
