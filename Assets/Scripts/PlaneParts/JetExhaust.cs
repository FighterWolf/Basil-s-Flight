using UnityEngine;

public class JetExhaust : MonoBehaviour
{
    public GameObject exhaustSmoke;
    private float speed;
    private float maxSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exhaustSmoke = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        speed = transform.root.GetComponent<Aircraft>().speed;
        maxSpeed = transform.root.GetComponent<Aircraft>().maxSpeed;
        HandleSmoke();
    }
    void HandleSmoke()
    {
        if ((speed/maxSpeed)>0.25)
        {
            exhaustSmoke.SetActive(true);
            float scale = speed>maxSpeed ? 1 : speed / maxSpeed;
            exhaustSmoke.transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            exhaustSmoke.SetActive(false);
        }
    }
}
