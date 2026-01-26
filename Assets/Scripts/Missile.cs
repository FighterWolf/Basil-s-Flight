using UnityEngine;
using System.Collections.Generic;

public class Missile : MonoBehaviour
{

    public float cruiseSpeed;
    public float speedModifier;
    public float explosionRadius;
    public float fuel;

    public Transform targetToStrike;
    public float distance;

    private Vector3 prediction;

    private float cooldown=0.75f;
    private Rigidbody rb;
    private Collider col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(targetToStrike) distance = Vector3.Distance(transform.position, targetToStrike.position);
        if (cooldown >= 0)
        {
            cooldown -= Time.deltaTime;
        }
        else
        {
            col.enabled = true;
            transform.SetParent(null);
        }

        if (fuel >= 0)
        {
            fuel -= Time.deltaTime;
        }
        else
        {
            Explode();
        }

        if(targetToStrike&&Vector3.Distance(transform.position, targetToStrike.position) < explosionRadius)
        {
            Explode();
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * (cruiseSpeed + speedModifier), ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, cruiseSpeed+speedModifier);

        if (cooldown < 0 && targetToStrike != null) {
            float percentageOfWayToTarget = Mathf.InverseLerp(1,1000,Vector3.Distance(transform.position,targetToStrike.position));

            PredictMovement(percentageOfWayToTarget);

            RotateMissile();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void PredictMovement(float percentageOfWayToTarget)
    {
        float time = Mathf.Lerp(0, 5, percentageOfWayToTarget);

        Rigidbody targetRigidBody = targetToStrike.GetComponent<Rigidbody>();

        prediction = targetRigidBody.position + targetRigidBody.linearVelocity * time;
    }

    void RotateMissile()
    {
        Vector3 coordsToStrike=prediction-transform.position;

        Quaternion rotation = Quaternion.LookRotation(coordsToStrike);
        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation,rotation,120*Time.deltaTime));
    }

    public void Explode()
    {
        if (cooldown <= 0)
        {
            Collider[] affectedColliders = Physics.OverlapSphere(transform.position, explosionRadius);

            HashSet<Entity> hitEntities = new HashSet<Entity>();

            foreach (var collider in affectedColliders)
            {
                if (collider.TryGetComponent<Entity>(out Entity entity) || collider.transform.root.TryGetComponent<Entity>(out entity))
                {
                    if (hitEntities.Add(entity))
                    {
                        entity.DecreaseHealth(150f);
                    }
                }
            }

            Debug.Log("Exploded");
            Destroy(gameObject);
        }
    }
}
