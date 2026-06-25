using UnityEngine;
using System.Collections;

public class DialogueStarter : DialogueObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
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

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        StartDialogue();
    }
}
