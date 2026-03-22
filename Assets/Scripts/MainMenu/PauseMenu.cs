using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PauseMenu : MonoBehaviour
{

    public InputActionAsset userInput;
    private InputAction pause;

    public GameObject pauseMenu;

    public static bool isGameOver;
    public static bool isPaused;

    void Start()
    {
        userInput.FindActionMap("GameSystem").Enable();
        pause = userInput.FindAction("Pause");
        HandlePlayer.isPlayerDead = false;
        Resume();
        isGameOver = false;
    }

    void Update()
    {
        if (pause.WasPressedThisFrame()&&!HandlePlayer.isPlayerDead)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        if (HandlePlayer.isPlayerDead)
        {
            EssentialFunctions.GameOver();
        }
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("Mainmenu");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        foreach(Transform t in pauseMenu.transform.parent)
        {
            t.gameObject.SetActive(false);
        }
        isPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        isPaused = true;
        AudioSource[] allAudio = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach(AudioSource a in allAudio)
        {
            a.Stop();
        }
    }
}
