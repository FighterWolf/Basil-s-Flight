using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ObjectiveArrow : MonoBehaviour
{
    private GameObject currentRing;
    private Transform objectivePointer;
    private GameObject objectiveDistance;
    public GameObject objectiveToFollow;
    private Camera cam;
    private Transform pilotCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        pilotCanvas = GetComponent<HandlePlayer>().pilotCanvas.transform;
        objectivePointer = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ObjectivePointer");
        objectiveDistance = EssentialFunctions.FindDescendants(pilotCanvas.transform, "ObjectiveDistance").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        HandleObjectiveArrow();
    }
    void HandleObjectiveArrow()
    {
        Ring ring = FindFirstObjectByType<Ring>();
        currentRing = ring != null ? ring.gameObject : null;
        objectiveToFollow = objectiveToFollow == null ? currentRing : objectiveToFollow;

        if (objectiveToFollow != null)
        {
            objectivePointer.gameObject.SetActive(true);
            objectiveDistance.gameObject.SetActive(true);
            Vector3 objectivePositionInScreen = EssentialFunctions.TransformWorldCoordsToScreen(objectiveToFollow.transform.position, cam);
            float isBehind = objectivePositionInScreen.z > 0 ? 0 : 180;
            objectivePointer.localEulerAngles = new Vector3(0, 0, isBehind + Vector2.SignedAngle(Vector2.up, new Vector2(objectivePositionInScreen.x, objectivePositionInScreen.y)));
            if(objectiveToFollow.GetComponent<Ring>()!=null) objectiveDistance.GetComponent<TextMeshProUGUI>().text = "Objective Distance: " + EssentialFunctions.FindDescendants(objectiveToFollow.transform.parent, "Distance").GetComponent<TextMeshProUGUI>().text;
        }
        else
        {
            objectivePointer.gameObject.SetActive(false);
            objectiveDistance.gameObject.SetActive(false);
        }
    }
}
