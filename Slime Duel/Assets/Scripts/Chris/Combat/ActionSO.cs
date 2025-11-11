using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="SlimeGame/Action")]
public class ActionSO : ScriptableObject
{
    [Header("UI")]
    public string actionName;
    [TextArea] public string description;
    public int manaCost = 0;
    public TargetMode targetMode = TargetMode.EnemySingle;
    public bool isSignature = false;

    [Header("Dégâts / Soins")]
    public bool doesDamage;
    public DamageKind damageKind = DamageKind.Physical;
    [Tooltip("Base = % de For/Int/Def/PVmax/PV manquants. Additionnés si multiples cochés.")]
    public bool useFor, useInt, useDef, usePVMax, useMissingPV;
    [Range(0f, 3f)] public float percentFor = 0f;
    [Range(0f, 3f)] public float percentInt = 0f;
    [Range(0f, 3f)] public float percentDef = 0f;
    [Range(0f, 1f)] public float percentPVMax = 0f;
    [Range(0f, 2f)] public float percentMissingPV = 0f;
    [Min(1)] public int hits = 1;
    public bool randomSplitHits = false; // ex: lancer de cailloux

    [Header("Soins")]
    public bool doesHeal;
    [Range(0f, 3f)] public float healPercentInt = 0f; // ex Clerc 0.6, 1.2, 1.5

    [Header("Effets")]
    public StatusSO applyStatus;  // ex: brûlure, stun, root, taunt, confuse, untargetable, immunité
    public int applyStatusTurns = 1;
    public int applyStatusStacks = 1;

    [Header("Buff/Débuff direct (%)")]
    public float addPercentFor, addPercentInt, addPercentAgi, addPercentDef; // crée un Status ad-hoc

    [Header("Drain/ressources")]
    public bool drainMana;
    [Range(0f, 1f)] public float drainManaPercentOfTargetRemaining = 0f;

    // --------- Exécution ----------
    public bool CanPay(SlimeUnit user) => user.Mana >= manaCost && user.IsAlive;

    public void Execute(SlimeUnit user, List<SlimeUnit> allies, List<SlimeUnit> enemies)
    {
        if (!CanPay(user)) { Debug.Log($"{user.slimeName} n'a pas assez de mana pour {actionName}"); return; }
        user.SpendMana(manaCost);

        var targets = ResolveTargets(user, allies, enemies);
        if (targets.Count == 0) { Debug.Log($"{actionName}: pas de cible."); return; }

        foreach (var t in targets)
        {
            // Heal
            if (doesHeal)
            {
                int heal = Mathf.RoundToInt(user.Int * ApplyStatMult(user, StatusAxis.Int) * healPercentInt);
                t.Heal(heal);
                Debug.Log($"{user.slimeName} soigne {t.slimeName} de {heal} PV ({actionName})");
            }

            // Damage
            if (doesDamage)
            {
                int total = 0;
                if (hits <= 1 || !randomSplitHits)
                {
                    int raw = ComputeRaw(user, t);
                    total = t.TakeDamage(raw, damageKind);
                    Debug.Log($"{user.slimeName} → {t.slimeName} subit {total} ({actionName})");
                }
                else
                {
                    // p.ex. 2 à 5 cailloux (mets hits=5 et un power progressif via plusieurs SO, ou approx aléatoire)
                    int n = Random.Range(2, Mathf.Min(hits,5)+1);
                    for (int i=0;i<n;i++)
                    {
                        int raw = ComputeRaw(user, t) * (i+1) / n; // progression simple
                        int d = t.TakeDamage(raw, damageKind);
                        total += d;
                        Debug.Log($"{actionName} coup {i+1}/{n}: {d}");
                    }
                }
            }

            // Drain Mana
            if (drainMana && drainManaPercentOfTargetRemaining > 0f)
            {
                int steal = Mathf.CeilToInt(t.Mana * drainManaPercentOfTargetRemaining);
                t.Mana = Mathf.Max(0, t.Mana - steal);
                user.Mana += steal;
                Debug.Log($"{user.slimeName} draine {steal} Mana à {t.slimeName}");
            }

            // Status direct
            if (applyStatus != null)
            {
                // immunité ?
                if (!t.HasTag(StatusTag.Immunity))
                {
                    t.AddStatus(applyStatus, applyStatusStacks, applyStatusTurns);
                    Debug.Log($"{t.slimeName} reçoit {applyStatus.statusName} ({applyStatusTurns}t)");
                }
            }

            // Buff/debuff % “ad-hoc”
            if (addPercentFor != 0 || addPercentInt != 0 || addPercentAgi != 0 || addPercentDef != 0)
            {
                var temp = ScriptableObject.CreateInstance<StatusSO>();
                temp.statusName = "Modif%";
                temp.percentFor = addPercentFor;
                temp.percentInt = addPercentInt;
                temp.percentAgi = addPercentAgi;
                temp.percentDef = addPercentDef;
                t.AddStatus(temp, 1, applyStatusTurns>0?applyStatusTurns:2);
                Debug.Log($"{t.slimeName} gagne modif % stats ({actionName})");
            }
        }
    }

    List<SlimeUnit> ResolveTargets(SlimeUnit user, List<SlimeUnit> allies, List<SlimeUnit> enemies)
    {
        var res = new List<SlimeUnit>();
        switch (targetMode)
        {
            case TargetMode.Self: res.Add(user); break;
            case TargetMode.AllySingle:
                var ally = PickAlive(allies, preferTaunted:false);
                if (ally!=null) res.Add(ally);
                break;
            case TargetMode.AllyAll: res.AddRange(allies.FindAll(a=>a && a.IsAlive)); break;
            case TargetMode.EnemySingle:
                var enemy = PickAlive(enemies, preferTaunted:true);
                if (enemy!=null) res.Add(enemy);
                break;
            case TargetMode.EnemyAll: res.AddRange(enemies.FindAll(e=>e && e.IsAlive)); break;
        }
        return res;
    }

    SlimeUnit PickAlive(List<SlimeUnit> list, bool preferTaunted)
    {
        var alive = list.FindAll(u => u && u.IsAlive && !u.HasTag(StatusTag.Untargetable));
        if (alive.Count == 0) return null;
        if (preferTaunted)
        {
            var taunts = alive.FindAll(u => u.HasTag(StatusTag.Taunt));
            if (taunts.Count > 0) return taunts[Random.Range(0, taunts.Count)];
        }
        return alive[Random.Range(0, alive.Count)];
    }

    int ComputeRaw(SlimeUnit user, SlimeUnit target)
    {
        // multiplicateurs de stats affectés par statuts sur l'attaquant
        float mFor = ApplyStatMult(user, StatusAxis.For);
        float mInt = ApplyStatMult(user, StatusAxis.Int);
        float mDef = ApplyStatMult(user, StatusAxis.Def);

        float raw = 0f;
        if (useFor) raw += user.For * mFor * percentFor;
        if (useInt) raw += user.Int * mInt * percentInt;
        if (useDef) raw += user.Def * mDef * percentDef;
        if (usePVMax) raw += target.PVMax * percentPVMax;          // ex ZAC E (PVmax ennemis)
        if (useMissingPV) raw += (user.PVMax - user.PV) * percentMissingPV; // signature guerrier

        // Réduction dégâts infligés / reçus (Formation tortue)
        if (user.HasTag(StatusTag.DmgOutDown)) raw *= 0.8f;   // -20% out
        if (target.HasTag(StatusTag.DmgInDown)) raw *= 0.9f;  // -10% in

        return Mathf.Max(0, Mathf.RoundToInt(raw));
    }

    enum StatusAxis { For, Int, Agi, Def }
    float ApplyStatMult(SlimeUnit user, StatusAxis ax)
    {
        float mul = 1f;
        foreach (var s in user.statuses)
        {
            if (!s.IsActive) continue;
            switch (ax)
            {
                case StatusAxis.For: mul *= s.def.MulFor(); break;
                case StatusAxis.Int: mul *= s.def.MulInt(); break;
                case StatusAxis.Agi: mul *= s.def.MulAgi(); break;
                case StatusAxis.Def: mul *= s.def.MulDef(); break;
            }
        }
        return mul;
    }
}
