using UnityEngine;

public class DialogueObject : MonoBehaviour,Interactable
{

    [System.Serializable]
    public struct DialogueLine
    {
        public string speakerNameEN;
        public string speakerNameZH;
        public string speakerNameJP;
        public string dialogueEN;
        public string dialogueZH;
        public string dialogueJP;
        public bool appearOnlyInFirstInteraction;
        public Sprite speakerPFP;
    }

    [SerializeField] public DialogueLine[] linesOfDialogue;
    public float timesInteractedWith;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(GameObject player)
    {
        GameObject db = EssentialFunctions.FindDescendants(GameObject.Find("Canvas").transform, "DialogueBox").gameObject;
        db.SetActive(true);
        Dialogue dialogueBox = db.GetComponent<Dialogue>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        dialogueBox.timesInteractedWith = timesInteractedWith;
        dialogueBox.AssignLines(this,linesOfDialogue);
        dialogueBox.StartDialogue(0);
    }

    public string GetName()
    {
        return "";
    }

    public bool IsHoldable()
    {
        return false;
    }

    public void OnRelease()
    {

    }
}
