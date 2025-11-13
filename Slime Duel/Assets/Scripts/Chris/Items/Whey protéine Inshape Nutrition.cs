using UnityEngine;

[CreateAssetMenu(menuName="SlimeGame/Item/Whey Inshape")]
public class Item_WheyInshape : BaseItemSO
{
    private void OnEnable()
    {
        itemName = "Whey protéine Inshape Nutrition";
        description = "+20 For, -10 Int. +2 For par kill. Le slime ne choisit plus sa cible.";
        flatFor = 20; 
        flatInt = -10;
    }

    public override void OnKill(SlimeUnit owner, SlimeUnit victim, ItemRuntime rt)
    {
        owner.For += 2;
        Debug.Log($"{owner.name} (Whey) gagne +2 For → {owner.For}");
    }

    // NEW: force la cible aléatoire pour attaques/compétences mono-cible
    public override bool ForceRandomSingleTarget => true;
}