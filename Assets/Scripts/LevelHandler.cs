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

    public bool mustExitThroughRing;
    public GameObject exitRing;
    public GameObject finalInstruction;
    private bool isExitRingGenerated;

    public bool isStory;

    public bool isEndless;

    public static bool isLevelComplete;

    public string nextLevel;

    public bool isLevelCheckpoint;
    public static string currentCheckpoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isLevelComplete = false;
        if (isLevelCheckpoint)
        {
            currentCheckpoint = SceneManager.GetActiveScene().name;
            MenuController.currentLevel = currentCheckpoint;
        }
        finalInstruction = mustExitThroughRing ? EssentialFunctions.FindDescendants(transform, "MustGoThroughExitRing").gameObject : null;
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
        if (currentKillPoints >= numberOfKillsNeeded && currentRingPoints >= numberOfRingsToFlyThrough && !isEndless)
        {
            if (mustExitThroughRing)
            {
                if (!isExitRingGenerated)
                {
                    exitRing.SetActive(true);
                    isExitRingGenerated = true;
                    finalInstruction.SetActive(true);
                }
                if (exitRing == null)
                {
                    GameOver();
                }
            }
            else
            {
                GameOver();
            }
        }
    }

    public void GameOver()
    {
        if (finalInstruction != null) finalInstruction.SetActive(false);
        levelCompleteHUD.SetActive(true);
        EssentialFunctions.GameOver();
        isLevelComplete = true;
    }
}
