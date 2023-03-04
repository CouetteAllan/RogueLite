using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUpScript : MonoBehaviour
{
    private TextMeshPro textMeshPro;

    [SerializeField]
    private AnimationCurve scaleCurve;
    [SerializeField]
    private AnimationCurve positionCurve;

    private void Awake()
    {
        textMeshPro = this.GetComponent<TextMeshPro>();
    }

    public void SetUp(int value)
    {
        textMeshPro.text = value.ToString();
    }

    public void StartAnimation()
    {
        StartCoroutine(Moving());
    }

    public void SetColorText(Color color)
    {
        textMeshPro.color = color;
    }

    IEnumerator Moving()
    {
        Vector3 pos = this.transform.position;
        Vector3 scale = this.transform.localScale;
        float baseTime = 1.2f;
        float t = baseTime;
        float timeCurve = 0f;
        float randomPosX = Random.Range(-1f, 1f);
        while (t > 0)
        {
            pos.y += 0.7f * Time.deltaTime;
            pos.x += randomPosX * Time.deltaTime;
            scale.x = scaleCurve.Evaluate(timeCurve / baseTime);
            scale.y = scaleCurve.Evaluate(timeCurve / baseTime);
            this.transform.position = pos;
            this.transform.localScale = scale;
            yield return null;
            t -= Time.deltaTime;
            timeCurve += Time.deltaTime;
        }
        //Destroy(this);
        this.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
