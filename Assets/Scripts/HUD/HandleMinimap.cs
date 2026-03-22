using UnityEngine;

public class HandleMinimap : MonoBehaviour
{

    public Transform player;

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
        transform.position = new Vector3(player.position.x,5000,player.position.z);
        transform.rotation.Normalize();
        transform.rotation = Quaternion.Euler(90, player.rotation.eulerAngles.y, 0);
    }
}
