using UnityEngine;
using System.Collections.Generic;

public class PlaneHUD : MonoBehaviour
{
    public GameObject targetBox;
    private Aircraft plane;
    private Camera camera;
    public List<Entity> potentialTargets;
    private Entity lockOnTarget;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = GetComponent<Aircraft>();
        camera = EssentialFunctions.FindDescendants(plane.transform,"Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        lockOnTarget = FindClosestTargetInScreen();
    }

    void HandleTargetLock()
    {
        
        float targetDistance = Vector3.Distance(lockOnTarget.transform.position,transform.position);
        Vector3 targetPositionInScreen = EssentialFunctions.TransformWorldCoordsToScreen(lockOnTarget.transform.position,camera);

        if (targetPositionInScreen.z > 0)
        {
            targetBox.SetActive(true);
            targetBox.transform.localPosition = new Vector3(targetPositionInScreen.x,targetPositionInScreen.y,0);
        }
        else
        {
            targetBox.SetActive(false);
        }
    }

    public Entity FindClosestTargetInScreen()
    {
        Entity closest=null;
        float distance = Mathf.Infinity;
        foreach (Entity e in potentialTargets)
        {
            if (e == null|| e == this.GetComponent<Entity>()) continue;

            Vector3 viewPoint = camera.WorldToViewportPoint(e.transform.position);

            bool isVisible = viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 0 && viewPoint.z > 0;

            if (!isVisible) continue;

            float targetDistance = Vector3.Distance(e.transform.position, plane.transform.position);

            if (targetDistance<distance)
            {
                closest = e;
                distance = targetDistance;
            }
        }
        return closest;
    }
}
