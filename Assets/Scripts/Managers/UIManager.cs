using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{

    [SerializeField] private Slider h_Slider;
    [SerializeField] private Gradient h_Gradient;
    [SerializeField] private Image h_Fill;
    [SerializeField] private TextMeshProUGUI h_Txt;
    private float playerHealth;
    private float playerMaxHealth;
    private Coroutine fillBarCoroutine;
    private Coroutine txtCoroutine;

    private MainCharacterScript player = null;
    public void SetMaxHealth(float health, MainCharacterScript player)
    {
        h_Slider.maxValue = health;
        playerMaxHealth = health;
        playerHealth = health;
        h_Fill.color = h_Gradient.Evaluate(1);
        this.player = player;
        player.OnPlayerChangeHealth += SetUIHealth;

    }

    public void SetUIHealth(float health)
    {

        //h_Slider.value = health;
        if (fillBarCoroutine != null)
            StopCoroutine(fillBarCoroutine);
        fillBarCoroutine = StartCoroutine(LerpHealthBar(health,h_Slider.value,0.5f));

        if (txtCoroutine != null)
            StopCoroutine(txtCoroutine);
        fillBarCoroutine = StartCoroutine(LerpHealthTxt(health,playerHealth,1f));


        h_Fill.color = h_Gradient.Evaluate(h_Slider.normalizedValue);

    }


    IEnumerator LerpHealthBar(float targetFill, float start,float timeToMove)
    {
        float t = 0;
        while (t < 1)
        {
            h_Slider.value = Mathf.Lerp(start, targetFill, t);
            t = t + Time.deltaTime / timeToMove;
            yield return null;
        }
        h_Slider.value = targetFill;
    }
    
    IEnumerator LerpHealthTxt(float target, float start, float timeToMove)
    {
        float t = 0;
        while (t < 1)
        {
            playerHealth = Mathf.Lerp(start, target, t);
            h_Txt.text = playerHealth.ToString("#") + "/" + playerMaxHealth.ToString("#");
            t = t + Time.deltaTime / timeToMove;
            yield return null;
        }
        playerHealth = target;
    }

}
