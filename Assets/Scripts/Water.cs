using UnityEngine;

public class Water : MonoBehaviour
{
    public GameObject splashEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent<Rigidbody>(out Rigidbody rigidBody))
        {
            if (Vector3.Distance(rigidBody.linearVelocity,Vector3.one)>10)
            {
                Instantiate(splashEffect, other.transform.position + Vector3.up * 2, Quaternion.identity);
            }

            if (other.transform.root.TryGetComponent<Aircraft>(out Aircraft plane))
            {
                DisableAircraft(plane);
                rigidBody.linearVelocity = new Vector3(0, 0, 0);
            }
            else if(other.transform.root.GetComponent<Projectile>()!=null || other.transform.root.GetComponent<Bullet>() != null)
            {
                Destroy(other.transform.root.gameObject);
            }
        }
        else
        {
            Instantiate(splashEffect, other.transform.position + Vector3.up * 2, Quaternion.identity);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.TryGetComponent<Aircraft>(out Aircraft plane))
        {
            Rigidbody rb = other.transform.root.GetComponent<Rigidbody>();
            DisableAircraft(plane);
            rb.AddForce(Vector3.up * 7.5f, ForceMode.Acceleration);
            plane.DecreaseHealth(false, 10);
        }
    }

    void DisableAircraft(Aircraft plane)
    {
        plane.ResetRotation();
        plane.speed = 0;
        plane.throttle = 0;
        plane.glideSpeed = 0;
    }
}
