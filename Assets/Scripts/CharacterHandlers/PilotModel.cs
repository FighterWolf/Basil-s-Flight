using UnityEngine;

public class PilotModel : MonoBehaviour
{
    Animator anim;
    int animationSit;
    int animationGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
