using UnityEngine;

public enum SlimeClass { Guerrier, Mage, Assassin, Clerc, Druide }

public class Slime : MonoBehaviour
{
    [Header("Choix de la classe")]
    public SlimeClass classe;

    [Header("Bonus de stats (ajoutés aux stats de base)")]
    public int bonusPV;
    public int bonusMana;
    public int bonusAgi;
    public int bonusFor;
    public int bonusInt;
    public int bonusDef;

    [Header("Objet équipé (facultatif)")]
    public BaseItemSO equippedItem;       // <-- l'objet

    // Stats runtime
    [HideInInspector] public int PV;
    [HideInInspector] public int PVMax;   // <-- ajouté
    [HideInInspector] public int Mana;
    [HideInInspector] public int Agi;
    [HideInInspector] public int For;
    [HideInInspector] public int Int;
    [HideInInspector] public int Def;

    // État runtime pour l'item (flags/stacks)
    public ItemRuntime itemRuntime;       // <-- ajouté

    void Start()
    {
        InitialiserStats();

        // applique l'objet (plats + %), puis hook OnEquip
        if (equippedItem != null)
        {
            equippedItem.ApplyOnEquip(this);
            itemRuntime = new ItemRuntime(equippedItem);
            equippedItem.OnEquip(this, itemRuntime);
        }

        Debug.Log($"{name} ({classe}) → PV:{PV}/{PVMax} Mana:{Mana} Agi:{Agi} For:{For} Int:{Int} Def:{Def}"
            + (equippedItem ? $" | Objet: {equippedItem.itemName}" : ""));
    }

    void InitialiserStats()
    {
        switch (classe)
        {
            case SlimeClass.Guerrier: PV = 60; Mana = 10; Agi = 8;  For = 10; Int = 5;  Def = 35; break;
            case SlimeClass.Mage:     PV = 35; Mana = 40; Agi = 9;  For = 4;  Int = 16; Def = 20; break;
            case SlimeClass.Assassin: PV = 40; Mana = 15; Agi = 16; For = 17; Int = 6;  Def = 20; break;
            case SlimeClass.Clerc:    PV = 40; Mana = 30; Agi = 8;  For = 7;  Int = 15; Def = 20; break;
            case SlimeClass.Druide:   PV = 45; Mana = 25; Agi = 10; For = 8;  Int = 12; Def = 24; break;
        }

        PV   += bonusPV; Mana += bonusMana; Agi += bonusAgi;
        For  += bonusFor; Int  += bonusInt; Def += bonusDef;

        PV = Mathf.Max(1, PV);
        PVMax = PV; // initialise PVMax à la valeur de départ
        ClampRuntime();
    }

    // === Hooks simples que le driver peut appeler ===
    public void BeginBattle() { equippedItem?.OnBattleStart(this, itemRuntime); ClampRuntime(); }
    public void BeginTurn()   { equippedItem?.OnTurnStart(this, itemRuntime);   ClampRuntime(); }
    public void Attack(Slime target) { equippedItem?.OnAttack(this, target, itemRuntime); }
    public void CastSpell(Slime target) { equippedItem?.OnSpellCast(this, target, itemRuntime); ClampRuntime(); }
    public void ReceiveHit(Slime attacker, int incomingDamage) { equippedItem?.OnReceiveHit(this, attacker, incomingDamage, itemRuntime); }
    public void Kill(Slime victim) { equippedItem?.OnKill(this, victim, itemRuntime); ClampRuntime(); }

    // === Helpers utilisés par tes items ===
    public void HealPercent(float pct)
    {
        int heal = Mathf.CeilToInt(PVMax * Mathf.Clamp01(pct));
        PV = Mathf.Min(PVMax, PV + heal);
        Debug.Log($"{name} soigne {heal} PV → {PV}/{PVMax}");
    }

    public void AddAllStats(int d)
    {
        PVMax = Mathf.Max(1, PVMax + d);
        PV    = Mathf.Clamp(PV + d, 0, PVMax);
        Mana += d; Agi += d; For += d; Int += d; Def += d;
        ClampRuntime();
    }

    public void ClampRuntime()
    {
        PVMax = Mathf.Max(1, PVMax);
        PV    = Mathf.Clamp(PV, 0, PVMax);
        Mana  = Mathf.Max(0, Mana);
        Agi   = Mathf.Max(0, Agi);
        For   = Mathf.Max(0, For);
        Int   = Mathf.Max(0, Int);
        Def   = Mathf.Max(0, Def);
    }
}

// État runtime par objet (flags/stacks)
public class ItemRuntime
{
    public BaseItemSO def;
    public bool flagA, flagB;
    public int stacksA, stacksB;
    public ItemRuntime(BaseItemSO def) { this.def = def; }
}
