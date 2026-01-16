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
        Quaternion q = Quaternion.LookRotation((targetTransform.position - ownerTransform.position).normalized);
        ownerTransform.rotation = Quaternion.RotateTowards(ownerTransform.rotation,q,rotationSpeed);
    }

    public static void AimForTarget(Transform ownerTransform, Vector3 targetTransform, float rotationSpeed)
    {
        Quaternion q = Quaternion.LookRotation((targetTransform - ownerTransform.position).normalized);
        ownerTransform.rotation = Quaternion.RotateTowards(ownerTransform.rotation, q, rotationSpeed);
    }

    public static Vector3 TransformWorldCoordsToScreen(Vector3 objectPosition,Camera camera)
    {
        Vector3 screenCoords = camera.WorldToScreenPoint(objectPosition);
        return screenCoords - new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2);
    }
}
