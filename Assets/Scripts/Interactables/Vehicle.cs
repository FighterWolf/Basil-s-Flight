using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class Vehicle : MonoBehaviour, Interactable
{
    public string vehicleName;

    public float speed;
    public float maxSpeed;

    public GameObject[] exitSpots;

    private GameObject player;
    private AircraftControls pilotInput;
    private Camera planeCam;
    private Camera playerCam;

    public float yaw;
    public float pitch;
    public float roll;
    public float throttle;
    public bool dismount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        planeCam= EssentialFunctions.FindDescendants(transform, "Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pilotInput != null)
        {
            yaw = pilotInput.yaw;
            pitch = pilotInput.pitch;
            roll = pilotInput.roll;
            throttle = pilotInput.throttle;
            dismount = pilotInput.dismount;
        }

        OnDismount();
        OnDrive();
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
            player.GetComponent<ThirdPersonController>().OnExitVehicle();
            pilotInput = null;
            dismount = false;
        }
    }

    public void OnDrive()
    {
        Accelerate(throttle*3);

        if (speed != 0)
        {
            UseRudder(yaw * speed * Time.deltaTime);
            UseElevator(pitch * speed * Time.deltaTime);
            UseAileron(roll * speed * Time.deltaTime);

            transform.position += transform.forward * speed * Time.deltaTime;
        }

    }

    public void Accelerate(float speed)
    {
        this.speed += speed*Time.deltaTime;
        this.speed = Mathf.Clamp(this.speed, -1, maxSpeed);
    }

    public void UseRudder(float y)
    {
        transform.rotation *= Quaternion.Euler(0,y,0);
    }

    public void UseElevator(float x)
    {
        transform.rotation *= Quaternion.Euler(-x, 0, 0);
    }

    public void UseAileron(float z)
    {
        transform.rotation *= Quaternion.Euler(0, 0, -z);
    }
}
