using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public BattleSystem battleSystem;

    void Start()
    {
        foreach (var ennemie in battleSystem.teamB)
        {
            if (ennemie != null)
                ennemie.Died += OnEnnemyDied;
        }
    }

    void OnEnnemyDied(SlimeUnit deadEnnemie)
    {
        int ExpGain = 50;

        Debug.Log($"+{ExpGain} XP pour l'équipe A.");


        foreach (var ally in battleSystem.teamA)
        {
            if (ally != null && ally.IsAlive)
            {
                AddExperience(ally, ExpGain);
            }
        }
    }

    void AddExperience(SlimeUnit slime, int amount)
    {

        slime.CurrentExp += amount;

        // Gestion des montées de niveau multiples
        while (slime.CurrentExp >= slime.NextLvl)
        {
            slime.CurrentExp -= slime.NextLvl; // Consomme l'XP pour le niveau
            slime.LvlUp();
            slime.NextLvl = Mathf.CeilToInt(slime.NextLvl * 1.5f); // Augmente la difficulté
            Debug.Log($"{slime.slimeName} passe au niveau {slime.Lvl} !");

        }
    }
}
