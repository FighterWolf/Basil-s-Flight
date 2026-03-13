using UnityEngine;

public class EntityDamageSmoke : MonoBehaviour
{
    private Entity entity;
    private float entityMaxHealth;

    public GameObject currentDamageSmoke;

    public GameObject lightDamageSmoke;
    public GameObject heavyDamageSmoke;
    public GameObject deathSmoke;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(TryGetComponent<Entity>(out Entity e)||transform.root.TryGetComponent<Entity>(out e)) entity = e;
        entityMaxHealth = entity.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSmokeToUse();
        SummonSmoke();
    }

    void SummonSmoke()
    {
        if (currentDamageSmoke && transform.childCount<1)
        {
            GameObject summonSmoke = Instantiate(currentDamageSmoke,transform.position,Quaternion.identity,transform);
            summonSmoke.name = currentDamageSmoke.name;
        }
    }

    void CalculateSmokeToUse()
    {
        float healthPercentage = entity.health / entityMaxHealth;

        if (healthPercentage <= 0)
        {
            ChangeSmoke(deathSmoke);
        }
        else if (healthPercentage < 0.33f)
        {
            ChangeSmoke(heavyDamageSmoke);
        }
        else if (healthPercentage < 0.75f)
        {
            ChangeSmoke(lightDamageSmoke);
        }
    }

    void ChangeSmoke(GameObject o)
    {
        if (transform.childCount > 0 && transform.GetChild(0).gameObject.name != o.name) Destroy(transform.GetChild(0).gameObject);

        if (!currentDamageSmoke)
        {
            currentDamageSmoke = o;
        }
        else if(currentDamageSmoke.name!=o.name)
        {
            currentDamageSmoke = o;
        }
    }
}
