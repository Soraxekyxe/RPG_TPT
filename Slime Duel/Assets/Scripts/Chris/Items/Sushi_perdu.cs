using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Sushi perdu")]
public class Item_SushiPerdu : BaseItemSO
{
    private void OnEnable(){ itemName="Sushi perdu";
        description="+20 PV. Début combat: +5 à toutes les stats OU -5 à toutes les stats.";
        flatPV=20; }
    public override void OnBattleStart(SlimeUnit owner, ItemRuntime rt)
    {
        int d = Random.value<0.5f ? 5 : -5;
        owner.AddAllStats(d);
        Debug.Log($"{owner.name} (Sushi) {(d>0?"+5":"-5")} partout.");
    }
}