using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class KillLog : MonoBehaviour
{
    private List<string> activeMessages = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform t in transform)
        {
            if (t.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI textBox))
            {
                textBox.text = "";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMessage(string text)
    {
        activeMessages.Insert(0,text);

        SetKillLog();
    }

    void SetKillLog()
    {
        int iterator = 0;
        foreach(Transform t in transform)
        {
            if(t.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI textBox))
            {
                if (activeMessages.Count < iterator+1)
                {
                    break;
                }
                textBox.text = activeMessages[iterator];
            }
            iterator++;
        }
    }
}
