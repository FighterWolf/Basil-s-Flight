using UnityEngine;
using UnityEngine.Localization;

public class PilotModel : MonoBehaviour
{
    public LocalizedString localizedCharacterName;
    public string characterName;
    public PlaneStats aircraft;
    protected Animator anim;
    protected int animationSit;
    protected int animationGrounded;
    Transform plane;
    Transform seat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        anim = GetComponent<Animator>();
        animationSit= Animator.StringToHash("Sitting");
        animationGrounded= Animator.StringToHash("Grounded");
        anim.SetBool(animationGrounded, false);
        anim.SetBool(animationSit, true);
        plane = transform.root;
        seat = EssentialFunctions.FindDescendants(plane,"Seat");
        transform.SetParent(seat);
        if(aircraft) transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
