using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Poireau 90 yen")]
public class Item_Poireau : BaseItemSO
{
    private void OnEnable(){ itemName="Le poireau à 90 yen";
        description="+10 PV, +10 Mana. Début combat: 50% prière (+2 Int/Def) ou 50% mange (+10% PV max).";
        flatPV=10; flatMana=10; }
    public override void OnBattleStart(Slime owner, ItemRuntime rt)
    {
        if (Roll(0.5f))
        { owner.Int+=2; owner.Def+=2; Debug.Log($"{owner.name} prie le poireau: +2 Int/Def."); }
        else
        {
            int add = Mathf.CeilToInt(owner.PVMax*0.10f);
            owner.PVMax+=add; owner.PV=Mathf.Min(owner.PV, owner.PVMax);
            Debug.Log($"{owner.name} mange le poireau: +{add} PV max.");
        }
        owner.ClampRuntime();
    }
}