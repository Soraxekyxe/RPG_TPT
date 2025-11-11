using UnityEngine;

// Statut "one-shot" qui nettoie tous les autres statuts au début du tour,
// puis se retire immédiatement.
[CreateAssetMenu(menuName = "SlimeGame/Status/CleanseOnce")]
public class Status_CleanseOnce : StatusSO
{
    private void OnEnable()
    {
        statusName = "Purification";
        tags = StatusTag.None;
    }

    public override void OnTurnStart(SlimeUnit owner, StatusInstance inst)
    {
        // Supprime tous les autres statuts actifs
        owner.statuses.Clear();

        // Se retire lui-même
        inst.turns = 0;

        Debug.Log($"{owner.slimeName} est purifié de tous ses statuts.");
    }
}