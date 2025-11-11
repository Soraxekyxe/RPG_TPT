
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Button boutonAttaque;
    public Button boutonCompetence;
    public Text infoText;

    private int choixAction = -1;

    public IEnumerator PlayTurn(SlimeUnit unit, List<SlimeUnit> allies, List<SlimeUnit> enemies)
    {
        choixAction = -1;
        infoText.text = $"Tour de {unit.slimeName} ! Choisissez une action.";

        boutonAttaque.gameObject.SetActive(true);
        boutonCompetence.gameObject.SetActive(true);

        boutonAttaque.onClick.RemoveAllListeners();
        boutonCompetence.onClick.RemoveAllListeners();

        boutonAttaque.onClick.AddListener(() => ChoisirAction(0));
        boutonCompetence.onClick.AddListener(() => ChoisirAction(1));

        while (choixAction == -1)
        {
            yield return null;
        }

        boutonAttaque.gameObject.SetActive(false);
        boutonCompetence.gameObject.SetActive(false);

        if (choixAction == 0)
        {
            var target = PickTarget(enemies);
            if (target != null)
            {
                BasicAttack(unit, target);
                infoText.text = $"{unit.slimeName} attaque {target.slimeName} !";
            }
        }
        else if (choixAction == 1)
        {
            var usableActions = unit.actions.FindAll(a => a.CanPay(unit));
            if (usableActions.Count > 0)
            {
                usableActions[0].Execute(unit, allies, enemies); // à améliorer plus tard
                infoText.text = $"{unit.slimeName} utilise {usableActions[0].actionName} !";
            }
            else
            {
                infoText.text = "Pas de compétence utilisable !";
            }
        }
    }

    public void ChoisirAction(int choix)
    {
        choixAction = choix;
    }

    SlimeUnit PickTarget(List<SlimeUnit> enemies)
    {
        return enemies.Find(e => e.IsAlive && !e.HasTag(StatusTag.Untargetable));
    }

    void BasicAttack(SlimeUnit attacker, SlimeUnit target)
    {
        int raw = Mathf.RoundToInt(attacker.For * (100f / (100f + target.Def)));
        int dmg = target.TakeDamage(raw, DamageKind.Physical);
        Debug.Log($"{attacker.slimeName} attaque {target.slimeName} pour {dmg} dégâts.");
    }
}
