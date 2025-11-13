using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Gacha gacha;             
    [SerializeField] private Image pulledSkinImage;     

    private GameObject _currentSkin;  

    public void OnPullButton()
    {
       //Pour chopper le skin ta capté
        Skin pulledSkin = gacha.pull();
        Debug.Log($"Tirage : {pulledSkin.name} ({pulledSkin.skinRarity})");

       //pour suprimmer l'ancien et afficher le nouveau 
        if (_currentSkin != null)
            Destroy(_currentSkin);

        pulledSkinImage.sprite = pulledSkin.Icon;
    }
}