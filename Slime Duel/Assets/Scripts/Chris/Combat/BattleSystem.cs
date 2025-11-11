using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem I;

    void Awake() => I = this;
    [Header("Équipe A (3)")]
    public SlimeUnit[] teamA = new SlimeUnit[3];
    [Header("Équipe B (3)")]
    public SlimeUnit[] teamB = new SlimeUnit[3];

    private readonly List<SlimeUnit> turnOrder = new();
    private int turnIndex = -1;
    public SlimeUnit Active { get; private set; }

    void Start()
    {
        BuildTurnOrder();
        NextTurnStart();
    }

    void Update()
    {
        if (Active == null) return;

        // Stun / Root => skip
        if (Active.HasTag(StatusTag.Stun) || Active.HasTag(StatusTag.Root))
        {
            Debug.Log($"{Active.slimeName} est étourdi/enraciné.");
            EndTurn();
            return;
        }

        // ✅ Si c'est un slime du joueur → attendre UI → pas d'IA
        if (teamA.Contains(Active))
            return;

        // ✅ Sinon IA joue toute seule
        var usable = Active.actions.Where(a => a != null && a.CanPay(Active)).ToList();
        if (usable.Count == 0)
        {
            Debug.Log($"{Active.slimeName} n'a rien à jouer.");
            EndTurn();
            return;
        }

        var act = usable[Random.Range(0, usable.Count)];
        var allies = teamA.Contains(Active) ? teamA.ToList() : teamB.ToList();
        var enemies = teamA.Contains(Active) ? teamB.ToList() : teamA.ToList();

        act.Execute(Active, allies, enemies);
        EndTurn();
    }


    void BuildTurnOrder()
    {
        turnOrder.Clear();
        var all = teamA.Concat(teamB).Where(u => u != null).ToList();
        turnOrder.AddRange(all.OrderByDescending(u => u.Agi));
        turnIndex = -1;
    }

    void NextTurnStart()
    {
        if (IsBattleOver())
        {
            var res = TeamAlive(teamA) ? "Équipe A gagne" : TeamAlive(teamB) ? "Équipe B gagne" : "Nul";
            Debug.Log($"=== FIN — {res} ===");
            enabled = false;
            return;
        }

        do
        {
            turnIndex = (turnIndex + 1) % turnOrder.Count;
            Active = turnOrder[turnIndex];
        } while (Active == null || !Active.IsAlive);

        Active.TickStartOfTurn();
        Debug.Log($"--- Tour de {Active.slimeName} ({Active.classe}) ---");
    }

    void EndTurn()
    {
        Active.TickEndOfTurn();
        NextTurnStart();
    }

    bool TeamAlive(SlimeUnit[] t) => t.Any(u => u && u.IsAlive);
    bool IsBattleOver() => !TeamAlive(teamA) || !TeamAlive(teamB);
    
    // ================= PLAYER TURN SUPPORT =================

    public List<SlimeUnit> GetValidTargetsFor(ActionSO act, SlimeUnit user)
    {
        var allies = teamA.Contains(user) ? teamA.ToList() : teamB.ToList();
        var enemies = teamA.Contains(user) ? teamB.ToList() : teamA.ToList();

        switch (act.targetMode)
        {
            case TargetMode.Self:
                return new List<SlimeUnit> { user };

            case TargetMode.AllySingle:
                return allies.Where(a => a != null && a.IsAlive).ToList();

            case TargetMode.AllyAll:
                return allies.Where(a => a != null && a.IsAlive).ToList();

            case TargetMode.EnemySingle:
                return enemies.Where(e => e != null && e.IsAlive).ToList();

            case TargetMode.EnemyAll:
                return enemies.Where(e => e != null && e.IsAlive).ToList();
        }

        return new List<SlimeUnit> { user };
    }

    public void PlayerCastsSkill(SlimeUnit user, ActionSO act, SlimeUnit target)
    {
        var allies = teamA.Contains(user) ? teamA.ToList() : teamB.ToList();
        var enemies = teamA.Contains(user) ? teamB.ToList() : teamA.ToList();

        Debug.Log($"🎯 {user.slimeName} utilise {act.actionName} sur {target.slimeName}");

        // IMPORTANT : on appelle l'Execute original (3 paramètres)
        act.Execute(user, allies, enemies);

        EndTurn();
    }

}


