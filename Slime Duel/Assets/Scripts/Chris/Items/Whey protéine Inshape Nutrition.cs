using UnityEngine;
[CreateAssetMenu(menuName="SlimeGame/Item/Whey Inshape")]
public class Item_WheyInshape : BaseItemSO
{
    private void OnEnable(){ itemName="Whey protéine Inshape Nutrition";
        description="+20 For, -10 Int. +2 For par kill. Le slime ne choisit plus sa cible.";
        flatFor=20; flatInt=-10; }
    public override void OnKill(Slime owner, Slime victim, ItemRuntime rt)
    { owner.For+=2; Debug.Log($"{owner.name} (Whey) gagne +2 For → {owner.For}"); }
    public override void OnAttack(Slime owner, Slime target, ItemRuntime rt)
    { Debug.Log($"{owner.name} (Whey): cible devrait être aléatoire (brancher dans ton sélecteur)."); }
}