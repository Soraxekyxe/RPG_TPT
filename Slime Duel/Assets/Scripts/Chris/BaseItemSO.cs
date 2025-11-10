using UnityEngine;

public abstract class BaseItemSO : ScriptableObject
{
    [Header("UI")]
    public string itemName;
    [TextArea] public string description;

    [Header("Bonus plats appliqués à l'équipement")]
    public int flatPV, flatMana, flatAgi, flatFor, flatInt, flatDef;

    [Header("Multiplicateurs (ex: 0.20 = +20%)")]
    public float percentInt;

    // Appelé juste après l'initialisation des stats de la classe
    public virtual void ApplyOnEquip(Slime owner)
    {
        owner.PVMax += flatPV; owner.PV += flatPV;
        owner.Mana  += flatMana;
        owner.Agi   += flatAgi;
        owner.For   += flatFor;
        owner.Int   += flatInt;
        owner.Def   += flatDef;

        if (Mathf.Abs(percentInt) > 0.0001f)
            owner.Int = Mathf.RoundToInt(owner.Int * (1f + percentInt));

        owner.ClampRuntime();
    }

    // Hooks d’événements
    public virtual void OnEquip(Slime owner, ItemRuntime rt) { }
    public virtual void OnBattleStart(Slime owner, ItemRuntime rt) { }
    public virtual void OnTurnStart(Slime owner, ItemRuntime rt) { }
    public virtual void OnAttack(Slime owner, Slime target, ItemRuntime rt) { }
    public virtual void OnReceiveHit(Slime owner, Slime attacker, int incomingDamage, ItemRuntime rt) { }
    public virtual void OnKill(Slime owner, Slime victim, ItemRuntime rt) { }
    public virtual void OnSpellCast(Slime owner, Slime target, ItemRuntime rt) { }

    protected bool Roll(float p) => Random.value < Mathf.Clamp01(p);
}