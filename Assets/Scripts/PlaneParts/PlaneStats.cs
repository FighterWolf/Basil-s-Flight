using UnityEngine;

public class PlaneStats : MonoBehaviour
{
    public string planeClass;

    public float speed;
    public float maxSpeed;

    public float health;
    public float maxHealth;

    public float gunDamage;
    public float missileDamage;

    public float gunFireRate;
    public float missileReloadRate;

    public float flareReloadRate;

    public GameObject gunPod;
    public Bullet bullet;

    public GameObject missilePod;
    public Missile missile;

    public GameObject flareDeployer;
    public Flare flareObject;
}
