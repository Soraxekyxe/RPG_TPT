using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class CombatUI : MonoBehaviour
{
    public static CombatUI I;

    [Header("Panels")]
    public GameObject commandPanel;      // panneau avec 3 boutons
    public GameObject skillPanel;        // panneau liste de compétences

    [Header("Command Buttons")]
    public Button attackButton;          // "Attaque"
    public Button skillsButton;          // "Compétences"
    public Button passButton;            // "Passe"

    [Header("Skills Listing")]
    public Transform skillsContainer;    // parent des boutons
    public Button skillButtonPrefab;     // prefab bouton skill

    private readonly List<Button> spawned = new();
    private SlimeUnit current;

    void Awake() { I = this; }

    // ==== Appelé par BattleSystem quand c'est au tour du joueur ====
    public void ShowCommands(SlimeUnit unit)
    {
        current = unit;
        HideAll();

        commandPanel.SetActive(true);

        attackButton.onClick.RemoveAllListeners();
        attackButton.onClick.AddListener(OnAttackClicked);

        skillsButton.onClick.RemoveAllListeners();
        skillsButton.onClick.AddListener(() => ShowSkills(current));

        passButton.onClick.RemoveAllListeners();
        passButton.onClick.AddListener(OnPassClicked);
    }

    // ==== Attaque de base : choisir l'ennemi en cliquant ====
    void OnAttackClicked()
    {
        commandPanel.SetActive(false);

        var enemies = BattleSystem.I.GetEnemiesOf(current).Where(e => e.IsAlive).ToList();
        if (enemies.Count == 0) { BattleSystem.I.EndTurn(); return; }

        TargetClickSelector.I.Begin(enemies, target =>
        {
            HideAll();
            BattleSystem.I.PlayerBasicAttack(current, target);
        });
    }

    // ==== Passe : régénère 30% du mana puis fin du tour ====
    void OnPassClicked()
    {
        commandPanel.SetActive(false);
        current.RestoreManaPercent(0.30f);
        HideAll();
        BattleSystem.I.EndTurn();
    }

    // ==== Onglet Compétences ====
    public void ShowSkills(SlimeUnit unit)
    {
        current = unit;
        ClearSkills();
        commandPanel.SetActive(false);
        skillPanel.SetActive(true);

        foreach (var act in unit.actions)
        {
            if (!act) continue;

            var btn = Instantiate(skillButtonPrefab, skillsContainer);
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (label) label.text = $"{act.actionName} ({act.manaCost})";
            btn.interactable = act.CanPay(unit);

            var cap = act; // capture locale
            btn.onClick.AddListener(() => OnSkillClicked(cap));

            spawned.Add(btn);
        }
    }

    // --- Quand on clique une compétence dans la liste ---
    void OnSkillClicked(ActionSO act)
    {
        // Si la compétence est Single → sélection au clic
        if (act.targetMode == TargetMode.EnemySingle || act.targetMode == TargetMode.AllySingle)
        {
            skillPanel.SetActive(false);
            ClearSkills();

            var candidates = (act.targetMode == TargetMode.EnemySingle)
                ? BattleSystem.I.GetEnemiesOf(current).Where(u => u.IsAlive).ToList()
                : BattleSystem.I.GetAlliesOf(current).Where(u => u.IsAlive).ToList();

            if (candidates.Count == 0) { HideAll(); BattleSystem.I.EndTurn(); return; }

            TargetClickSelector.I.Begin(candidates, chosen =>
            {
                HideAll();
                BattleSystem.I.PlayerCastsSkillOnTarget(current, act, chosen);
            });
            return;
        }

        // Sinon (Self/All/etc.) → exécution directe
        var allies  = BattleSystem.I.GetAlliesOf(current);
        var enemies = BattleSystem.I.GetEnemiesOf(current);
        act.Execute(current, allies, enemies);

        HideAll();
        BattleSystem.I.EndTurn();
    }

    // ==== Utilitaires UI ====
    public void HideAll()
    {
        commandPanel?.SetActive(false);
        skillPanel?.SetActive(false);
        ClearSkills();
        if (TargetClickSelector.I) TargetClickSelector.I.End();
    }

    void ClearSkills()
    {
        foreach (var b in spawned) if (b) Destroy(b.gameObject);
        spawned.Clear();
    }
}


