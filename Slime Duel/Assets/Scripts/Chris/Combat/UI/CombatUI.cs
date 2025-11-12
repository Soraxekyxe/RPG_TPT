using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CombatUI : MonoBehaviour
{
    public static CombatUI I;

    [Header("Panels")]
    public GameObject skillPanel;
    public GameObject targetPanel;

    [Header("Prefabs")]
    public Button skillButtonPrefab;
    public Button targetButtonPrefab;

    List<Button> tempButtons = new();
    SlimeUnit currentUnit;
    ActionSO pendingSkill;

    void Awake() { I = this; }

    public void ShowSkills(SlimeUnit unit)
    {
        ClearButtons();
        currentUnit = unit;
        pendingSkill = null;

        skillPanel.SetActive(true);
        targetPanel.SetActive(false);

        foreach (var act in unit.actions)
        {
            // Affiche tout et grise si non jouable (plus ergonomique)
            var btn = Instantiate(skillButtonPrefab, skillPanel.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = $"{act.actionName} ({act.manaCost})";
            btn.interactable = act.CanPay(unit);

            btn.onClick.AddListener(() => SelectSkill(act));
            tempButtons.Add(btn);
        }
    }

    void SelectSkill(ActionSO act)
    {
        pendingSkill = act;
        skillPanel.SetActive(false);

        // Résout les cibles possibles
        List<SlimeUnit> targets = BattleSystem.I.GetValidTargetsFor(act, currentUnit);

        // Si aucune cible à choisir (Self / All / ou 1 seule option) → exécuter direct
        bool needsTargetSelection =
            (act.targetMode == TargetMode.AllySingle || act.targetMode == TargetMode.EnemySingle)
            && targets.Count > 1;

        if (!needsTargetSelection)
        {
            targetPanel.SetActive(false);
            ClearButtons();

            SlimeUnit t = targets.Count > 0 ? targets[0] : null; // optionnel
            BattleSystem.I.PlayerCastsSkill(currentUnit, pendingSkill, t);
            return;
        }

        // Sinon: afficher la liste de cibles
        ClearButtons();
        targetPanel.SetActive(true);

        foreach (var t in targets)
        {
            var btn = Instantiate(targetButtonPrefab, targetPanel.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = t.slimeName;
            btn.onClick.AddListener(() => ExecuteSkill(t));
            tempButtons.Add(btn);
        }
    }

    void ExecuteSkill(SlimeUnit t)
    {
        ClearButtons();
        skillPanel.SetActive(false);
        targetPanel.SetActive(false);

        BattleSystem.I.PlayerCastsSkill(currentUnit, pendingSkill, t);
    }

    void ClearButtons()
    {
        foreach (var b in tempButtons) if (b) Destroy(b.gameObject);
        tempButtons.Clear();
    }
    
    public void HideAll()
    {
        // ferme et nettoie l’UI proprement
        foreach (var b in tempButtons) if (b) Destroy(b.gameObject);
        tempButtons.Clear();
        if (skillPanel)  skillPanel.SetActive(false);
        if (targetPanel) targetPanel.SetActive(false);
    }

}
