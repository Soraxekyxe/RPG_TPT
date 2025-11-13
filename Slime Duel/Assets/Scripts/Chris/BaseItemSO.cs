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

    public virtual void ApplyOnEquip(SlimeUnit owner)
    {
        // PV
        owner.PVMax += flatPV;
        owner.PV    += flatPV;

        // Mana (augmente aussi le max)
        owner.ManaMax += flatMana;
        owner.Mana    += flatMana;

        // Autres stats
        owner.Agi += flatAgi;
        owner.For += flatFor;
        owner.Int += flatInt;
        owner.Def += flatDef;

        if (Mathf.Abs(percentInt) > 0.0001f)
            owner.Int = Mathf.RoundToInt(owner.Int * (1f + percentInt));

        owner.ClampRuntime();
    }

    // ===== Hooks Items =====
    public virtual void OnEquip(SlimeUnit owner, ItemRuntime rt) { }
    public virtual void OnBattleStart(SlimeUnit owner, ItemRuntime rt) { }
    public virtual void OnTurnStart(SlimeUnit owner, ItemRuntime rt) { }
    public virtual void OnAttack(SlimeUnit owner, SlimeUnit target, ItemRuntime rt) { }
    public virtual void OnReceiveHit(SlimeUnit owner, SlimeUnit attacker, int incomingDamage, ItemRuntime rt) { }
    public virtual void OnKill(SlimeUnit owner, SlimeUnit victim, ItemRuntime rt) { }
    public virtual void OnSpellCast(SlimeUnit owner, SlimeUnit target, ItemRuntime rt) { }
    
    public virtual bool ForceRandomSingleTarget => false;


    protected bool Roll(float p) => Random.value < Mathf.Clamp01(p);
}