using UnityEngine;

public class HandleAlly : MonoBehaviour
{
    public bool isStoryMode;
    public Entity player;
    public PlaneWeaponSystem pws;

    public virtual void Awake()
    {
        if (TryGetComponent<Entity>(out Entity e)) player = e;
        if (TryGetComponent<PlaneWeaponSystem>(out PlaneWeaponSystem pws)) this.pws = pws;
        if (!isStoryMode) SummonAircraft();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SummonPlayerModel()
    {
        GameObject model = Instantiate(PlaneSelector.pilotToSummon, transform, false);
        Instantiate(model.GetComponent<PilotModel>().aircraft, transform, false);
        transform.root.GetComponent<Entity>().killCreditName = model.GetComponent<PilotModel>().characterName;
    }

    public void SummonAircraft()
    {
        if (this.GetType() == typeof(HandlePlayer)) Instantiate(PlaneSelector.planeToSummon, transform, false);
        if (this is not HandlePlayer) SummonPlayerModel();
        player.Start();
        pws.Start();
    }
}
