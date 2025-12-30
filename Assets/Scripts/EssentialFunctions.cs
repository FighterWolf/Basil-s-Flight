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
}
