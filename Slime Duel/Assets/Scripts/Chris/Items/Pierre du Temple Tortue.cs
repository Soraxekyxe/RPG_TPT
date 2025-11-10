using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Pierre Temple Tortue")]
public class Item_Tortue : BaseItemSO
{
    private void OnEnable(){ itemName="Pierre du Temple Tortue";
        description="+20 For, +5 Def. Quand PV ≤ 30%, gagne +10 For (une fois).";
        flatFor=20; flatDef=5; }
    public override void OnTurnStart(Slime owner, ItemRuntime rt)
    {
        if (!rt.flagA && owner.PV <= owner.PVMax*0.3f)
        { owner.For+=10; rt.flagA=true; Debug.Log($"{owner.name} (Tortue): +10 For (enragé)"); }
    }
}