using UnityEngine;

public class BuildingOverride : MonoBehaviour
{
    public bool isNight;
    MeshRenderer r;
    MaterialPropertyBlock mpb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        r = GetComponent<MeshRenderer>();
        mpb = new MaterialPropertyBlock();

        r.GetPropertyBlock(mpb);

        if (isNight)
        {
            mpb.SetColor("_EmissionColor", Color.white * 3f);
        }
        else
        {
            mpb.SetColor("_EmissionColor", Color.black);
        }

        r.SetPropertyBlock(mpb);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
