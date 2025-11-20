using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gacha : MonoBehaviour
{
    public List<Skin> Commun_Skins;
    public List<Skin> Rare_Skins;
    public List<Skin> Epic_Skins;
    public List<Skin> Legendary_Skins;
    [SerializeField] private GachaManager AffichageSkin;
    List<Skin> skins = new List<Skin>();
    [SerializeField] private Skin SkinEnCasDeBug;
    
    
    private int rarity_preOp = 0;
    private int rarity_pulled = 0;

    private void Start()
    {
        skins.AddRange(Commun_Skins);
        skins.AddRange(Rare_Skins);
        skins.AddRange(Epic_Skins);
        skins.AddRange(Legendary_Skins);
        /*foreach (var skin in skins)
        {
            Debug.Log("un skin a été ajouté a la machine!");
            switch (skin.MySkinRarity)
            {
                case Skin.SkinRarity.Common:
                    Commun_Skins.Add(skin);
                    break;
                case Skin.SkinRarity.Rare:
                    Rare_Skins.Add(skin);
                    break;
                case Skin.SkinRarity.Epic:
                    Epic_Skins.Add(skin);
                    break;
                case Skin.SkinRarity.Legendary:
                    Legendary_Skins.Add(skin);
                    break;
            }
        }*/
        
        
    }

    public Skin pull()
    {
        Debug.Log("count skins commun: " + Commun_Skins.Count);
        Skin pulled_skin = null;
        int rarity_commun = 4 * Commun_Skins.Count;
        int rarity_rare = 3 * Rare_Skins.Count;
        int rarity_epic = 2 * Epic_Skins.Count;
        int rarity_legendary = Legendary_Skins.Count;
        
        rarity_preOp = rarity_commun + rarity_rare + rarity_epic + rarity_legendary;
        rarity_pulled = Random.Range(1, rarity_preOp + 1);
        Debug.Log("rarity_preOp: " + rarity_preOp);
        Debug.Log("rarity_pulled: " + rarity_pulled);

        if (rarity_pulled <= rarity_commun)
            pulled_skin = Commun_Skins[Random.Range(0, Commun_Skins.Count)];
        else if (rarity_pulled <= rarity_commun + rarity_rare)
            pulled_skin = Rare_Skins[Random.Range(0, Rare_Skins.Count)];
        else if (rarity_pulled <= rarity_commun + rarity_rare + rarity_epic)
            pulled_skin = Epic_Skins[Random.Range(0, Epic_Skins.Count)];
        else
            pulled_skin = Legendary_Skins[Random.Range(0, Legendary_Skins.Count)];
        
        AffichageSkin.OnPullButton(pulled_skin);
        return pulled_skin;
    }
    
    
    public void PullButton()
    {
        Skin result = pull();
        Inventory playerInventory = Inventory.Instance;
        if(playerInventory.Skins.Contains(result))
            return;
        
        playerInventory.AddSkin(result);
        Debug.Log($"Tirage réussi : {result.name} ({result.MySkinRarity})");
    }
    
    public List<int> ID_Dump()
    {
        List<int> IDs_Skins_Locked = new List<int>{};
        foreach (var i in Commun_Skins)
        {
            IDs_Skins_Locked.Add(i.ID);
        }
        foreach (var i in Rare_Skins)
        {
            IDs_Skins_Locked.Add(i.ID);
        }
        foreach (var i in Epic_Skins)
        {
            IDs_Skins_Locked.Add(i.ID);
        }
        foreach (var i in Legendary_Skins)
        {
            IDs_Skins_Locked.Add(i.ID);
        }
        return IDs_Skins_Locked;
    }
    
}