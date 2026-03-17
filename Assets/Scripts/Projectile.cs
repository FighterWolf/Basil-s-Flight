using UnityEngine;

public class Projectile : Entity
{
    public float fuel;
    public float damage;
    public float speed;
    public float speedModifier;
    public Entity owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
   
    public void AssignStats(float damage,Entity owner,float speed = 0,float speedModifier=0)
    {
        this.damage = damage;
        this.owner = owner;
        this.speed = speed;
        this.speedModifier = speedModifier;
        tag = owner.tag;
    }
}
