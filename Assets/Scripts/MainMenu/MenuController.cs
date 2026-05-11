using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreen;
    public TMP_Dropdown langaugeDropdown;
    static int selectedResolution;
    static int selectedLanguage;
    public static string currentLevel;

    Resolution[] resolutions;

    public static bool isFullScreen=true;

    public enum Language
    {
        English,
        ZhongWen,
        NihonGo
    }

    public static Language language;

    void Start()
    {
        if (currentLevel == null)
        {
            currentLevel = "TestSceneFlight";
        }
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
        SceneManager.LoadScene(currentLevel);
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
        langaugeDropdown.value = selectedLanguage;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                resolutionDropdown.value = i;
                break;
            }
        }
    }

    public void ChangeLanguage()
    {
        selectedLanguage = langaugeDropdown.value;
        language = (Language)selectedLanguage;
    }
}
