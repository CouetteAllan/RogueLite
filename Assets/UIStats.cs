using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStats : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup groupOfStats;
    private TextMeshProUGUI[] statTexts;

    void Start()
    {
        statTexts = groupOfStats.GetComponentsInChildren<TextMeshProUGUI>();
    }
}
