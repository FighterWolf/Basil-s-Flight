using UnityEngine;

public class VFormationSpot : MonoBehaviour
{
    public Aircraft whoTakesTheSpot;

    public enum LeftORRight
    {
        Left,
        Right
    }

    public LeftORRight spot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (whoTakesTheSpot==null)
        {
            whoTakesTheSpot = null;
        }
    }
}
