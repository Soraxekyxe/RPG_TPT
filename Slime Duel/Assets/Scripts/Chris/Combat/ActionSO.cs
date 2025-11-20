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

    [Header("Condition")] 
    public int Niveau = 1;
    public SlimeClass classe = SlimeClass.Guerrier;

    public bool IsUnlocked(SlimeUnit user)
    {
        bool lvlOk = user.Lvl >= Niveau;
        bool classOk = user.classe == classe;
        return lvlOk && classOk;
    }
    
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

    public bool CanPay(SlimeUnit user)
        => user != null && user.IsAlive && user.Mana >= manaCost;

    /// <summary>
    /// Version “auto” (IA ou skill sans ciblage manuel) :
    /// les cibles sont choisies avec targetMode.
    /// </summary>
    public void Execute(SlimeUnit user, List<SlimeUnit> allies, List<SlimeUnit> enemies)
    {
        if (user == null)
        {
            Debug.LogError("[ActionSO] Execute appelé avec user NULL sur " + name);
            return;
        }

        if (!CanPay(user))
        {
            Debug.Log($"{user.slimeName} n'a pas assez de mana pour {actionName}");
            return;
        }

        allies  ??= new List<SlimeUnit>();
        enemies ??= new List<SlimeUnit>();

        var targets = ResolveTargets(user, allies, enemies);
        // nettoie les null / morts
        targets.RemoveAll(t => t == null || !t.IsAlive);

        if (targets.Count == 0)
        {
            Debug.Log($"{actionName}: pas de cible.");
            return;
        }

        user.SpendMana(manaCost);
        ApplyToTargets(user, targets);
    }

    /// <summary>
    /// Version utilisée quand le joueur clique une cible précise (EnemySingle/AllySingle).
    /// </summary>
    public void ExecuteOnTarget(SlimeUnit user, SlimeUnit target, List<SlimeUnit> allies, List<SlimeUnit> enemies)
    {
        if (user == null)
        {
            Debug.LogError("[ActionSO] ExecuteOnTarget user NULL sur " + name);
            return;
        }

        if (!CanPay(user))
        {
            Debug.Log($"{user.slimeName} n'a pas assez de mana pour {actionName}");
            return;
        }

        allies  ??= new List<SlimeUnit>();
        enemies ??= new List<SlimeUnit>();

        var forcedTargets = ResolveForcedTargets(user, target, allies, enemies);
        forcedTargets.RemoveAll(t => t == null || !t.IsAlive);

        if (forcedTargets.Count == 0)
        {
            Debug.Log($"{actionName}: pas de cible valide (ExecuteOnTarget).");
            return;
        }

        user.SpendMana(manaCost);
        ApplyToTargets(user, forcedTargets);
    }

    // --- PRIVATE: construit la liste des cibles “forcées” (clic) ---
    List<SlimeUnit> ResolveForcedTargets(SlimeUnit user, SlimeUnit target, List<SlimeUnit> allies, List<SlimeUnit> enemies)
    {
        var res = new List<SlimeUnit>();

        switch (targetMode)
        {
            case TargetMode.EnemySingle:
                if (target && enemies.Contains(target) && target.IsAlive && !target.HasTag(StatusTag.Untargetable))
                    res.Add(target);
                break;

            case TargetMode.AllySingle:
                if (target && allies.Contains(target) && target.IsAlive)
                    res.Add(target);
                break;

            default:
                // Pour Self / All / etc. → on retombe sur la logique classique
                res = ResolveTargets(user, allies, enemies);
                break;
        }

        return res;
    }

    // --- PRIVATE: applique dégâts/soins/effets à une liste de cibles ---
    void ApplyToTargets(SlimeUnit user, List<SlimeUnit> targets)
    {
        foreach (var t in targets)
        {
            if (t == null) continue;

            // Heal
            if (doesHeal)
            {
                int heal = Mathf.RoundToInt(user.Int * ApplyStatMult(user, StatusAxis.Int) * healPercentInt);
                if (heal != 0)
                {
                    t.Heal(heal);
                    Debug.Log($"{user.slimeName} soigne {t.slimeName} de {heal} PV ({actionName})");
                }
            }

            // Damage
            if (doesDamage)
            {
                if (hits <= 1 || !randomSplitHits)
                {
                    int raw = ComputeRaw(user, t);
                    int dealt = t.TakeDamage(raw, damageKind);
                    Debug.Log($"{user.slimeName} → {t.slimeName} subit {dealt} ({actionName})");
                }
                else
                {
                    int n = Random.Range(2, Mathf.Min(hits, 5) + 1);
                    for (int i = 0; i < n; i++)
                    {
                        int raw = ComputeRaw(user, t) * (i + 1) / n;
                        int d = t.TakeDamage(raw, damageKind);
                        Debug.Log($"{actionName} coup {i + 1}/{n}: {d}");
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
            if (applyStatus != null && !t.HasTag(StatusTag.Immunity))
            {
                t.AddStatus(applyStatus, applyStatusStacks, applyStatusTurns);
                Debug.Log($"{t.slimeName} reçoit {applyStatus.statusName} ({applyStatusTurns}t)");
            }

            // Buff/debuff % “ad-hoc”
            if (addPercentFor != 0 || addPercentInt != 0 || addPercentAgi != 0 || addPercentDef != 0)
            {
                // on crée un petit status simple qui applique juste des % de stats
                var temp = ScriptableObject.CreateInstance<SimplePercentStatus>();
                temp.statusName = "Modif%";
                temp.percentFor = addPercentFor;
                temp.percentInt = addPercentInt;
                temp.percentAgi = addPercentAgi;
                temp.percentDef = addPercentDef;

                int turns = applyStatusTurns > 0 ? applyStatusTurns : 2;
                t.AddStatus(temp, 1, turns);
                Debug.Log($"{t.slimeName} gagne modif % stats ({actionName})");
            }
        }
    }

    // --------- Ciblage "auto" ----------

    List<SlimeUnit> ResolveTargets(SlimeUnit user, List<SlimeUnit> allies, List<SlimeUnit> enemies)
    {
        var res = new List<SlimeUnit>();

        allies  ??= new List<SlimeUnit>();
        enemies ??= new List<SlimeUnit>();

        switch (targetMode)
        {
            case TargetMode.Self:
                res.Add(user);
                break;

            case TargetMode.AllySingle:
            {
                var ally = PickAlive(allies, preferTaunted:false);
                if (ally != null) res.Add(ally);
                break;
            }

            case TargetMode.AllyAll:
                res.AddRange(allies.FindAll(a => a && a.IsAlive));
                break;

            case TargetMode.EnemySingle:
            {
                var enemy = PickAlive(enemies, preferTaunted:true);
                if (enemy != null) res.Add(enemy);
                break;
            }

            case TargetMode.EnemyAll:
                res.AddRange(enemies.FindAll(e => e && e.IsAlive));
                break;
        }
        return res;
    }

    SlimeUnit PickAlive(List<SlimeUnit> list, bool preferTaunted)
    {
        if (list == null || list.Count == 0) return null;

        var alive = list.FindAll(u => u && u.IsAlive && !u.HasTag(StatusTag.Untargetable));
        if (alive.Count == 0) return null;

        if (preferTaunted)
        {
            var taunts = alive.FindAll(u => u.HasTag(StatusTag.Taunt));
            if (taunts.Count > 0)
                return taunts[Random.Range(0, taunts.Count)];
        }

        return alive[Random.Range(0, alive.Count)];
    }

    // --------- Calcul dégâts ----------

    int ComputeRaw(SlimeUnit user, SlimeUnit target)
    {
        if (user == null || target == null) return 0;

        float mFor = ApplyStatMult(user, StatusAxis.For);
        float mInt = ApplyStatMult(user, StatusAxis.Int);
        float mDef = ApplyStatMult(user, StatusAxis.Def);

        float raw = 0f;
        if (useFor)       raw += user.For * mFor * percentFor;
        if (useInt)       raw += user.Int * mInt * percentInt;
        if (useDef)       raw += user.Def * mDef * percentDef;
        if (usePVMax)     raw += target.PVMax * percentPVMax;
        if (useMissingPV) raw += (user.PVMax - user.PV) * percentMissingPV;

        // Réduction dégâts infligés / reçus (Formation tortue)
        if (user.HasTag(StatusTag.DmgOutDown)) raw *= 0.8f;
        if (target.HasTag(StatusTag.DmgInDown)) raw *= 0.9f;

        return Mathf.Max(0, Mathf.RoundToInt(raw));
    }

    enum StatusAxis { For, Int, Agi, Def }

    float ApplyStatMult(SlimeUnit user, StatusAxis ax)
    {
        float mul = 1f;
        if (user == null || user.statuses == null) return mul;

        foreach (var s in user.statuses)
        {
            if (s == null || !s.IsActive || s.def == null) continue;

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

/// <summary>
/// Petit status concret pour les buffs % ad-hoc (addPercentFor / Int / Agi / Def).
/// </summary>
public class SimplePercentStatus : StatusSO
{
    // On utilise les champs percentFor / percentInt / percentAgi / percentDef hérités.
    // Pas besoin d'ajouter de logique : les méthodes MulFor/MulInt/MulAgi/MulDef
    // de StatusSO suffisent.
}

