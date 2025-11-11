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
            if (!act.CanPay(unit)) continue;

            var btn = Instantiate(skillButtonPrefab, skillPanel.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = act.actionName;

            btn.onClick.AddListener(() => SelectSkill(act));
            tempButtons.Add(btn);
        }
    }

    void SelectSkill(ActionSO act)
    {
        pendingSkill = act;
        skillPanel.SetActive(false);

        ClearButtons();
        targetPanel.SetActive(true);

        List<SlimeUnit> targets = BattleSystem.I.GetValidTargetsFor(act, currentUnit);

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
        foreach (var b in tempButtons) Destroy(b.gameObject);
        tempButtons.Clear();
    }
}