using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Tome du Mage Fatigué")]
public class Item_TomeIsekai : BaseItemSO
{
    private void OnEnable(){ itemName="Tome du Magicien Fatigué par l’Isekai";
        description="+15 Int, +10 Mana. À chaque sort: 30% de gagner +2 Int OU +2 Mana.";
        flatInt=15; flatMana=10; }
    public override void OnSpellCast(Slime owner, Slime target, ItemRuntime rt)
    {
        if (!Roll(0.30f)) return;
        if (Roll(0.5f)) { owner.Int+=2; Debug.Log($"{owner.name} (Tome): +2 Int ({owner.Int})"); }
        else            { owner.Mana+=2; Debug.Log($"{owner.name} (Tome): +2 Mana ({owner.Mana})"); }
    }
}