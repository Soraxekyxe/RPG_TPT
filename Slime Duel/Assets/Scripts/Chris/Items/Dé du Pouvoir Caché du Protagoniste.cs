using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Dé du Protagoniste")]
public class Item_Protagoniste : BaseItemSO
{
    private void OnEnable(){ itemName="Dé du Pouvoir Caché du Protagoniste";
        description="+5 Agi, +5 Int. Début: 50% +3 à une stat aléatoire, sinon -2 à une stat aléatoire.";
        flatAgi=5; flatInt=5; }
    public override void OnBattleStart(Slime owner, ItemRuntime rt)
    {
        bool up = Roll(0.5f); int d = up?3:-2; int i = Random.Range(0,6);
        switch(i){
            case 0: owner.PVMax+=d; owner.PV=Mathf.Clamp(owner.PV+d,0,owner.PVMax); break;
            case 1: owner.Mana+=d; break; case 2: owner.Agi+=d; break;
            case 3: owner.For+=d; break; case 4: owner.Int+=d; break; case 5: owner.Def+=d; break;
        }
        owner.ClampRuntime();
        Debug.Log($"{owner.name} (Dé): {(up?"+3":"-2")} appliqué sur une stat aléatoire.");
    }
}