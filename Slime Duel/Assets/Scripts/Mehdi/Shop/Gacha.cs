using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gacha : MonoBehaviour
{
    private List<Skin> Commun_Skins = new List<Skin>();
    private List<Skin> Rare_Skins = new List<Skin>();
    private List<Skin> Epic_Skins = new List<Skin>();
    private List<Skin> Legendary_Skins = new List<Skin>();
    
    private int rarity_preOp = 0;
    private int rarity_pulled = 0;

    private void Awake()
    {
        var skins = Resources.LoadAll<Skin>("Skins");
        foreach (var skin in skins)
        {
            switch (skin.skinRarity)
            {
                case SkinRarity.Common:
                    Commun_Skins.Add(skin);
                    break;
                case SkinRarity.Rare:
                    Rare_Skins.Add(skin);
                    break;
                case SkinRarity.Epic:
                    Epic_Skins.Add(skin);
                    break;
                case SkinRarity.Legendary:
                    Legendary_Skins.Add(skin);
                    break;
            }
        }
    }

    public Skin pull()
    {
        Skin pulled_skin = null;
        int rarity_commun = 40 * Commun_Skins.Count;
        int rarity_rare = 30 * Rare_Skins.Count;
        int rarity_epic = 20 * Epic_Skins.Count;
        int rarity_legendary = 10 * Legendary_Skins.Count;
        rarity_preOp = rarity_commun + rarity_rare + rarity_epic + rarity_legendary;
        rarity_pulled = Random.Range(1, rarity_preOp + 1);

        if (rarity_pulled <= rarity_commun)
            pulled_skin = Commun_Skins[Random.Range(0, Commun_Skins.Count)];
        else if (rarity_pulled <= rarity_commun + rarity_rare)
            pulled_skin = Rare_Skins[Random.Range(0, Rare_Skins.Count)];
        else if (rarity_pulled <= rarity_commun + rarity_rare + rarity_epic)
            pulled_skin = Epic_Skins[Random.Range(0, Epic_Skins.Count)];
        else
            pulled_skin = Legendary_Skins[Random.Range(0, Legendary_Skins.Count)];

        return pulled_skin;
    }
    
    public void PullButton()
    {
        Skin result = pull();
        Inventory playerInventory = Inventory.Instance;
        if(playerInventory.Skins.Contains(result))
            return;
        
        playerInventory.AddSkin(result);
        Debug.Log($"Tirage réussi : {result.name} ({result.skinRarity})");
    }
}