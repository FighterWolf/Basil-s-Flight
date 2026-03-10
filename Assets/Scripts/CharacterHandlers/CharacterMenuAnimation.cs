using UnityEngine;

public class CharacterLookAt : MonoBehaviour
{
    public bool isSitting;

    public float bodyWeight;
    public float headWeight;

    public Transform lookAtObject;
    private Animator anim;
    int animationSit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        animationSit = Animator.StringToHash("Sitting");
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool(animationSit,isSitting);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetLookAtPosition(lookAtObject.position);
        anim.SetLookAtWeight(1, bodyWeight, headWeight);
    }
}
