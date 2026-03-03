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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(transform.forward * (speed + speedModifier), ForceMode.VelocityChange);
        CheckForSurroundings();
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
        if(Physics.SphereCast(transform.position,3,transform.forward,out hit))
        {
            if(hit.collider.transform.root.TryGetComponent<Entity>(out Entity e))
            {
                if (e != owner)
                {
                    EssentialFunctions.OnSuccessfulHit(owner, e,false, gunDamage,"Machine Gun");
                }
            }
            Destroy(gameObject);
        }
    }
}
