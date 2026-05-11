using UnityEngine;

public class HandlePlayer : HandleAlly
{
    public GameObject gameOverScreen;
    public Canvas pilotCanvas;

    public float currentKillPoints;
    public float currentRingPoints;

    public static bool isPlayerDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        currentKillPoints = 0;
        currentRingPoints = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDisabled)
        {
            gameOverScreen.SetActive(true);
            isPlayerDead = true;
        }

        if (PauseMenu.isGameOver)
        {
            if(player is Aircraft a)
            {
                PlaneWeaponSystem pws = a.GetComponent<PlaneWeaponSystem>();
                pws.fire=false;
                pws.flare = false;
                pws.switchWeapon = false;
            }
        }

        if (LevelHandler.isLevelComplete)
        {
            ResetPlayer();
        }
    }

    public void ResetPlayer()
    {
        if(player is Aircraft a)
        {
            a.Patrol();
        }
    }
}
