using UnityEngine;

public class PlaneCrashEvent : MonoBehaviour
{
    private Aircraft plane;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = transform.root.GetComponent<Aircraft>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsColliderAPartOfThePlane(other) && plane.actualSpeed > 25f && (other.GetType() != typeof(CharacterController)) && other.GetComponent<Bullet>() == null && other.GetComponent<Projectile>() == null && !other.isTrigger)
        {
            if (other.gameObject.GetComponent<Flare>())
            {
                Destroy(other.gameObject);
                return;
            }
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
