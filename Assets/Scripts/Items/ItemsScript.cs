using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsScript : MonoBehaviour,IPickable
{
    [SerializeField] private ItemsSO itemData;
    [SerializeField] private GameObject graph;
    private Animator animator;

    public void InitItem(ItemsSO datas)
    {
        itemData = datas;
    }
    private void Awake()
    {
        this.animator = graph.GetComponent<Animator>();
    }

    public void OnPick(Entity entity)
    {
        switch (itemData.effect)
        {
            case ItemsSO.Effect.Heal:
                //change health ++
                entity.ChangeHealth(itemData.bonusAmountEffect);
                break;
            case ItemsSO.Effect.Coin:
                //add moula
                Debug.Log("plus d'argent let's goo");
                break;
            case ItemsSO.Effect.DamageBonus:
                //add dégâts bonus
                Debug.Log("Dégâts augmentés !");
                break;
        }
        StartCoroutine(PickAnim());

    }

    IEnumerator PickAnim()
    {
        this.animator.enabled = false;
        SpriteRenderer sprite = this.graph.GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 12;
        Color newcolor = new Color(1, 1, 1, 1);
        while (newcolor.a > 0.05f)
        {
            newcolor.a = newcolor.a - (230f / 255f) * Time.deltaTime * 1.2f ;
            sprite.color = newcolor;
            yield return null;
        }
        sprite.color = new Color(1, 1, 1, 0);

        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
