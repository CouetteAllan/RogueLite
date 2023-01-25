using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Entity player;

    public void InitPlayer()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
    }


}