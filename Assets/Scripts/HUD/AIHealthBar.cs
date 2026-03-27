using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIHealthBar : ObjectiveDistance
{
    private Image healthBar;
    public Color healthBarColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        healthBar = EssentialFunctions.FindDescendants(transform, "HealthBar").GetComponent<Image>();
        healthBar.color = healthBarColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void LateUpdate()
    {
        if (entity.health > 0 && !entity.isDisabled)
        {
            base.LateUpdate();
            float alpha = Mathf.InverseLerp(100, 250, distance);
            alpha = Mathf.Clamp(alpha, 0.1f, 1);
            healthBar.fillAmount = entity.health / entity.maxHealth;
            healthBar.color = new Color(healthBarColor.r, healthBarColor.g, healthBarColor.b, alpha);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}