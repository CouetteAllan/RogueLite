using UnityEngine;

public enum GameState
{
    MainMenu,
    InGame,
    InSelect,
    InHub,
    Pause,
}

public class GameManager : Singleton<GameManager>
{
    public MainCharacterScript player;
    private int enemyCountOnScreens = 0;
    private int coins = 0;



    private GameState actualGameState = GameState.InGame;
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
                    Time.timeScale = 1.0f;
                    UIManager.Instance.SetActiveMenu(false, actualGameState);
                    InputManager.playerInputAction.Player.Enable();
                    InputManager.playerInputAction.UI.Disable();
                    break;
                case GameState.InHub:
                    Debug.Log("In Hub Mode");

                    break;
                case GameState.Pause:
                    Debug.Log("In Pause Mode");
                    UIManager.Instance.SetActiveMenu(true, actualGameState);
                    Time.timeScale = 0.05f;
                    break;
                case GameState.MainMenu:
                    Debug.Log("In MainMenu Mode");

                    break;
                case GameState.InSelect:
                    Debug.Log("In Select Mode");
                    Time.timeScale = 0.05f;
                    UIManager.Instance.SetActiveMenu(true, actualGameState);
                    break;
            }
        }
    }
    protected override void Awake()
    {
        base.Awake();
        InitEvents();
        ActualGameState = GameState.InGame;
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