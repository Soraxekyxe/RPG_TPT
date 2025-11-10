using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Micro Auto-Tune")]
public class Item_AutoTune : BaseItemSO
{
    private void OnEnable(){ itemName="Micro Auto-Tune";
        description="+10 Agi, +10 For. 20%: ignore la DEF de l’ennemi pour cette attaque.";
        flatAgi=10; flatFor=10; }
    public override void OnAttack(Slime owner, Slime target, ItemRuntime rt)
    {
        if (Roll(0.20f))
            Debug.Log($"{owner.name} (Auto-Tune): IGNORE DEF sur cette attaque (prends-le en compte dans ton calcul).");
    }
}