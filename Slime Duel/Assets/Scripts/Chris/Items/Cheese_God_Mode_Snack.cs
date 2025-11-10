using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Cheese God Mode Snack")]
public class Item_CheeseGod : BaseItemSO
{
    private void OnEnable(){ itemName="Cheese God Mode Snack";
        description="+30 PV, +5 Def. 20%/tour: régénère 5% PV.";
        flatPV=30; flatDef=5; }
    public override void OnTurnStart(Slime owner, ItemRuntime rt)
    {
        if (Roll(0.20f)) owner.HealPercent(0.05f);
    }
}