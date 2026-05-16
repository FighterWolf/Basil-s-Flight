using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Aircraft : Entity, Interactable
{
    //public string vehicleName;

    public float speed;
    public float maxSpeed;

    public bool overrideHeightLimit;
    public static float maxHeightLimit=100f;

    public float altitude;
    public float actualSpeed;

    public Transform[] exitSpots;
    public VFormationSpot[] vFormations;

    public bool isLeadPlane;
    public Aircraft planeToFollow;

    public float glideSpeed;
    private GameObject player;
    private AircraftControls pilotInput;
    private Camera planeCam;
    private Camera playerCam;
    private Rigidbody rb;
    protected int planeLayer;
    private PlaneWeaponSystem weaponSystem;
    private Transform cameraHolder;

    public enum FormationPosition
    {
        Lead,
        Left,
        Right
    }

    public FormationPosition formationPosition;

    public VFormationSpot whichSpotToFollow;

    public List<Aircraft> listOfLastTrailingPlanes = new List<Aircraft>();
    private HashSet<Aircraft> checkedList = new HashSet<Aircraft>();

    private float steerModifier = 2;

    public Vector2 look;
    public float yaw;
    public float pitch;
    public float roll;
    public float throttle;
    public bool flare;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {

    }
    
    public override void Start()
    {
        base.Start();
        //No idea why the next 4 lines were in Awake()
        planeCam = EssentialFunctions.FindDescendants(transform, "Camera").GetComponent<Camera>();
        weaponSystem = GetComponent<PlaneWeaponSystem>();
        vFormations[0] = EssentialFunctions.FindDescendants(transform, "VFormationLeft").GetComponent<VFormationSpot>();
        vFormations[1] = EssentialFunctions.FindDescendants(transform, "VFormationRight").GetComponent<VFormationSpot>();

        //
        
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = speed * 0.75f;
        EnableModelStats();
        planeLayer = LayerMask.GetMask("Plane Parts");
        cameraHolder = EssentialFunctions.FindDescendants(transform, "LookAtObject");
        if (isLeadPlane)
        {
            formationPosition = FormationPosition.Lead;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        if (PauseMenu.isGameOver)
        {
            pilotInput = null;
        }

        if (!PauseMenu.isPaused)
        {
            base.Update();
            if (pilotInput != null)
            {
                look = pilotInput.look;
                yaw = pilotInput.yaw;
                pitch = pilotInput.pitch;
                roll = pilotInput.roll;
                throttle = pilotInput.throttle;
                flare = pilotInput.flare;
            }

            CalculateAltitude();
            HandleGlideSpeed();
            RemoveMissingAircraftFromTrailingList();
            if (isLeadPlane)
            {
                checkedList.Clear();
                listOfLastTrailingPlanes.Clear();
                AddAllLastTrailingAircraft(this, listOfLastTrailingPlanes);
            }
            LevelAircraft();

            if (health <= 0)
            {
                HandleOnZeroHealth();
            }
        }
    }

    public virtual void FixedUpdate()
    {
        OnSteer();
        AdhereToHeightLimit();

        actualSpeed=rb.linearVelocity.magnitude;

        rb.angularVelocity *= 0.95f;
    }

    public virtual void LateUpdate()
    {
        if(!PauseMenu.isPaused) HandleCamera();
    }

    public void Interact(GameObject player)
    {
        OnPlayerEnter(player);
    }

    public string GetName()
    {
        return "Enter ";// + vehicleName;
    }

    public bool IsHoldable()
    {
        return false;
    }

    public void OnRelease()
    {
        
    }

    public void EnableModelStats()
    {
        PlaneStats stats = GetComponentInChildren<PlaneStats>();
        speed = stats.speed;
        glideSpeed = stats.speed;
        maxSpeed = stats.speed;
        health = stats.health;
        maxHealth = stats.maxHealth;
    }

    public void SwitchControls(bool turnOnPlane)
    {

        playerCam.enabled = !turnOnPlane;
        planeCam.enabled = turnOnPlane;

    }

    public void OnPlayerEnter(GameObject player)
    {
        if(!TryGetComponent<AircraftAI>(out AircraftAI a))
        {
            this.player = player;
            Transform playerTransform = player.transform;

            if (TryGetComponent<PlaneHUD>(out PlaneHUD planeHUD))
            {
                planeHUD.pilot = player;
            }

            pilotInput = player.GetComponent<AircraftControls>();
            playerCam = EssentialFunctions.FindDescendants(playerTransform, "MainCamera").GetComponent<Camera>();
            playerTransform.SetParent(EssentialFunctions.FindDescendants(transform, "Seat"));
            playerTransform.localPosition = Vector3.zero;
            playerTransform.localEulerAngles = Vector3.zero;
            SwitchControls(true);
            weaponSystem.SetPlayer(player.GetComponent<ThirdPersonController>());
        }
    }

    public void HandleOnZeroHealth()
    {
        speed = 0;
        steerModifier = 0.5f;
        isDisabled = true;
    }

    public bool IsDisabled()
    {
        return isDisabled;
    }

    public void OnSteer()
    {
        if(!IsDisabled()) Accelerate(throttle*3);

        if (speed < 0)
        {
            if (throttle >= 0)
            {
                speed = 0;
            }
        }
        else
        {
            rb.AddForce(transform.forward * glideSpeed, ForceMode.Acceleration);

            if (speed<0)
            {
                rb.AddForce(transform.forward * speed*5, ForceMode.Force);
            }
            
            rb.AddTorque(transform.up * yaw * actualSpeed * steerModifier * Time.fixedDeltaTime, ForceMode.Acceleration);
            rb.AddTorque(transform.right  * pitch * actualSpeed * -1f* steerModifier *Time.fixedDeltaTime, ForceMode.Acceleration);
            rb.AddTorque(transform.forward * roll * actualSpeed * -1f* steerModifier * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    public void LevelAircraft()
    {
        if (pilotInput && pilotInput.roll != 0) return;

        float bank = Vector3.Dot(transform.right, Vector3.up);
        roll = Mathf.Clamp(bank, -0.5f, 0.5f);
    }

    public GameObject whatIsBelowPlane;
    public void CalculateAltitude()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down,out hit, Mathf.Infinity, ~planeLayer))
        {
            altitude = hit.distance;
            whatIsBelowPlane = hit.collider.gameObject;
        }
        else
        {
            altitude = Mathf.Infinity;
            whatIsBelowPlane = null;
        }
    }

    public void AdhereToHeightLimit()
    {
        if (!overrideHeightLimit)
        {
            if (altitude > maxHeightLimit)
            {
                float excessHeight = altitude - maxHeightLimit;
                rb.AddForce(Vector3.down*5, ForceMode.Force);
            }
        }
    }

    public void Accelerate(float speed)
    {
        this.speed += speed; 
        this.speed = Mathf.Clamp(this.speed, -5, maxSpeed);

        if (speed > 0)
        {
            glideSpeed += speed;
            glideSpeed = Mathf.Clamp(glideSpeed, -5, maxSpeed);
        }
    }

    public void HandleGlideSpeed()
    {
        if (glideSpeed < 0)
        {
            glideSpeed = 0;
            return;
        }

        if (glideSpeed > speed || (IsDisabled()&& glideSpeed>0))
        {
            glideSpeed -= Time.deltaTime * 5;
        }
        else if (throttle < 0 && speed<=0)
        {
            glideSpeed += throttle;
        }
        else if (speed < 0)
        {
            glideSpeed -= speed;
        }
    }
    public Aircraft GetLeadAircraft()
    {
        if (planeToFollow != null && planeToFollow.TryGetComponent<Aircraft>(out Aircraft leadPlane))
        {
            return leadPlane.GetLeadAircraft();
        }
        else
        {
            return this;
        }
    }

    public void AddAllLastTrailingAircraft(Aircraft plane, List<Aircraft> list)
    {
        if (!checkedList.Add(plane))
        {
            return;
        }

        if (plane.isLeadPlane)
        {
            //This assumes that more than one of the formation spots can be occupied.
            bool areAllSpotsFull = true;

            foreach (VFormationSpot v in plane.vFormations)
            {
                if (v.whoTakesTheSpot != null)
                {
                    AddAllLastTrailingAircraft(v.whoTakesTheSpot, list);
                }
                else
                {
                    areAllSpotsFull = false;
                }
            }

            if (!areAllSpotsFull)
            {
                if (!list.Contains(plane))
                {
                    list.Add(plane);
                }
            }
            else
            {
                list.Remove(plane);
            }
        }
        else
        {
            bool isAnyPlaneFollowing = false;

            //If there is an aircraft in any of the formation spots, add aircraft to the list. This assumes only one formation spot is occupied.
            foreach (VFormationSpot v in plane.vFormations)
            {
                if (v.whoTakesTheSpot != null)
                {
                    isAnyPlaneFollowing = true;
                    AddAllLastTrailingAircraft(v.whoTakesTheSpot, list);
                    break;
                }
            }
            //Debug.Log(isAnyPlaneFollowing+" "+plane.vehicleName);
            if (!isAnyPlaneFollowing && !list.Contains(plane))
            {
                if (!list.Contains(plane))
                {
                    list.Add(plane);
                }
            }
            else
            {
                if (isAnyPlaneFollowing)
                {
                    switch (plane.formationPosition)
                    {
                        case Aircraft.FormationPosition.Left:
                            if (plane.formationPosition == Aircraft.FormationPosition.Left) list.Remove(plane);
                            break;
                        case Aircraft.FormationPosition.Right:
                            if (plane.formationPosition == Aircraft.FormationPosition.Right) list.Remove(plane);
                            break;
                    }
                }
            }
        }
    }

    public void RemoveMissingAircraftFromTrailingList()
    {
        listOfLastTrailingPlanes.RemoveAll(missingPlane => missingPlane == null);
    }

    public void NavigateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 localPosition = transform.InverseTransformDirection(direction);

        float threshold = 0.05f;

        pitch = Mathf.Clamp(localPosition.y, -1, 1);
        yaw = Mathf.Clamp(localPosition.x, -1, 1);
        if (Mathf.Abs(localPosition.x) > threshold)
        {
            roll = Mathf.Clamp(localPosition.x, -1, 1);
        }
        else
        {
            LevelAircraft();
        }
    }

    public void Patrol()
    {
        ResetRotation();
        if (!currentWaypoint || ReachedWaypoint())
        {
            SearchNextWaypoint();
        }

        if (currentWaypoint)
        {
            NavigateToTarget(currentWaypoint);
        }

        bool ReachedWaypoint()
        {
            if (Vector3.Distance(transform.position, currentWaypoint.position) < waypointDistanceThreshhold)
            {
                if (waypointsIterator == waypoints.Length - 1)
                {
                    waypointsIterator = 0;
                }
                else
                {
                    waypointsIterator++;
                }
                return true;
            }
            return false;
        }

        void SearchNextWaypoint()
        {
            if (waypoints.Length > 0)
            {
                currentWaypoint = waypoints[waypointsIterator];
            }
        }
    }

    public void ResetRotation()
    {
        pitch = 0;
        yaw = 0;
        roll = 0;
    }

    public void HandleCamera()
    {
        if (pilotInput != null)
        {
            if (pilotInput.allowLook && !IsDisabled())
            {
                cameraHolder.rotation *= Quaternion.Euler(pilotInput.look.y * 3 * Time.fixedDeltaTime, pilotInput.look.x * 3 * Time.fixedDeltaTime, 0f);
            }
            else
            {
                cameraHolder.rotation = transform.rotation;
            }
        }
    }

    public void Explode()
    {
        rb.linearVelocity = new Vector3(0,0,0);
        speed = 0;
        glideSpeed = 0;
        health = 0;

        EssentialFunctions.CreateExplosion(explosionParticle, explosionSound, transform.position);

        Debug.Log(killCreditName+" exploded");
    }
}
