using System;
using UnityEngine;
using Yuu.Inventory;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] 
    private Transform container;
    [SerializeField]
    private GameObject itemPrefab;
    
    [SerializeField]
    private CanvasGroup canvasGroup;

    private void Start()
    {
        Close();
    }

    public void Open()
    {
        foreach (Transform t in container)
            Destroy(t.gameObject);

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        
        foreach (Skin skin in Inventory.Instance.Skins)
        {
            GameObject instance = Instantiate(itemPrefab, container);
            if (instance.TryGetComponent(out SkinUI skinUI))
                skinUI.SetSkin(this, skin);
        }
    }
    
    public void Close()
    {
        foreach (Transform t in container)
            Destroy(t.gameObject);
        
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}
