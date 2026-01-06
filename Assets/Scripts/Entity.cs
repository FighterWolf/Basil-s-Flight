using UnityEngine;

public class Entity : MonoBehaviour
{
    public float health;
    public float maxHealth;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
