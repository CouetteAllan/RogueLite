using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUpScript : MonoBehaviour
{
    private TextMeshPro textMeshPro;

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

    IEnumerator Moving()
    {
        Vector3 pos = this.transform.position;
        float t = 1.2f;
        while (t > 0)
        {
            pos.y += 1f * Time.deltaTime;
            this.transform.position = pos;
            //move text
            yield return null;
            t -= Time.deltaTime;
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
