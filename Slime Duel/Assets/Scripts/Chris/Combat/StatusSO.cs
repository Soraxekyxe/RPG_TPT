using UnityEngine;

[System.Flags]
public enum StatusTag
{
    None=0,
    Stun=1<<0,          // ne joue pas
    Taunt=1<<1,         // force les ennemis à te viser
    Untargetable=1<<2,  // ne peut pas être ciblé
    Confused=1<<3,      // cible aléatoire
    Root=1<<4,          // ne peut pas agir (ou mouvement si tu en ajoutes)
    Burn=1<<5,          // DOT feu
    Bleed=1<<6,         // DOT saignement
    Immunity=1<<7,      // immunisé aux statuts
    DmgOutDown=1<<8,    // réduction dégâts infligés
    DmgInDown=1<<9      // réduction dégâts reçus
}

public abstract class StatusSO : ScriptableObject
{
    public string statusName;
    public StatusTag tags;
    [Range(-1f,1f)] public float percentFor, percentInt, percentAgi, percentDef; // ±% sur stats
    [Range(0f, 1f)] public float dotPercentPVMax; // DoT % PV max par tour

    public bool HasTag(StatusTag t) => (tags & t) != 0;

    public virtual void OnTurnStart(SlimeUnit owner, StatusInstance inst)
    {
        // DOT début de tour
        if (dotPercentPVMax > 0f && owner.IsAlive)
        {
            int dmg = Mathf.CeilToInt(owner.PVMax * dotPercentPVMax * Mathf.Max(1, inst.stacks));
            owner.TakeDamage(dmg, DamageKind.True);
            Debug.Log($"{owner.slimeName} subit {dmg} DOT ({statusName})");
        }
    }

    public virtual void OnTurnEnd(SlimeUnit owner, StatusInstance inst) { }

    // Helpers appliqués dans les calculs d’actions :
    public float MulFor() => 1f + percentFor;
    public float MulInt() => 1f + percentInt;
    public float MulAgi() => 1f + percentAgi;
    public float MulDef() => 1f + percentDef;
}