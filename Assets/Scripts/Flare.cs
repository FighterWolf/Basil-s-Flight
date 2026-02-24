using UnityEngine;

public class Flare : Entity
{

    public Entity owner;
    public float fuel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
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
