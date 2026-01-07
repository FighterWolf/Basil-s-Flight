using UnityEngine;
using System.Collections.Generic;

public class Missile : MonoBehaviour
{

    public float cruiseSpeed;
    public float speedModifier;
    public float explosionRadius;
    public float fuel;

    public Transform targetToStrike;

    private float cooldown=0.1f;
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
        if (cooldown >= 0)
        {
            cooldown -= Time.deltaTime;
        }
        else
        {
            col.enabled = true;
            transform.SetParent(null);
            if (targetToStrike != null) EssentialFunctions.AimForTarget(transform, targetToStrike, 25f);
        }
        Cruise();

        if (fuel >= 0)
        {
            fuel -= Time.deltaTime;
        }
        else
        {
            Explode();
        }
    }

    public void Cruise()
    {
        rb.AddForce(transform.forward * 2 * (speedModifier+cruiseSpeed), ForceMode.Acceleration);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Explode();
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
