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

        RectTransform rt = GameObject.Find("Canvas").GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt,screenCoords,null,out Vector2 localPoint);

        return new Vector3(localPoint.x,localPoint.y,screenCoords.z);
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

    public static void HandleSound(AudioSource source,bool stopLoop)
    {
        if (!source.loop&&!stopLoop)
        {
            source.loop = true;
            source.Play();
        }else if (stopLoop)
        {
            source.loop = false;
            source.Stop();
        }
    }
}
