using UnityEngine;
using System.Collections.Generic;
using System.Collections;
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
    private HandlePlayer player;
    private LevelHandler level;
    public List<Entity> potentialTargets;
    public Entity lockOnTarget;
    public Entity confirmedTarget;

    private Image planeHealthBar;
    private TextMeshProUGUI planeThrottle;
    private TextMeshProUGUI planeSpeed;
    private TextMeshProUGUI currentWeaponSystem;
    private TextMeshProUGUI altitude;
    private GameObject missileWarning;
    private TextMeshProUGUI pointsCounter;

    private Image planeMissileReloadStatus;
    private Image planeFlareReloadStatus;

    private float missileReloadRate;
    public float missileCooldown;

    private float flareReloadRate;
    public float flareCooldown;

    private GameObject currentRing;
    private Transform objectivePointer;
    private GameObject objectiveDistance;

    private bool blinkActive = true;
    private Coroutine blink;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pilotCanvas = GetComponent<HandlePlayer>().pilotCanvas.gameObject;
        plane = GetComponent<Aircraft>();
        planeWeaponSystem = GetComponent<PlaneWeaponSystem>();
        cam = EssentialFunctions.FindDescendants(plane.transform,"Camera").GetComponent<Camera>();
        cameraHolder = EssentialFunctions.FindDescendants(plane.transform, "LookAtObject");

        planeHealthBar = EssentialFunctions.FindDescendants(pilotCanvas.transform, "HealthBar").GetComponent<Image>();
        planeThrottle = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ThrottleSpeed").GetComponent<TextMeshProUGUI>();
        planeSpeed = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ActualSpeed").GetComponent<TextMeshProUGUI>();
        currentWeaponSystem = EssentialFunctions.FindDescendants(pilotCanvas.transform, "WeaponSystem").GetComponent<TextMeshProUGUI>();
        altitude = EssentialFunctions.FindDescendants(pilotCanvas.transform, "Altitude").GetComponent<TextMeshProUGUI>();
        missileWarning = EssentialFunctions.FindDescendants(pilotCanvas.transform, "MissileWarning").gameObject;
        objectivePointer = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ObjectivePointer");
        objectiveDistance = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ObjectiveDistance").gameObject;
        Transform pc =EssentialFunctions.FindDescendants(pilotCanvas.transform, "PointCounter");

        if(pc) pointsCounter = pc.GetComponent<TextMeshProUGUI>();

        player = GetComponent<HandlePlayer>();
        level = pilotCanvas.GetComponent<LevelHandler>();

        planeMissileReloadStatus = EssentialFunctions.FindDescendants(pilotCanvas.transform, "MissileReloadBar").GetComponent<Image>();
        planeFlareReloadStatus = EssentialFunctions.FindDescendants(pilotCanvas.transform, "FlareReloadBar").GetComponent<Image>();

        missileReloadRate = planeWeaponSystem.missileReloadRate;
        flareReloadRate = planeWeaponSystem.flareReloadRate;

        missileCooldown = missileReloadRate;
        flareCooldown = flareReloadRate;
    }

    // Update is called once per frame
    void Update()
    {
        lockOnTarget = FindClosestTargetInScreen();
        HandleTargetLock();
        HandleObjectiveArrow();
        HandleDisplay();
    }

    void HandleTargetLock()
    {
        float targetDistance = Mathf.Infinity;
        if (lockOnTarget) targetDistance=Vector3.Distance(lockOnTarget.transform.position, transform.position);

        if (pilot != null&&lockOnTarget&& targetDistance <= 1000)
        {
            Vector3 targetPositionInScreen = planeWeaponSystem.weaponSystem==PlaneWeaponSystem.WeaponSystem.Missile ? EssentialFunctions.TransformWorldCoordsToScreen(lockOnTarget.transform.position, cam) : EssentialFunctions.TransformWorldCoordsToScreen(EstimatedTargetFutureCoordinates(lockOnTarget), cam);

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

    Vector3 EstimatedTargetFutureCoordinates(Entity e)
    {
        Rigidbody targetRigidBody = e.GetComponent<Rigidbody>();

        return targetRigidBody.position + targetRigidBody.linearVelocity *0.5f;
    }

    void HandleDisplay()
    {
        if (pilotCanvas)
        {
            planeHealthBar.fillAmount = plane.health / plane.maxHealth;

            if (plane.health <= (plane.maxHealth * 0.33))
            {
                planeHealthBar.color = Color.red;
            }
            else
            {
                planeHealthBar.color = Color.green;
            }

            if (plane.altitude < 200)
            {
                altitude.color = Color.red;
            }
            else
            {
                altitude.color = Color.white;
            }

            if (plane.health > 0)
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

            if (plane.IsBeingLockedOn())
            {
                if (blink == null) blink = StartCoroutine(FlickerText());
            }
            else
            {
                missileWarning.SetActive(false);
                blink = null;
            }

            altitude.text = plane.altitude.ToString("F2");
            planeSpeed.text = plane.glideSpeed.ToString("F2");

            if (missileCooldown<missileReloadRate)
            {
                missileCooldown += Time.deltaTime;
                planeMissileReloadStatus.fillAmount = missileCooldown / missileReloadRate;
                planeMissileReloadStatus.color = Color.red;
            }
            else if (planeWeaponSystem.isReadyToBomb||missileCooldown>=missileReloadRate)
            {
                planeMissileReloadStatus.fillAmount = 1;
                planeMissileReloadStatus.color = Color.green;
            }

            if (flareCooldown < flareReloadRate)
            {
                flareCooldown += Time.deltaTime;
                planeFlareReloadStatus.fillAmount = flareCooldown / flareReloadRate;
                planeFlareReloadStatus.color = Color.red;
            }
            else if (planeWeaponSystem.isFlareReady||flareCooldown>=flareReloadRate)
            {
                planeFlareReloadStatus.fillAmount = 1;
                planeFlareReloadStatus.color = Color.green;
            }

            if(pointsCounter) pointsCounter.text = (level.currentKillPoints + level.currentRingPoints) + " / " + (level.numberOfKillsNeeded + level.numberOfRingsToFlyThrough);
        }
    }

    void HandleObjectiveArrow()
    {
        Ring ring = FindFirstObjectByType<Ring>();
        currentRing = ring!=null ? ring.gameObject : null;

        if (currentRing != null)
        {
            objectivePointer.gameObject.SetActive(true);
            objectiveDistance.gameObject.SetActive(true);
            Vector3 ringPositionInScreen = EssentialFunctions.TransformWorldCoordsToScreen(currentRing.transform.position,cam);
            float isBehind = ringPositionInScreen.z > 0 ? 0 : 180;
            objectivePointer.localEulerAngles=new Vector3(0,0, isBehind+Vector2.SignedAngle(Vector2.up, new Vector2(ringPositionInScreen.x, ringPositionInScreen.y)));
            objectiveDistance.GetComponent<TextMeshProUGUI>().text = "Objective Distance: " + EssentialFunctions.FindDescendants(ring.transform.parent,"Distance").GetComponent<TextMeshProUGUI>().text;
        }
        else
        {
            objectivePointer.gameObject.SetActive(false);
            objectiveDistance.gameObject.SetActive(false);
        }
    }

    IEnumerator FlickerText()
    {
        while (plane.IsBeingLockedOn())
        {
            blinkActive = !blinkActive;
            missileWarning.SetActive(blinkActive);
            yield return new WaitForSeconds(0.25f);
        }
        blink = null;
    }
}
