using UnityEngine;
using System.Collections;

public class DialogueStarter : DialogueObject
{
    bool enabledDialogueComplete;

    public GameObject[] listOfGameObjectsToEnable;
    public MonoBehaviour[] listOfMonoBehaviorsToEnable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {
        if (timesInteractedWith > 0 && !enabledDialogueComplete)
        {
            EnableAtTheEndOfDialogue();
            enabledDialogueComplete = true;
        }
    }

    public void StartDialogue()
    {
        GameObject db = EssentialFunctions.FindDescendants(GameObject.Find("Canvas").transform, "DialogueBox").gameObject;
        db.SetActive(true);
        Dialogue dialogueBox = db.GetComponent<Dialogue>();
        dialogueBox.timesInteractedWith = timesInteractedWith;
        dialogueBox.AssignLines(this, linesOfDialogue);
        dialogueBox.StartDialogue(0);
    }

    public void EnableAtTheEndOfDialogue()
    {
        foreach (GameObject o in listOfGameObjectsToEnable)
        {
            o.SetActive(true);
        }
        foreach (MonoBehaviour mb in listOfMonoBehaviorsToEnable)
        {
            mb.enabled = true;
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        StartDialogue();
    }
}
