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
        if(plane.speed>0)transform.Rotate(Vector3.up*plane.speed*20*Time.deltaTime);
    }
}
