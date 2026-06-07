using UnityEngine;
using UnityEngine.Localization;

public class DialogueObject : MonoBehaviour,Interactable
{

    [System.Serializable]
    public struct DialogueLine
    {
        public LocalizedString localizedSpeakerName;
        public LocalizedString localizedDialogue;
        public bool appearOnlyInFirstInteraction;
        public Sprite speakerPFP;
    }

    [SerializeField] public DialogueLine[] linesOfDialogue;
    public float timesInteractedWith;

    public bool deleteObjectAfterDialogue;
    private bool isTriggerEnter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TryGetComponent<Collider>(out Collider c) && c.isTrigger) isTriggerEnter = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(GameObject player)
    {
        if(!isTriggerEnter) StartDialogue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<HandlePlayer>(out HandlePlayer player))
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        GameObject db = EssentialFunctions.FindDescendants(GameObject.Find("Canvas").transform, "DialogueBox").gameObject;
        db.SetActive(true);
        Dialogue dialogueBox = db.GetComponent<Dialogue>();
        if (dialogueBox.isSceneCutscene)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        dialogueBox.timesInteractedWith = timesInteractedWith;
        dialogueBox.AssignLines(this, linesOfDialogue);
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
