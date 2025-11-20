using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Poignard low-cost")]
public class Item_Poignard : BaseItemSO
{
    private void OnEnable(){ itemName="Poignard low-cost du RP Quartier";
        description="+10 Agi, +10 For. 15%: inflige 30% des PV max de la cible en dégâts vrais.";
        flatAgi=10; flatFor=10; }
    public override void OnAttack(SlimeUnit owner, SlimeUnit target, ItemRuntime rt)
    {
        if (target==null) return;
        if (Roll(0.15f))
        {
            int trueDmg = Mathf.CeilToInt(target.PVMax*0.30f);
            target.PV = Mathf.Max(0, target.PV - trueDmg);
            Debug.Log($"{owner.name} (Poignard): {trueDmg} dégâts vrais → {target.name} {target.PV}/{target.PVMax}");
        }
    }
}