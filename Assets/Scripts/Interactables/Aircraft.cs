using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Aircraft : MonoBehaviour, Interactable
{
    public string vehicleName;

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
    private int planeLayer;
    private PlaneWeaponSystem weaponSystem;

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

    public Vector2 look;
    public float yaw;
    public float pitch;
    public float roll;
    public float throttle;
    public bool dismount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planeCam= EssentialFunctions.FindDescendants(transform, "Camera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = maxSpeed * 0.75f;
        planeLayer = LayerMask.GetMask("Plane Parts");
        weaponSystem = GetComponent<PlaneWeaponSystem>();
        if (isLeadPlane)
        {
            formationPosition = FormationPosition.Lead;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pilotInput != null)
        {
            look = pilotInput.look;
            yaw = pilotInput.yaw;
            pitch = pilotInput.pitch;
            roll = pilotInput.roll;
            throttle = pilotInput.throttle;
            dismount = pilotInput.dismount;
        }

        CalculateAltitude();
        OnDismount();
        HandleGlideSpeed();
        RemoveMissingAircraftFromTrailingList();
        if (isLeadPlane)
        {
            checkedList.Clear();
            listOfLastTrailingPlanes.Clear();
            AddAllLastTrailingAircraft(this,listOfLastTrailingPlanes);
        }
    }

    void FixedUpdate()
    {
        OnSteer();
        OnTakeOff();
        AdhereToHeightLimit();

        actualSpeed=rb.linearVelocity.magnitude;

        rb.angularVelocity *= 0.95f;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void Interact(GameObject player)
    {
        OnPlayerEnter(player);
    }

    public string GetName()
    {
        return "Enter " + vehicleName;
    }

    public bool IsHoldable()
    {
        return false;
    }

    public void OnRelease()
    {
        
    }

    public void SwitchControls(bool turnOnPlane,string actionMap)
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

            pilotInput = player.GetComponent<AircraftControls>();
            playerCam = EssentialFunctions.FindDescendants(playerTransform, "MainCamera").GetComponent<Camera>();
            playerTransform.SetParent(EssentialFunctions.FindDescendants(transform, "Seat"));
            playerTransform.localPosition = Vector3.zero;
            playerTransform.localEulerAngles = Vector3.zero;
            SwitchControls(true, "Aircraft");
            weaponSystem.SetPlayer(player.GetComponent<ThirdPersonController>());
        }
    }

    public void OnDismount()
    {
        if (dismount)
        {
            Transform spotToExit=null;

            foreach(Transform o in exitSpots)
            {
                if (o != null)
                {
                    spotToExit = o;
                    break;
                }
            }

            player.transform.position = spotToExit.position;
            player.transform.rotation = spotToExit.rotation;

            player.transform.SetParent(null);
            SwitchControls(false,"Player");
            player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<ThirdPersonController>().enabled = true;
            player.GetComponent<ThirdPersonController>().OnExitVehicle();
            pilotInput = null;
            speed = 0;
            weaponSystem.SetPlayer(null);
            dismount = false;
        }
    }

    public void OnSteer()
    {
        Accelerate(throttle*3);

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
            
            rb.AddTorque(transform.up * yaw * actualSpeed * 1f*Time.fixedDeltaTime, ForceMode.Acceleration);
            rb.AddTorque(transform.right  * pitch * actualSpeed * -1f* Time.fixedDeltaTime, ForceMode.Acceleration);
            rb.AddTorque(transform.forward * roll * actualSpeed * -1f* Time.fixedDeltaTime, ForceMode.Acceleration);
            //OnDrag();
        }
    }

    public void OnTakeOff()
    {
        rb.AddForce(transform.up*pitch*1.2f, ForceMode.Acceleration);
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
            altitude = -1;
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
                //rb.MoveRotation(Quaternion.Euler(0,transform.rotation.y,0));
                rb.AddForce(Vector3.down*5, ForceMode.Force);
            }
        }
    }

    public void OnDrag()
    {
        rb.AddForce(transform.forward * actualSpeed * -0.25f, ForceMode.Force);
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

        
        if (throttle < 0 && speed<=0)
        {
            glideSpeed += throttle;
        }
        else if (speed < 0)
        {
            glideSpeed -= speed;
        }
        else if (glideSpeed > speed)
        {
            glideSpeed -= Time.deltaTime * 5;
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

    public void Explode()
    {
        rb.linearVelocity = new Vector3(0,0,0);
        speed = 0;
        glideSpeed = 0;
        Debug.Log("Plane Exploded");
        //Destroys aircraft
    }
}
