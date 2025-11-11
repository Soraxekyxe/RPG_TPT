using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI
{
    public static IEnumerator PlayTurn(SlimeUnit enemy, List<SlimeUnit> allies, List<SlimeUnit> enemies)
    {
        yield return new WaitForSeconds(0.5f); // petite pause pour le rythme

        if (!enemy.IsAlive) yield break;

        // Essaye d'utiliser une compétence
        foreach (var action in enemy.actions)
        {
            if (action.CanPay(enemy))
            {
                action.Execute(enemy, allies, enemies);
                yield break;
            }
        }

        // Sinon, attaque de base
        var target = PickTarget(enemies);
        if (target != null)
        {
            BasicAttack(enemy, target);
        }
    }

    static SlimeUnit PickTarget(List<SlimeUnit> enemies)
    {
        return enemies.Find(e => e.IsAlive && !e.HasTag(StatusTag.Untargetable));
    }

    static void BasicAttack(SlimeUnit attacker, SlimeUnit target)
    {
        int raw = Mathf.RoundToInt(attacker.For * (100f / (100f + target.Def)));
        int dmg = target.TakeDamage(raw, DamageKind.Physical);
        Debug.Log($"{attacker.slimeName} attaque {target.slimeName} pour {dmg} dégâts.");
    }
}