using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemInfoUI : MonoBehaviour
{
    [SerializeField] private ItemsScript item;
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI descritpion;

    private void Awake()
    {
        item = GetComponent<ItemsScript>();
        SetItemText();
        container.SetActive(false);
        MainCharacterScript3D.OnInteractSelectChange += Player_OnInteractSelectChange;
    }

    private void Player_OnInteractSelectChange(IInteractable interact)
    {
        if (item == (ItemsScript)interact)
        {
            Show();
        }
        else
            Hide();
    }

    private void SetItemText()
    {
        itemName.text = item.GetItemData().name;
        descritpion.text = item.GetItemData().description;
    }

    public void Show()
    {
        if(container != null)
            container.SetActive(true);
    }
    
    public void Hide()
    {
        if (container != null)
            container.SetActive(false);
    }

    public void OnDisable()
    {
        MainCharacterScript3D.OnInteractSelectChange -= Player_OnInteractSelectChange;

    }

}
