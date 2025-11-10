using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Phantom")]
public class Item_Phantom : BaseItemSO
{
    private void OnEnable(){ itemName="Phantom";
        description="+20 Mana, +20% Int.";
        flatMana=20; percentInt=0.20f; }
}