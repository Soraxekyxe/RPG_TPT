using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Pot de miel de Wini")]
public class Item_Wini : BaseItemSO
{
    private void OnEnable(){ itemName="Pot de miel de Wini";
        description="+20 PV, +5 Def. Quand attaqué: 20% de réduire l'Agi de l'attaquant de 3.";
        flatPV=20; flatDef=5; }
    public override void OnReceiveHit(Slime owner, Slime attacker, int incomingDamage, ItemRuntime rt)
    {
        if (attacker==null) return;
        if (Roll(0.20f))
        {
            attacker.Agi = Mathf.Max(0, attacker.Agi - 3);
            Debug.Log($"{owner.name} (Miel) ralentit {attacker.name}: -3 Agi ({attacker.Agi})");
        }
    }
}