using UnityEngine;
using StarterAssets;

public class CanvasHolder : MonoBehaviour
{

    public GameObject canvas;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TryGetComponent<ThirdPersonController>(out ThirdPersonController tps))
        {
            if (tps.currentVehicle)
            {
                canvas.SetActive(true);
            }
            else
            {
                canvas.SetActive(false);
            }
        }
    }
}
