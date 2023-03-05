using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsScript : MonoBehaviour,IPickable3D, IInteractable
{
    [SerializeField] private ItemsSO itemData;
    [SerializeField] private GameObject graph;
    private Animator animator;
    private bool pickedUp = false;

    public Transform interactTransform => this.transform;

    public void InitItem(ItemsSO datas)
    {
        itemData = datas;
    }
    private void Awake()
    {
        this.animator = graph.GetComponent<Animator>();
        animator.runtimeAnimatorController = itemData.animator;
        this.graph.GetComponent<SpriteRenderer>().sprite = itemData.sprite;
    }

    public void OnPick(MainCharacterScript3D entity)
    {
        if (!pickedUp)
        {
            itemData.DoEffect(entity);
            StartCoroutine(PickAnim());
            pickedUp = true;
        }
        
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

    public void OnInteract(MainCharacterScript3D player)
    {
        OnPick(player);
    }

    public BonusSO GetItemData()
    {
        return this.itemData as BonusSO;
    }

}
