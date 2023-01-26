using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public MainCharacterScript player;

    public void InitPlayer(MainCharacterScript player)
    {
        this.player = player;
    }

    
}