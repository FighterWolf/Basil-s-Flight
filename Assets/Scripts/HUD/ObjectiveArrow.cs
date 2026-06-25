using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ObjectiveArrow : MonoBehaviour
{
    public List<GameObject> objectives = new List<GameObject>();

    protected Transform objectivePointer;
    protected Transform objectiveArrow;
    protected GameObject objectiveDistance;
    public GameObject objectiveToFollow;
    protected Camera cam;
    protected Transform pilotCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        cam = Camera.main != null ? Camera.main : cam = EssentialFunctions.FindDescendants(transform, "Camera").GetComponent<Camera>() ;
        pilotCanvas = GetComponent<HandlePlayer>().pilotCanvas.transform;
        objectivePointer = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ObjectivePointer");
        objectiveArrow = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ObjectiveArrow");
        objectiveDistance = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ObjectiveDistance").gameObject;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        FindObjective();
        HandleObjectiveArrow();
    }

    public void FindObjective()
    {
        foreach(GameObject o in objectives)
        {
            if (o == null) continue;
            
            if (o.activeInHierarchy && (o.GetComponent<DialogueObject>()==null || (o.TryGetComponent<DialogueObject>(out DialogueObject dialogueObj) && dialogueObj.timesInteractedWith<1)))
            {
                objectiveToFollow = o;
            }
        }
    }

    public void HandleObjectiveArrow()
    {
        if (objectiveToFollow != null)
        {
            objectivePointer.gameObject.SetActive(true);
            objectiveDistance.gameObject.SetActive(true);

            Vector3 objectivePositionInScreen = EssentialFunctions.TransformWorldCoordsToScreen(objectiveToFollow.transform.position, cam);
            float isBehind = objectivePositionInScreen.z < 0 ? -1 : 1;

            Vector2 target = new Vector2(objectivePositionInScreen.x, objectivePositionInScreen.y)*isBehind;

            float radius=Screen.height/4;

            bool isOffScreen = target.magnitude > radius;//objectivePositionInScreen.x> radius || objectivePositionInScreen.x < -radius || objectivePositionInScreen.y > radius || objectivePositionInScreen.y < -radius;

            if (!isOffScreen && isBehind>0)
            {
                objectivePointer.localPosition = new Vector3(objectivePositionInScreen.x, objectivePositionInScreen.y, 0);
                objectiveArrow.gameObject.SetActive(false);
            }
            else
            {
                objectivePointer.localPosition = target.normalized * radius;
                objectiveArrow.gameObject.SetActive(true);
                objectiveArrow.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, target.normalized));
            }

            if (objectiveToFollow)
            {
                objectiveDistance.GetComponent<TextMeshProUGUI>().text = objectiveToFollow.GetComponentInChildren<ObjectiveDistance>().distanceTextString;
            }
        }
        else
        {
            objectivePointer.gameObject.SetActive(false);
            objectiveDistance.gameObject.SetActive(false);
        }
    }
}
