using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreen;

    int selectedResolution;

    Resolution[] resolutions;

    public bool isFullScreen=true;

    void Start()
    {
        resolutions = Screen.resolutions;

        List<string> resolutionStrings = new List<string>();
        List<Resolution> filteredResolutions = new List<Resolution>();
        HashSet<string> uniqueResolutions = new HashSet<string>();

        foreach (Resolution r in resolutions)
        {
            string res = r.width.ToString() + " x " + r.height.ToString();
            if (!uniqueResolutions.Contains(res))
            {
                uniqueResolutions.Add(res);
                resolutionStrings.Add(res);
                filteredResolutions.Add(r);
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionStrings);

        resolutions = filteredResolutions.ToArray();

        Resolution current = Screen.currentResolution;
        for(int i = 0; i < resolutions.Length; i++)
        {
            if(resolutions[i].width==current.width&& resolutions[i].height == current.height)
            {
                resolutionDropdown.value = i;
                break;
            }
        }
    }

    public void OnStart()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void ChangeResolution()
    {
        selectedResolution = resolutionDropdown.value;
        Screen.SetResolution(resolutions[selectedResolution].width, resolutions[selectedResolution].height,isFullScreen);
    }

    public void ChangeFullscreen()
    {
        isFullScreen = fullScreen.isOn;
        Screen.SetResolution(resolutions[selectedResolution].width, resolutions[selectedResolution].height, isFullScreen);
    }
}
