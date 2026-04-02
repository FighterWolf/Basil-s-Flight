using UnityEngine;
using System.Collections.Generic;

public class Ring : MonoBehaviour
{
    public GameObject collectedParticle;

    public Ring ringToActivateOnCollect;
    public bool doNotAddPoint;

    private bool isCollected;
    private Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.allCameras[0];
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion cameraAngle = cam.transform.rotation;
        Vector3 angle = cameraAngle.eulerAngles;
        angle.z = 0;
        transform.rotation = cameraAngle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent<HandlePlayer>(out HandlePlayer player))
        {
            if (isCollected) return;
            
            if(!LevelHandler.isLevelComplete && !doNotAddPoint)player.currentRingPoints++;

            if (collectedParticle)
            {
                isCollected = true;
                GameObject particle = Instantiate(collectedParticle, transform.position, Quaternion.identity);
            }

            if (ringToActivateOnCollect) ringToActivateOnCollect.transform.root.gameObject.SetActive(true);

            Destroy(transform.root.gameObject);
        }
    }
}
