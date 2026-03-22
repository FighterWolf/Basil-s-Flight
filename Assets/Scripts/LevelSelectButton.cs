using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    public string internalSceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    public void GoToScene()
    {
        if (internalSceneName != null) SceneManager.LoadScene(internalSceneName);
    }
}
