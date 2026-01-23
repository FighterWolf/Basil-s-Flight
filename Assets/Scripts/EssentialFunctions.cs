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
        Vector3 newDirection = Vector3.RotateTowards(ownerTransform.forward,desiredDirection,rotationSpeed*Mathf.Rad2Deg*Time.deltaTime,0f);
        ownerTransform.rotation = Quaternion.LookRotation(newDirection);
    }

    public static Vector3 TransformWorldCoordsToScreen(Vector3 objectPosition,Camera camera)
    {
        Vector3 screenCoords = camera.WorldToScreenPoint(objectPosition);
        return screenCoords - new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2);
    }
}
