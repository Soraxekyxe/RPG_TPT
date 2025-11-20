using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    // Références aux listes de skills par classe
    public List<ActionSO> guerrierSkills;
    public List<ActionSO> mageSkills;
    public List<ActionSO> assassinSkills;
    public List<ActionSO> clercSkills;
    public List<ActionSO> druideSkills;

    void Start()
    {
        foreach (var slime in FindObjectsOfType<SlimeUnit>())
        {
            AssignSkills(slime);
        }
    }
    

    public void AssignSkills(SlimeUnit slime)
    {
        
        slime.actions.Clear();
        switch (slime.classe)
        {
            case SlimeClass.Guerrier:
                slime.actions.AddRange(guerrierSkills.Where(skill=>skill.IsUnlocked(slime)));
                break;
            case SlimeClass.Mage:
                slime.actions.AddRange(mageSkills.Where(skill=>skill.IsUnlocked(slime)));
                break;
            case SlimeClass.Assassin:
                slime.actions.AddRange(assassinSkills.Where(skil =>skil.IsUnlocked(slime)));
                break;
            case SlimeClass.Clerc:
                slime.actions.AddRange(clercSkills.Where(skill =>skill.IsUnlocked(slime)));
                break;
            case SlimeClass.Druide:
                slime.actions.AddRange(druideSkills.Where(skill =>skill.IsUnlocked(slime)));
                break;
        }

        Debug.Log($"{slime.slimeName} a reçu {slime.actions.Count} compétences pour la classe {slime.classe}");
    }
}
