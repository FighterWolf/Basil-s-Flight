using UnityEngine;

public class PilotModel : MonoBehaviour
{
    protected Animator anim;
    protected int animationSit;
    protected int animationGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        anim = GetComponent<Animator>();
        animationSit= Animator.StringToHash("Sitting");
        animationGrounded= Animator.StringToHash("Grounded");
        anim.SetBool(animationGrounded, false);
        anim.SetBool(animationSit, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
