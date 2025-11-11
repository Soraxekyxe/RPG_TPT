using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SlimeUnit : MonoBehaviour
{
    [Header("Données")]
    public string slimeName = "Slime";
    public SlimeClass classe;

    [Header("Liste des compétences connues")]
    public List<ActionSO> actions = new();

    [Header("Stats de base")]
    public Stats baseStats;

    [Header("Objet équipé (facultatif)")]
    public BaseItemSO equippedItem;
    public ItemRuntime itemRuntime; 

    [Header("Runtime")]
    public int PV, PVMax, Mana, Agi, For, Int, Def;
    public bool IsAlive => PV > 0;

    // Statuts actifs
    public readonly List<StatusInstance> statuses = new();

    void Awake()
    {
        // init si vide
        if (baseStats.PV == 0 && baseStats.Mana == 0)
        {
            switch (classe)
            {
                case SlimeClass.Guerrier: baseStats = new Stats{PV=60,Mana=10,Agi=8, For=10,Int=5, Def=35}; break;
                case SlimeClass.Mage:     baseStats = new Stats{PV=35,Mana=40,Agi=9, For=4, Int=16,Def=20}; break;
                case SlimeClass.Assassin: baseStats = new Stats{PV=40,Mana=15,Agi=16,For=17,Int=6, Def=20}; break;
                case SlimeClass.Clerc:    baseStats = new Stats{PV=60,Mana=30,Agi=8, For=7, Int=15,Def=25}; break;
                case SlimeClass.Druide:   baseStats = new Stats{PV=45,Mana=25,Agi=10,For=8, Int=12,Def=24}; break;
            }
        }

        PVMax = PV = Mathf.Max(1, baseStats.PV);
        Mana = baseStats.Mana; Agi = baseStats.Agi; For = baseStats.For; Int = baseStats.Int; Def = baseStats.Def;

        // Equip item
        if (equippedItem != null)
        {
            equippedItem.ApplyOnEquip(this);
            itemRuntime = new ItemRuntime(equippedItem);
            equippedItem.OnEquip(this, itemRuntime);
        }
    }

    // ======== Combat Utilities ========

    public void SpendMana(int c) => Mana = Mathf.Max(0, Mana - c);
    public void Heal(int v){ PV = Mathf.Min(PVMax, PV + Mathf.Abs(v)); }
    public void HealPercent(float p){ Heal(Mathf.CeilToInt(PVMax * Mathf.Clamp01(p))); }

    public int TakeDamage(int raw, DamageKind kind)
    {
        int dmg = kind == DamageKind.True ? Mathf.Max(0, raw) : Mathf.Max(1, raw - Def);
        PV = Mathf.Max(0, PV - dmg);

        equippedItem?.OnReceiveHit(this, null, dmg, itemRuntime);

        return dmg;
    }

    // ======== Item Hooks ========
    public void BeginBattle()       => equippedItem?.OnBattleStart(this, itemRuntime);
    public void BeginTurn()         => equippedItem?.OnTurnStart(this, itemRuntime);
    public void OnAttack(SlimeUnit t)   => equippedItem?.OnAttack(this, t, itemRuntime);
    public void OnKill(SlimeUnit v)     => equippedItem?.OnKill(this, v, itemRuntime);
    public void OnSpellCast(SlimeUnit t)=> equippedItem?.OnSpellCast(this, t, itemRuntime);

    // ======== Status System ========

    public void AddStatus(StatusSO so, int stacks = 1, int turns = 1)
    {
        statuses.Add(new StatusInstance(so, stacks, turns));
    }

    public bool HasTag(StatusTag tag)
    {
        foreach(var s in statuses)
            if (s.IsActive && s.def.HasTag(tag)) return true;
        return false;
    }

    public void TickStartOfTurn()
    {
        foreach (var s in statuses)
            if (s.IsActive) s.def.OnTurnStart(this, s);

        equippedItem?.OnTurnStart(this, itemRuntime);
    }

    public void TickEndOfTurn()
    {
        foreach (var s in statuses)
            if (s.IsActive) s.def.OnTurnEnd(this, s);

        for(int i=statuses.Count-1;i>=0;i--)
        {
            statuses[i].turns--;
            if (statuses[i].turns<=0) statuses.RemoveAt(i);
        }
    }

    // ======== UTILITAIRES POUR LES OBJETS (ajoutés) ========

    public void AddAllStats(int d)
    {
        PVMax = Mathf.Max(1, PVMax + d);
        PV    = Mathf.Clamp(PV + d, 0, PVMax);
        Mana += d;
        Agi  += d;
        For  += d;
        Int  += d;
        Def  += d;
        ClampRuntime();
    }

    public void ClampRuntime()
    {
        PVMax = Mathf.Max(1, PVMax);
        PV    = Mathf.Clamp(PV, 0, PVMax);
        Mana = Mathf.Max(0, Mana);
        Agi  = Mathf.Max(0, Agi);
        For  = Mathf.Max(0, For);
        Int  = Mathf.Max(0, Int);
        Def  = Mathf.Max(0, Def);
    }
}

// Runtime status instance
public class StatusInstance
{
    public StatusSO def;
    public int stacks;
    public int turns;
    public bool IsActive => turns > 0;

    public StatusInstance(StatusSO def,int stacks,int turns)
    {
        this.def = def;
        this.stacks = stacks;
        this.turns = turns;
    }
}







