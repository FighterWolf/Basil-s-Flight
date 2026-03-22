using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreen;
    static int selectedResolution;

    Resolution[] resolutions;

    public static bool isFullScreen=true;

    void Start()
    {
        PauseMenu.isGameOver = false;

        resolutions = Screen.resolutions;

        List<string> resolutionStrings = new List<string>();
        List<Resolution> filteredResolutions = new List<Resolution>();

        foreach (Resolution r in resolutions)
        {
            string res = r.width.ToString() + " x " + r.height.ToString();
            if (!resolutionStrings.Contains(res))
            {
                resolutionStrings.Add(res);
                filteredResolutions.Add(r);
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionStrings);

        resolutions = filteredResolutions.ToArray();

        ChangeIcon();
    }

    public void OnStart()
    {
        SceneManager.LoadScene("TestSceneFlight");
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

    void ChangeIcon()
    {
        fullScreen.isOn = isFullScreen;
        resolutionDropdown.value = selectedResolution;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                resolutionDropdown.value = i;
                break;
            }
        }
    }
}
