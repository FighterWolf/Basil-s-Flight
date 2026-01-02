using UnityEngine;

public class PlaneCrashEvent : MonoBehaviour
{
    private Vehicle plane;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = transform.root.GetComponent<Vehicle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsColliderAPartOfThePlane(other)&&plane.speed>(plane.maxSpeed*0.25f)&&(other.GetType() != typeof(CharacterController)))
        {
            plane.speed -= plane.maxSpeed * 0.25f;
            plane.Explode();
        }
    }

    bool IsColliderAPartOfThePlane(Collider c)
    {
        if (c.transform.root == transform.root)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
