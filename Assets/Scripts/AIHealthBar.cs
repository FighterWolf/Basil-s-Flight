using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIHealthBar : MonoBehaviour
{
    private Camera cam;
    private float distance;
    private TMP_Text distanceText;
    private Image healthBar;
    public Color healthBarColor;
    public Color distanceTextColor;
    private Entity entity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.allCameras[0];
        entity = transform.root.GetComponent<Entity>();
        distanceText = EssentialFunctions.FindDescendants(transform,"Distance").GetComponent<TMP_Text>();
        healthBar = EssentialFunctions.FindDescendants(transform, "HealthBar").GetComponent<Image>();
        distanceText.color = distanceTextColor;
        healthBar.color = healthBarColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (entity.health > 0 && !entity.isDisabled)
        {
            distance = Vector3.Distance(transform.position, cam.transform.position);
            distanceText.text = distance.ToString("F2") + "m";

            healthBar.fillAmount = entity.health / entity.maxHealth;

            float alpha = Mathf.InverseLerp(100,250,distance);
            alpha=Mathf.Clamp(alpha,0.1f,1);
            healthBar.color = new Color(healthBarColor.r, healthBarColor.g, healthBarColor.b, alpha);
            distanceText.color = new Color(distanceTextColor.r, distanceTextColor.g, distanceTextColor.b, alpha);
            transform.localScale = Vector3.one * Mathf.Clamp(distance * 0.05f, 1f, 20);
            //transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
            Quaternion cameraAngle = cam.transform.rotation;
            Vector3 angle = cameraAngle.eulerAngles;
            angle.z=0;
            transform.rotation = cameraAngle;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}