using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public MainCharacterScript player;
    private int enemyCountOnScreens = 0;
    private int coins = 0;

    public enum GameState
    {
        MainMenu,
        InGame,
        InHub,
        Pause,
    }

    private GameState actualGameState;
    public GameState ActualGameState
    {
        get => actualGameState;
        set
        {
            actualGameState = value;
            switch (value)
            {
                case GameState.InGame:
                    Debug.Log("In Game Mode");
                    break;
                case GameState.InHub:
                    Debug.Log("In Hub Mode");

                    break;
                case GameState.Pause:
                    Debug.Log("In Pause Mode");

                    Time.timeScale = 0;
                    break;
                case GameState.MainMenu:
                    Debug.Log("In MainMenu Mode");

                    break;
            }
        }
    }
    protected override void Awake()
    {
        
    }
    private void InitEvents()
    {

    }

    public void InitPlayer(MainCharacterScript player)
    {
        this.player = player;
    }

    public MainCharacterScript GetPlayer()
    {
        if (player == null)
            return null;
        return player;
    }

    public void PlayerDeath()
    {
        player = null;
    }
}