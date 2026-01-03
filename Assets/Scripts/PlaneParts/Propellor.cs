using UnityEngine;

public class Propellor : MonoBehaviour
{
    private Aircraft plane;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plane = transform.root.GetComponent<Aircraft>();   
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,plane.speed*5,0);
    }
}
