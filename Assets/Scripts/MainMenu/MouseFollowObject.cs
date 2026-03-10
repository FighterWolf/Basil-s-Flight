using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollowObject : MonoBehaviour
{
    Camera mainCam;
    Mouse currentMouse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main;
        currentMouse = Mouse.current;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = currentMouse.position.ReadValue();
        mousePosition.z = 0.5f;
        Vector3 screenToWorldPosition = mainCam.ScreenToWorldPoint(mousePosition);
        transform.position = screenToWorldPosition;
    }
}
