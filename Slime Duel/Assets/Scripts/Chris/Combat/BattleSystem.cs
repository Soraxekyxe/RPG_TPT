using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem I;

    void Awake() => I = this;

    [Header("Équipe A (Joueur)")]
    public SlimeUnit[] teamA = new SlimeUnit[3];
    [Header("Équipe B (IA)")]
    public SlimeUnit[] teamB = new SlimeUnit[3];

    private readonly List<SlimeUnit> turnOrder = new();
    private int turnIndex = -1;
    public SlimeUnit Active { get; private set; }

    // Bloque Update pendant le choix du joueur
    private bool waitingForPlayer = false;

    void Start()
    {
        BuildTurnOrder();
        NextTurnStart();
    }

    void Update()
    {
        // Si on attend le joueur → ne rien faire
        if (Active == null || waitingForPlayer) return;

        // Stun / Root => passe le tour
        if (Active.HasTag(StatusTag.Stun) || Active.HasTag(StatusTag.Root))
        {
            Debug.Log($"{Active.slimeName} ne peut pas jouer !");
            CombatLogUI.I?.Log(Active, $"{Active.slimeName} ne peut pas jouer !");
            EndTurn();
            return;
        }

        // Si c’est un slime joueur : attendre l'UI
        if (teamA.Contains(Active)) return;

        // ---------- IA ----------
        var usable = Active.actions.Where(a => a != null && a.CanPay(Active)).ToList();
        if (usable.Count == 0)
        {
            Debug.Log($"{Active.slimeName} ne peut rien faire.");
            CombatLogUI.I?.Log(Active, $"{Active.slimeName} ne peut rien faire.");
            EndTurn();
            return;
        }

        var act = usable[Random.Range(0, usable.Count)];

        var allies  = teamB.Contains(Active) ? teamB.ToList() : teamA.ToList();
        var enemies = teamB.Contains(Active) ? teamA.ToList() : teamB.ToList();

        CombatLogUI.I?.Log(Active, $"{Active.slimeName} utilise {act.actionName}");
        act.Execute(Active, allies, enemies);
        EndTurn();
    }

    // ======== Tour suivant ========
    void NextTurnStart()
    {
        if (IsBattleOver())
        {
            var res = TeamAlive(teamA) ? "ÉQUIPE A GAGNE" : TeamAlive(teamB) ? "ÉQUIPE B GAGNE" : "Match nul";
            Debug.Log($"=== FIN DU COMBAT — {res} ===");
            CombatLogUI.I?.Log(null, res); // neutre
            enabled = false;
            return;
        }

        do
        {
            turnIndex = (turnIndex + 1) % turnOrder.Count;
            Active = turnOrder[turnIndex];
        } 
        while (Active == null || !Active.IsAlive);

        Active.TickStartOfTurn();
        Debug.Log($"--- Tour de {Active.slimeName} ({Active.classe}) ---");
        CombatLogUI.I?.Log(Active, $"Tour de {Active.slimeName}");

        // Tour joueur → afficher commandes
        if (teamA.Contains(Active))
        {
            waitingForPlayer = true;
            CombatUI.I.ShowCommands(Active);
        }
    }

    public void EndTurn()
    {
        Active.TickEndOfTurn();
        waitingForPlayer = false;
        NextTurnStart();
    }

    // ======== Setup de l'ordre ========
    void BuildTurnOrder()
    {
        turnOrder.Clear();
        var all = teamA.Concat(teamB).Where(u => u != null).ToList();
        turnOrder.AddRange(all.OrderByDescending(u => u.Agi));
        turnIndex = -1;
    }

    bool TeamAlive(SlimeUnit[] t) => t.Any(u => u && u.IsAlive);
    bool IsBattleOver() => !TeamAlive(teamA) || !TeamAlive(teamB);

    // ======== SÉLECTION DE CIBLES POUR UI (ancienne API) ========
    public List<SlimeUnit> GetValidTargetsFor(ActionSO act, SlimeUnit user)
    {
        var allies  = teamA.Contains(user) ? teamA.ToList() : teamB.ToList();
        var enemies = teamA.Contains(user) ? teamB.ToList() : teamA.ToList();

        switch (act.targetMode)
        {
            case TargetMode.Self:        return new List<SlimeUnit> { user };
            case TargetMode.AllySingle:  return allies.Where(a => a && a.IsAlive).ToList();
            case TargetMode.AllyAll:     return allies.Where(a => a && a.IsAlive).ToList();
            case TargetMode.EnemySingle: return enemies.Where(e => e && e.IsAlive).ToList();
            case TargetMode.EnemyAll:    return enemies.Where(e => e && e.IsAlive).ToList();
        }

        return new List<SlimeUnit> { user };
    }

    // ======== Appelé par l'UI dans l'ancienne logique (multi-cible / self) ========
    public void PlayerCastsSkill(SlimeUnit user, ActionSO act, SlimeUnit target)
    {
        var allies  = teamA.Contains(user) ? teamA.ToList() : teamB.ToList();
        var enemies = teamA.Contains(user) ? teamB.ToList() : teamA.ToList();

        Debug.Log($"🎯 {user.slimeName} lance {act.actionName}");
        CombatLogUI.I?.Log(user, $"{user.slimeName} utilise {act.actionName}");

        act.Execute(user, allies, enemies);
        EndTurn();
    }

    // ======== Mort d'une unité ========
    public void OnUnitDied(SlimeUnit u)
    {
        // Si le joueur était en train de choisir une action et que l’actif meurt
        if (Active == u)
        {
            CombatUI.I?.HideAll();
            EndTurn(); // passe au suivant sans planter
        }
    }

    // ======== Helpers équipes ========
    public List<SlimeUnit> GetAlliesOf(SlimeUnit u)
        => (teamA.Contains(u) ? teamA : teamB).Where(x => x != null).ToList();

    public List<SlimeUnit> GetEnemiesOf(SlimeUnit u)
        => (teamA.Contains(u) ? teamB : teamA).Where(x => x != null).ToList();

    public bool IsAlly(SlimeUnit u)
        => teamA != null && System.Array.IndexOf(teamA, u) >= 0;

    // ======== Attaque de base joueur ========
    public void PlayerBasicAttack(SlimeUnit user, SlimeUnit target)
    {
        int raw = Mathf.Max(1, user.For); // simple: dégâts = For
        int dealt = target.TakeDamage(raw, DamageKind.Physical);
        Debug.Log($"{user.slimeName} attaque {target.slimeName} ({dealt} dmg)");

        CombatLogUI.I?.Log(user, $"{user.slimeName} attaque {target.slimeName} ({dealt} dégâts)");

        EndTurn();
    }

    // ======== Compétence mono-cible avec cible choisie ========
    public void PlayerCastsSkillOnTarget(SlimeUnit user, ActionSO act, SlimeUnit target)
    {
        var allies  = GetAlliesOf(user);
        var enemies = GetEnemiesOf(user);

        CombatLogUI.I?.Log(user, $"{user.slimeName} utilise {act.actionName} sur {target.slimeName}");

        act.ExecuteOnTarget(user, target, allies, enemies);
        EndTurn();
    }
}





