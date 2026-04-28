using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ObjectiveArrow : MonoBehaviour
{
    protected GameObject currentRing;
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
        HandleObjectiveArrow();
    }
    public void HandleObjectiveArrow()
    {
        Ring ring = FindFirstObjectByType<Ring>();
        currentRing = ring != null ? ring.gameObject : null;
        objectiveToFollow = objectiveToFollow == null ? currentRing : objectiveToFollow;

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

            if(objectiveToFollow.GetComponent<Ring>()!=null) objectiveDistance.GetComponent<TextMeshProUGUI>().text = "Objective Distance: " + EssentialFunctions.FindDescendants(objectiveToFollow.transform.parent, "Distance").GetComponent<TextMeshProUGUI>().text;
        }
        else
        {
            objectivePointer.gameObject.SetActive(false);
            objectiveDistance.gameObject.SetActive(false);
        }
    }
}
