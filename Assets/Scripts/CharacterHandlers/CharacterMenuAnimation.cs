using UnityEngine;

public class CharacterMenuAnimation : PilotModel
{
    public bool isSitting;

    public float bodyWeight;
    public float headWeight;

    public Transform lookAtObject;

    public override void Start()
    {
        base.Start();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetLookAtPosition(lookAtObject.position);
        anim.SetLookAtWeight(1, bodyWeight, headWeight);
    }
}
