using UnityEngine;
using System.Collections.Generic;

public class Entity : MonoBehaviour
{   
    public static List<Entity> bluForEntity = new List<Entity>();
    public static List<Entity> opForEntity = new List<Entity>();

    public float health;
    public float maxHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.CompareTag("BluFor"))
        {
            bluForEntity.Add(this);
        }else if (gameObject.CompareTag("OpFor"))
        {
            opForEntity.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        OnZeroHealth();
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
        }
    }

    public void DecreaseHealth(float health)
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
}
