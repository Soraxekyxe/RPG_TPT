
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatManager : MonoBehaviour
{
    [Header("Slimes du joueur")]
    public List<SlimeUnit> playerUnits;

    [Header("Slimes ennemis")]
    public List<SlimeUnit> enemyUnits;

    [Header("Boss (optionnel)")]
    public SlimeUnit bossUnit;

    [Header("Référence au PlayerController")]
    public PlayerController playerController;

    public List<CombatUnit> allUnits = new();

    void Start()
    {
        // Initialisation des unités
        foreach (var slime in playerUnits)
        {
            allUnits.Add(new CombatUnit(slime, UnitRole.Player));
        }

        foreach (var slime in enemyUnits)
        {
            allUnits.Add(new CombatUnit(slime, UnitRole.Monster));
        }

        if (bossUnit != null)
        {
            allUnits.Add(new CombatUnit(bossUnit, UnitRole.Boss));
        }

        StartCoroutine(CombatLoop());
    }

    IEnumerator CombatLoop()
    {
        while (!IsCombatOver())
        {
            var turnOrder = CalculateTurnOrder();

            foreach (var combatUnit in turnOrder)
            {
                if (!combatUnit.IsAlive) continue;

                combatUnit.unit.TickStartOfTurn();

                if (combatUnit.role == UnitRole.Player)
                {
                    yield return StartCoroutine(playerController.PlayTurn(
                        combatUnit.unit,
                        GetAllies(combatUnit),
                        GetEnemies(combatUnit)
                    ));
                }
                else
                {
                    yield return StartCoroutine(EnemyAI.PlayTurn(
                        combatUnit.unit,
                        GetAllies(combatUnit),
                        GetEnemies(combatUnit)
                    ));
                }

                combatUnit.unit.TickEndOfTurn();
            }
        }

        EndCombat();
    }

    List<CombatUnit> CalculateTurnOrder()
    {
        return allUnits
            .Where(u => u.IsAlive)
            .OrderByDescending(u => u.Agi)
            .ThenBy(u => GetPriorityValue(u.role))
            .ToList();
    }

    int GetPriorityValue(UnitRole role)
    {
        switch (role)
        {
            case UnitRole.Boss: return 0;
            case UnitRole.Player: return 1;
            case UnitRole.Monster: return 2;
            default: return 3;
        }
    }

    List<SlimeUnit> GetAllies(CombatUnit unit)
    {
        return allUnits
            .Where(u => u.role == unit.role && u != unit && u.IsAlive)
            .Select(u => u.unit)
            .ToList();
    }

    List<SlimeUnit> GetEnemies(CombatUnit unit)
    {
        return allUnits
            .Where(u => u.role != unit.role && u.IsAlive)
            .Select(u => u.unit)
            .ToList();
    }

    bool IsCombatOver()
    {
        bool playerAlive = allUnits.Any(u => u.role == UnitRole.Player && u.IsAlive);
        bool enemyAlive = allUnits.Any(u => u.role != UnitRole.Player && u.IsAlive);
        return !playerAlive || !enemyAlive;
    }

    void EndCombat()
    {
        Debug.Log("Combat terminé !");
    }
}
