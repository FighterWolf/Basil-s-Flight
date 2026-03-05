using UnityEngine;

public class HandlePlayerDeath : MonoBehaviour
{
    public GameObject gameOverScreen;

    public Entity player;

    public static bool isPlayerDead;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TryGetComponent<Entity>(out Entity e)) player = e;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDisabled)
        {
            gameOverScreen.SetActive(true);
            isPlayerDead = true;
        }
    }
}
