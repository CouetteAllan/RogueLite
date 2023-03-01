using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehaviour : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private EnemyEntity3D enemy;
    private float enemyHealth;
    private float enemyMaxHealth;
    private Coroutine fillBarCoroutine;


    public void SetMaxHealth(float health, EnemyEntity3D _enemy)
    {
        slider.maxValue = health;
        enemyMaxHealth = health;
        enemyHealth = health;
        this.enemy = _enemy;
        slider.gameObject.SetActive(false);
        enemy.OnChangeHealth += SetHealth;

    }

    public void SetHealth(float health)
    {
        if(!slider.gameObject.activeInHierarchy)
            slider.gameObject.SetActive(true);
        if (fillBarCoroutine != null)
            StopCoroutine(fillBarCoroutine);
        fillBarCoroutine = StartCoroutine(LerpHealthBar(health, slider.value, 0.3f));
    }

    IEnumerator LerpHealthBar(float targetFill, float start, float timeToMove)
    {
        float t = 0;
        while (t < 1)
        {
            slider.value = Mathf.Lerp(start, targetFill, t);
            t = t + Time.deltaTime / timeToMove;
            yield return null;
        }
        slider.value = targetFill;
    }

    private void OnDisable()
    {
        enemy.OnChangeHealth -= SetHealth;

    }
}
