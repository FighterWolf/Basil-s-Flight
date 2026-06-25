using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveDistance : MonoBehaviour
{
    protected Camera cam;
    protected float distance;
    protected TMP_Text distanceText;
    public string distanceTextString;
    public Color distanceTextColor;
    protected Entity entity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        cam = Camera.allCameras[0];
        entity = transform.root.GetComponent<Entity>();

        if (EssentialFunctions.FindDescendants(transform, "Distance") is Transform t && t.TryGetComponent<TMP_Text>(out TMP_Text tmpText))
        {
            distanceText = tmpText;
            distanceText.color = distanceTextColor;
        }

        if (TryGetComponent<Entity>(out Entity e)) entity = e;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void LateUpdate()
    {
         distance = Vector3.Distance(transform.position, cam.transform.position);
        distanceTextString = distance.ToString("F2") + "m";
        if (distanceText) distanceText.text = distanceTextString;
        float alpha = Mathf.InverseLerp(100, 250, distance);
        alpha = Mathf.Clamp(alpha, 0.1f, 1);

        if (distanceText)
        {
            distanceText.color = new Color(distanceTextColor.r, distanceTextColor.g, distanceTextColor.b, alpha);
            transform.localScale = Vector3.one * Mathf.Clamp(distance * 0.05f, 0.75f, 20);
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        }
        Quaternion cameraAngle = cam.transform.rotation;
        Vector3 angle = cameraAngle.eulerAngles;
        angle.z = 0;
        transform.rotation = cameraAngle;
    }
}
