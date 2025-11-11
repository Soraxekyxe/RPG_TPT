using System.Collections.Generic;
using UnityEngine;

public class HUDTeamPanels : MonoBehaviour
{
    [Header("Références")]
    public BattleSystem battle;          // peut rester vide, on auto-trouve
    public RectTransform panelTeamA;     // un RectTransform avec VerticalLayoutGroup
    public RectTransform panelTeamB;     // idem
    public SlimeHUD hudPrefab;

    readonly List<SlimeHUD> spawned = new();

    void Awake()
    {
        if (!battle) battle = BattleSystem.I ?? FindObjectOfType<BattleSystem>();
    }

    void Start()
    {
        if (!battle || !hudPrefab || !panelTeamA || !panelTeamB)
        {
            Debug.LogError("HUDTeamPanels: références manquantes.");
            return;
        }

        SpawnForTeam(battle.teamA, panelTeamA);
        SpawnForTeam(battle.teamB, panelTeamB);
    }

    void SpawnForTeam(SlimeUnit[] team, RectTransform parent)
    {
        foreach (var u in team)
        {
            if (!u) continue;
            var hud = Instantiate(hudPrefab, parent);
            hud.Bind(u);
            spawned.Add(hud);
        }
    }
}