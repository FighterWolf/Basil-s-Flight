using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    public GameObject levelCompleteHUD;

    public float numberOfKillsNeeded;
    public float numberOfRingsToFlyThrough;

    public float currentKillPoints;
    public float currentRingPoints;

    public HandlePlayer player;

    public static bool isLevelComplete;

    public string nextLevel;
    public static string currentLevelStoryMode;

    public bool isLevelCheckpoint;
    public static string currentCheckpoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isLevelComplete = false;
        if (isLevelCheckpoint)
        {
            currentCheckpoint = currentLevelStoryMode;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentKillPoints = player.currentKillPoints;
        currentRingPoints = player.currentRingPoints;
        CompletionCheck();
    }

    public void LoadNextLevel()
    {
        if(nextLevel!=null) SceneManager.LoadScene(nextLevel);
    }

    public void GoToLastCheckpoint()
    {
        if(currentCheckpoint!=null) SceneManager.LoadScene(currentCheckpoint);
    }

    public void CompletionCheck()
    {
        if (currentKillPoints >= numberOfKillsNeeded && currentRingPoints >= numberOfRingsToFlyThrough)
        {
            levelCompleteHUD.SetActive(true);
            EssentialFunctions.GameOver();
            isLevelComplete = true;
        }
    }
}
