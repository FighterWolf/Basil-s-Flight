using UnityEngine;
using System.Collections.Generic;

public class Missile : Projectile
{
    public float explosionRadius;

    public Transform targetToStrike;

    public float distance;

    private Vector3 prediction;

    private Rigidbody rb;

    public AudioClip missileSound;
    public AudioClip launchMissileSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        EssentialFunctions.FindDescendants(transform, "Marker").GetComponent<Canvas>().worldCamera = GameObject.Find("MinimapCamera").GetComponent<Camera>();

        rb = GetComponent<Rigidbody>();
        if (targetToStrike) targetToStrike.GetComponent<Entity>().missilesHeadingTowardsSelf.Add(this);

        source.clip = missileSound;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        CheckHeatSignature();
        
        if(targetToStrike) distance = Vector3.Distance(transform.position, targetToStrike.position);

        if (fuel >= 0)
        {
            fuel -= Time.deltaTime;
        }
        else
        {
            Explode();
        }

        if(targetToStrike&&Vector3.Distance(transform.position, targetToStrike.position) < explosionRadius && Vector3.Distance(transform.position, owner.transform.position)> explosionRadius)
        {
            Explode();
        }

        EssentialFunctions.HandleSound(source,PauseMenu.isPaused);
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * (speed + speedModifier), ForceMode.VelocityChange);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, speed + speedModifier);

        if (targetToStrike != null) {
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

    void CheckHeatSignature()
    {
        if (EssentialFunctions.EntityInFront(transform, 40, 350) is Flare f)
        {
            this.targetToStrike = f.transform;
        }
    }

    public void Explode()
    {
        EssentialFunctions.CreateExplosion(explosionParticle,explosionSound,transform.position);

        Collider[] affectedColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        HashSet<Entity> hitEntities = new HashSet<Entity>();

        foreach (var collider in affectedColliders)
        {
            if (collider.TryGetComponent<Entity>(out Entity entity) || collider.transform.root.TryGetComponent<Entity>(out entity))
            {
                if (hitEntities.Add(entity) && entity != owner)
                {
                    EssentialFunctions.OnSuccessfulHit(owner,entity,true,damage, "Missile");

                    if (entity.TryGetComponent<AircraftAI>(out AircraftAI planeAI))
                    {
                        planeAI.isHit = true;
                    }
                }
            }
        }

        if (targetToStrike) targetToStrike.GetComponent<Entity>().missilesHeadingTowardsSelf.Remove(this);
        
        Destroy(gameObject);
    }
}
