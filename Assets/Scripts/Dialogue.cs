using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    private TextMeshProUGUI dialogueText;
    private TextMeshProUGUI characterName;
    private Image characterPortrait;
    private bool isTextDone;

    public static bool isInDialogue;

    private DialogueObject.DialogueLine[] currentLines;
    private DialogueObject dialogueInteractable;

    public bool isSceneCutscene;
    public float timesInteractedWith;
    public int iterator = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        dialogueText = EssentialFunctions.FindDescendants(canvas,"DialogueText").GetComponent<TextMeshProUGUI>();
        characterName = EssentialFunctions.FindDescendants(canvas, "CharacterName").GetComponent<TextMeshProUGUI>();
        characterPortrait = EssentialFunctions.FindDescendants(canvas, "CharacterPortrait").GetComponent<Image>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignLines(DialogueObject dialogueInteractable, DialogueObject.DialogueLine[] lines)
    {
        this.dialogueInteractable = dialogueInteractable;
        currentLines = lines;
    }

    public void StartDialogue(int iterator)
    {
        isInDialogue = true;
        if (iterator<currentLines.Length)
        {
            if (timesInteractedWith > 0 && currentLines[iterator].appearOnlyInFirstInteraction)
            {
                this.iterator++;
                StartDialogue(this.iterator);
            }
            else
            {
                characterName.text = currentLines[iterator].speakerName;
                if (characterPortrait != null) characterPortrait.sprite = currentLines[iterator].speakerPFP;
                dialogueText.text = "";
                StartCoroutine(Type(currentLines[iterator].dialogue));
            }
        }
        else
        {
            MoveToNextText();
        }
    }

    public IEnumerator Type(string line)
    {
        isTextDone = false;
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.005f);
        }
        isTextDone = true;
        if (!isSceneCutscene)
        {
            yield return new WaitForSeconds(1.5f);
            MoveToNextText();
        }
    }

    public void MoveToNextText()
    {
        if (iterator >= currentLines.Length)
        {
            iterator = 0;
            currentLines = null;
            timesInteractedWith = 0;
            if (isSceneCutscene)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            isInDialogue = false;
            dialogueInteractable.timesInteractedWith++;
            dialogueInteractable = null;
            gameObject.SetActive(false);
        }
        else
        {
            if (isTextDone)
            {
                iterator++;
                StartDialogue(iterator);
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = currentLines[iterator].dialogue;
                isTextDone=true;
            }
        }
    }

    public void ResetText()
    {
        dialogueText.text = "";
    }
}
