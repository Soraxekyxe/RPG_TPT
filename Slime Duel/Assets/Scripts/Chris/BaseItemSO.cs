using UnityEngine;

public abstract class BaseItemSO : ScriptableObject
{
    [Header("UI")]
    public string itemName;
    [TextArea] public string description;

    [Header("Bonus plats à l'équipement")]
    public int flatPV, flatMana, flatAgi, flatFor, flatInt, flatDef;

    [Header("Multiplicateurs (ex: 0.20 = +20%)")]
    public float percentInt;

    // Appliqué une seule fois à l'équipement
    public virtual void ApplyOnEquip(SlimeUnit owner)
    {
        owner.PVMax += flatPV; owner.PV += flatPV;
        owner.Mana  += flatMana;
        owner.Agi   += flatAgi;
        owner.For   += flatFor;
        owner.Int   += flatInt;
        owner.Def   += flatDef;

        if (Mathf.Abs(percentInt) > 0.0001f)
            owner.Int = Mathf.RoundToInt(owner.Int * (1f + percentInt));

        owner.PV = Mathf.Clamp(owner.PV, 0, owner.PVMax);
    }

    // ===== Hooks Items (tous en SlimeUnit) =====
    public virtual void OnEquip(SlimeUnit owner, ItemRuntime rt) { }
    public virtual void OnBattleStart(SlimeUnit owner, ItemRuntime rt) { }
    public virtual void OnTurnStart(SlimeUnit owner, ItemRuntime rt) { }
    public virtual void OnAttack(SlimeUnit owner, SlimeUnit target, ItemRuntime rt) { }
    public virtual void OnReceiveHit(SlimeUnit owner, SlimeUnit attacker, int incomingDamage, ItemRuntime rt) { }
    public virtual void OnKill(SlimeUnit owner, SlimeUnit victim, ItemRuntime rt) { }
    public virtual void OnSpellCast(SlimeUnit owner, SlimeUnit target, ItemRuntime rt) { }

    protected bool Roll(float p) => Random.value < Mathf.Clamp01(p);
}