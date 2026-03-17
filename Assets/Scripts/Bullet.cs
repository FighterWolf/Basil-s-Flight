using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{

    public float speed;
    public float speedModifier;
    public float bulletTimer;
    public float gunDamage;
    public Entity owner;

    public Rigidbody rb;
    public Collider col;

    public AudioClip bulletSound;

    private Vector3 lastPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.linearVelocity = transform.forward * (speed + speedModifier);
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //rb.AddForce(transform.forward * (speed + speedModifier), ForceMode.VelocityChange);
        CheckForSurroundings();
        HandleTunneling();
        if (bulletTimer >= 0)
        {
            bulletTimer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.root.TryGetComponent<Entity>(out Entity e))
        {
            if (e != owner)
            {
                e.DecreaseHealth(false,gunDamage);
            }
        }
        Destroy(gameObject);
    }

    public void CheckForSurroundings()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, 32f))
        {
            OnHitTarget(hit);
        }
    }

    public void HandleTunneling()
    {
        RaycastHit hit;

        Vector3 distance = transform.position - lastPosition;
        lastPosition = transform.position;

        Ray r = new Ray(lastPosition,distance.normalized);

        if (Physics.SphereCast(r,2f,out hit, distance.magnitude))
        {
            OnHitTarget(hit);
        }
    }

    public void OnHitTarget(RaycastHit hit)
    {
        if (hit.collider.transform.root.TryGetComponent<Entity>(out Entity e))
        {
            if(!(e is Projectile p && p.tag == owner.tag))
            {
                if (e != owner)
                {
                    EssentialFunctions.OnSuccessfulHit(owner, e, false, gunDamage, "Machine Gun");
                }
            }
        }
        Destroy(gameObject);
    }
}
