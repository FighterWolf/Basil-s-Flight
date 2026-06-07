using UnityEngine;

public class ObjectFaceCamera : MonoBehaviour
{

    protected Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        cam = Camera.allCameras[0];
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Quaternion cameraAngle = cam.transform.rotation;
        Vector3 angle = cameraAngle.eulerAngles;
        angle.z = 0;
        transform.rotation = cameraAngle;
    }
}
