using UnityEngine;
using System.Collections.Generic;

public class PlaneHUD : MonoBehaviour
{
    public GameObject targetBox;
    public GameObject pilot;
    private Aircraft plane;
    private PlaneWeaponSystem planeWeaponSystem;
    private Camera cam;
    public Entity[] potentialTargets;
    public Entity lockOnTarget;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = GetComponent<Aircraft>();
        planeWeaponSystem = GetComponent<PlaneWeaponSystem>();
        cam = EssentialFunctions.FindDescendants(plane.transform,"Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        potentialTargets = Entity.FindObjectsByType<Entity>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);
        lockOnTarget = FindClosestTargetInScreen();
        HandleTargetLock();
    }

    void HandleTargetLock()
    {
        if (pilot != null&&planeWeaponSystem.weaponSystem==PlaneWeaponSystem.WeaponSystem.Missile&&lockOnTarget != null)
        {
            float targetDistance = Vector3.Distance(lockOnTarget.transform.position, transform.position);
            Vector3 targetPositionInScreen = EssentialFunctions.TransformWorldCoordsToScreen(lockOnTarget.transform.position, cam);

            if (targetPositionInScreen.z > 0)
            {
                targetBox.SetActive(true);
                targetBox.transform.localPosition = new Vector3(targetPositionInScreen.x, targetPositionInScreen.y, 0);
            }
            else
            {
                targetBox.SetActive(false);
            }
        }
        else
        {
            targetBox.SetActive(false);
        }
    }

    public Entity FindClosestTargetInScreen()
    {
        Entity closest=null;
        float closestDistance = Mathf.Infinity;
        foreach (Entity e in potentialTargets)
        {
            if (e == null|| e == this.GetComponent<Entity>()) continue;
            
            //This viewpoint part has problems, not turning true
            Vector3 viewPoint = cam.WorldToViewportPoint(e.transform.position);
            bool isVisible = viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1 && viewPoint.z > 0;
            if (!isVisible) continue;

            float targetDistance = Vector3.Distance(e.transform.position, plane.transform.position);

            if (targetDistance < closestDistance)
            {
                closest = e;
                closestDistance = targetDistance;
            }
        }
        return closest;
    }
}
