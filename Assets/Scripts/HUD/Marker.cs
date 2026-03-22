using UnityEngine;

public class Marker : MonoBehaviour
{
    public bool isPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.rotation.Normalize();
        transform.rotation = Quaternion.Euler(90, transform.root.eulerAngles.y, 0);
    }
}
