using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PlaneHUD : MonoBehaviour
{
    public GameObject targetBox;
    public TextMeshProUGUI distanceCalculator;
    public GameObject pilot;
    public GameObject pilotCanvas;

    public string enemyToSearch;

    private Aircraft plane;
    private PlaneWeaponSystem planeWeaponSystem;
    private Camera cam;
    private Transform cameraHolder;
    public List<Entity> potentialTargets;
    public Entity lockOnTarget;
    public Entity confirmedTarget;

    private Image planeHealthBar;
    private TextMeshProUGUI planeThrottle;
    private TextMeshProUGUI planeSpeed;
    private TextMeshProUGUI currentWeaponSystem;
    private TextMeshProUGUI altitude;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = GetComponent<Aircraft>();
        planeWeaponSystem = GetComponent<PlaneWeaponSystem>();
        cam = EssentialFunctions.FindDescendants(plane.transform,"Camera").GetComponent<Camera>();
        cameraHolder = EssentialFunctions.FindDescendants(plane.transform, "LookAtObject");
    }

    // Update is called once per frame
    void Update()
    {
        lockOnTarget = FindClosestTargetInScreen();
        HandleTargetLock();
        HandleDisplay();
    }

    void HandleTargetLock()
    {
        float targetDistance = Mathf.Infinity;
        if (lockOnTarget) targetDistance=Vector3.Distance(lockOnTarget.transform.position, transform.position);

        if (pilot != null&&lockOnTarget&& targetDistance <= 1000)
        {
            Vector3 targetPositionInScreen = EssentialFunctions.TransformWorldCoordsToScreen(lockOnTarget.transform.position, cam);

            float angle = Vector3.Angle(transform.forward,cameraHolder.forward);
            bool isLookingForward = angle < 45;

            if (targetPositionInScreen.z > 0 && isLookingForward)
            {
                targetBox.SetActive(true);
                if(distanceCalculator!=null) distanceCalculator.text = targetDistance.ToString("F2") + "m";
                targetBox.transform.localPosition = new Vector3(targetPositionInScreen.x, targetPositionInScreen.y, 0);
                confirmedTarget = lockOnTarget;
            }
            else
            {
                targetBox.SetActive(false);
                confirmedTarget = null;
            }
        }
        else
        {
            targetBox.SetActive(false);
            confirmedTarget = null;
        }
    }

    public Entity FindClosestTargetInScreen()
    {
        if (enemyToSearch.Equals("OpFor"))
        {
            potentialTargets = Entity.opForEntity;
        }
        else if (enemyToSearch.Equals("BluFor"))
        {
            potentialTargets = Entity.bluForEntity;
        }

        Entity closest=null;
        float closestDistance = Mathf.Infinity;
        foreach (Entity e in potentialTargets)
        {
            if (e == null|| e == this.GetComponent<Entity>()) continue;
            
            Vector3 viewPoint = cam.WorldToViewportPoint(e.transform.position);
            bool isVisible = viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1 && viewPoint.z > 0;
            if (!isVisible) continue;

            float targetDistance = Vector3.Distance(e.transform.position, plane.transform.position);

            if (targetDistance<=1000&&targetDistance < closestDistance)
            {
                closest = e;
                closestDistance = targetDistance;
            }
        }
        return closest;
    }

    void HandleDisplay()
    {
        if (pilotCanvas)
        {
            Aircraft plane = GetComponent<Aircraft>();
            Entity e = GetComponent<Entity>();
            planeHealthBar = EssentialFunctions.FindDescendants(pilotCanvas.transform,"HealthBar").GetComponent<Image>();
            planeThrottle = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ThrottleSpeed").GetComponent<TextMeshProUGUI>();
            planeSpeed = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ActualSpeed").GetComponent<TextMeshProUGUI>();
            currentWeaponSystem = EssentialFunctions.FindDescendants(pilotCanvas.transform, "WeaponSystem").GetComponent<TextMeshProUGUI>();
            altitude = EssentialFunctions.FindDescendants(pilotCanvas.transform, "Altitude").GetComponent<TextMeshProUGUI>();

            planeHealthBar.fillAmount = e.health / e.maxHealth;

            if (e.health <= (e.maxHealth * 0.33))
            {
                planeHealthBar.color = Color.red;
            }
            else
            {
                planeHealthBar.color = Color.green;
            }

            if (e.health > 0)
            {
                if (plane.speed >= plane.maxSpeed)
                {
                    planeThrottle.color = Color.red;
                    planeThrottle.text = "WEP";
                }
                else
                {
                    planeThrottle.color = Color.white;
                    planeThrottle.text = plane.speed.ToString("F2");
                }

                switch (planeWeaponSystem.weaponSystem)
                {
                    case PlaneWeaponSystem.WeaponSystem.Gun:
                        currentWeaponSystem.text = "Machine Gun";
                        break;
                    case PlaneWeaponSystem.WeaponSystem.Missile:
                        currentWeaponSystem.text = "Missile";
                        break;
                }
            }
            else
            {
                currentWeaponSystem.text = "PLANE DISABLED";
                currentWeaponSystem.color = Color.red;
                planeThrottle.text = "PLANE DISABLED";
                planeThrottle.color = Color.red;
            }

            altitude.text = plane.altitude.ToString("F2");
            planeSpeed.text = plane.glideSpeed.ToString("F2");
        }
    }
}
