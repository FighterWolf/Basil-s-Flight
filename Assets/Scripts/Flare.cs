using UnityEngine;

public class Flare : Entity
{

    public Entity owner;
    public float fuel;
    public GameObject flareParticle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        flareParticle = Instantiate(explosionParticle,transform.position,Quaternion.identity,transform);
        flareParticle.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (fuel >= 0)
        {
            fuel -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}
