using UnityEngine;

public class NPCAnimations : MonoBehaviour
{
    protected Animator anim;

    public float bodyWeight;
    public float headWeight;

    public float headWeightModifier;

    public float currentLookWeight;

    private Transform closestPlayerObject;
    private Vector3 lastLookPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        lastLookPos = transform.position + transform.forward * 2f + Vector3.up * 1.6f;
    }

    // Update is called once per frame
    void Update()
    {
        closestPlayerObject = FindClosestPlayer();
        TurnHeadTowardsObject();
    }

    Transform FindClosestPlayer()
    {
        Collider[] possiblePlayerColliders = Physics.OverlapSphere(transform.position,2f);
        float distance = Mathf.Infinity;
        Transform closestPlayer=null;
        foreach (Collider c in possiblePlayerColliders)
        {
            if(c.TryGetComponent<CharacterController>(out CharacterController player))
            {
                if (player == GetComponent<CharacterController>()) continue;
                
                float currentDistance = Vector3.Distance(player.transform.position, transform.position);
                if (currentDistance < distance)
                {
                    closestPlayer = player.transform;
                    distance = currentDistance;
                }
            }
        }
        return closestPlayer;
    }

    void TurnHeadTowardsObject()
    {
        float isPlayerNear = closestPlayerObject == null ? 0 : 1;
        currentLookWeight = Mathf.MoveTowards(currentLookWeight,isPlayerNear,2*Time.deltaTime);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (closestPlayerObject)
        {
            Transform head = EssentialFunctions.FindDescendants(closestPlayerObject, "Face");
            if (head != null && !PauseMenu.isPaused)
            {
                //anim.SetLookAtPosition(head.position + Vector3.up * headWeightModifier);
                lastLookPos = head.position + Vector3.up * headWeightModifier;
            }
        }
        anim.SetLookAtPosition(lastLookPos);
        anim.SetLookAtWeight(currentLookWeight, bodyWeight, headWeight);
    }
}
