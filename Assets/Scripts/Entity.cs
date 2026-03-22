using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{   
    public static List<Entity> bluForEntity = new List<Entity>();
    public static List<Entity> opForEntity = new List<Entity>();

    public List<Missile> missilesHeadingTowardsSelf = new List<Missile>();
    public List<Flare> deployedFlares = new List<Flare>();

    public string killCreditName;

    public float health;
    public float maxHealth;

    public bool isDisabled;
    protected float missileAwareness;

    private bool isOnHitCooldown;
    private Coroutine hitCooldown;

    private Marker marker;

    protected AudioSource source;
    public AudioClip missileWarning;

    public GameObject explosionParticle;
    public AudioClip explosionSound;

    //Waypoint system only if entity is AI
    public Transform[] waypoints;
    protected Transform currentWaypoint;
    public int waypointsIterator = 0;
    protected float waypointDistanceThreshhold = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        if (gameObject.CompareTag("BluFor"))
        {
            bluForEntity.Add(this);
        }else if (gameObject.CompareTag("OpFor"))
        {
            opForEntity.Add(this);
        }
        marker = EssentialFunctions.FindDescendants(transform, "Marker").GetComponent<Marker>();
        if(TryGetComponent<AudioSource>(out AudioSource a)) source = a;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        OnZeroHealth();
        ClearNulls();
        if (source&&missileWarning)
        {
            source.clip = missileWarning;
            EssentialFunctions.HandleSound(source, !IsBeingLockedOn());
        }

        if (LevelHandler.isLevelComplete && gameObject.CompareTag("OpFor"))
        {
            health = 0;
        }
    }

    void OnZeroHealth()
    {
        if (health <= 0)
        {
            if (gameObject.CompareTag("BluFor"))
            {
                bluForEntity.Remove(this);
            }
            else if (gameObject.CompareTag("OpFor"))
            {
                opForEntity.Remove(this);
            }
            if(marker&&!marker.isPlayer)Destroy(marker.gameObject);
        }

        if (missilesHeadingTowardsSelf.Count > 0 &&missileAwareness<5)
        {
            missileAwareness += Time.deltaTime;
        }
        else
        {
            missileAwareness = 0;
        }
    }

    public void DecreaseHealth(bool ignoreCooldown,float health)
    {
        if (!isOnHitCooldown||ignoreCooldown)
        {
            if (this.health - health <= 0)
            {
                this.health = 0;
            }
            else
            {
                this.health -= health;
            }
        }
        if (hitCooldown == null)
        {
            hitCooldown = StartCoroutine(HitCooldown());
        }
    }

    private IEnumerator HitCooldown()
    {
        isOnHitCooldown = true;
        yield return new WaitForSeconds(0.25f);
        isOnHitCooldown = false;
        hitCooldown = null;
    }

    public void IncreaseHealth(float health)
    {
        if (this.health + health >= maxHealth)
        {
            this.health = maxHealth;
        }
        else
        {
            this.health += health;
        }
    }

    public bool IsBeingLockedOn()
    {
        if (missilesHeadingTowardsSelf.Count > 0)
        {
            return true;
        }
        return false;
    }

    void ClearNulls()
    {
        missilesHeadingTowardsSelf.RemoveAll(m => m == null);
        deployedFlares.RemoveAll(f => f == null);
    }
}
