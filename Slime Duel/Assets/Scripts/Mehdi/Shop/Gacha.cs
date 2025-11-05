using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Gacha : MonoBehaviour
{
    [Header("proba: commun -> 40/100, rare -> 30/100, epic -> 20/100, legendaire -> 10/100")]
    [SerializeField] public List<Skin> Commun_Skins = new List<Skin>();
    [SerializeField] public List<Skin> Rare_Skins = new List<Skin>();
    [SerializeField] public List<Skin> Epic_Skins = new List<Skin>();
    [SerializeField] public List<Skin> Legendary_Skins = new List<Skin>();
    private int rarity_preOp = 0;
    private int rarity_pulled = 0;

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
        {
            pulled_skin = Commun_Skins[Random.Range(0, Commun_Skins.Count)];
            return pulled_skin;
        }
        if (rarity_pulled <= rarity_commun + rarity_rare)
        {
            pulled_skin = Rare_Skins[Random.Range(0, Rare_Skins.Count)];
            return pulled_skin;
        }
        if (rarity_pulled <= rarity_commun + rarity_rare + rarity_epic)
        {
            pulled_skin = Epic_Skins[Random.Range(0, Epic_Skins.Count)];
            return pulled_skin;
        }
        if (rarity_pulled <= rarity_commun + rarity_rare + rarity_epic + rarity_legendary)
        {
            pulled_skin = Legendary_Skins[Random.Range(0, Legendary_Skins.Count)];
            return pulled_skin;
        }
        return pulled_skin;
    }
}
