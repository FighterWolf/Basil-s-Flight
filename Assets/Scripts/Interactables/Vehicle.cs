using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class Vehicle : MonoBehaviour, Interactable
{
    public string vehicleName;

    public float speed;
    public float maxSpeed;

    private float turnSpeed;

    public GameObject[] exitSpots;

    private GameObject player;
    private AircraftControls pilotInput;
    private Camera planeCam;
    private Camera playerCam;
    private Rigidbody rb;

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

        OnDismount();
    }

    void FixedUpdate()
    {
        OnDrive();
        OnTakeOff();

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
        this.player = player;
        Transform playerTransform = player.transform;

        pilotInput = player.GetComponent<AircraftControls>();
        playerTransform.position = transform.position;
        playerTransform.rotation = transform.rotation;
        playerCam = EssentialFunctions.FindDescendants(playerTransform, "MainCamera").GetComponent<Camera>();
        playerTransform.SetParent(transform);
        SwitchControls(true,"Aircraft");
    }

    public void OnDismount()
    {
        if (dismount)
        {
            Transform spotToExit=null;

            foreach(GameObject o in exitSpots)
            {
                if (o != null)
                {
                    spotToExit = o.transform;
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
            dismount = false;
        }
    }

    public void OnDrive()
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
            turnSpeed = Mathf.Lerp(maxSpeed * 0.25f, maxSpeed, rb.linearVelocity.magnitude);

            rb.AddForce(transform.forward * speed);
            
            rb.AddTorque(transform.up * yaw * turnSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
            rb.AddTorque(transform.right  * pitch * turnSpeed * -1 * Time.fixedDeltaTime, ForceMode.Acceleration);
            rb.AddTorque(transform.forward * roll * turnSpeed * -1 * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    public void OnTakeOff()
    {
        rb.AddForce(transform.up*pitch*1.2f, ForceMode.Acceleration);
    }

    public void Accelerate(float speed)
    {
        this.speed += speed*Time.deltaTime;
        this.speed = Mathf.Clamp(this.speed, -5, maxSpeed);
    }

    public void Explode()
    {
        Debug.Log("Plane Exploded");
        //Destroys aircraft
    }
}
