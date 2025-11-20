using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Gacha gacha;             
    [SerializeField] private Image pulledSkinImage;     

    private GameObject _currentSkin;  

    public void OnPullButton(Skin pulledSkin)
    {
        if (_currentSkin != null)
            Destroy(_currentSkin);

        pulledSkinImage.sprite = pulledSkin.Icon;
    }
}