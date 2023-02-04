using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    //faire spawn des level aléatoirement avec une cohérence
    //Set up les entrées et sorties des levels


    private int numberOfLevel = 5;
    public int NumberOfLevel { get => numberOfLevel; }

    [SerializeField] private List<GameObject> levels = new List<GameObject>();

    public void SpawnLevel()
    {
        //faire spawn un level. donc avoir accès au prefab du level
    }
}
