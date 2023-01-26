using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public MainCharacterScript player;

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