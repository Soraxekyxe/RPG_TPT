using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatLogUI : MonoBehaviour
{
    public static CombatLogUI I;

    [Header("UI")]
    public TextMeshProUGUI logText;   // TMP sur l'objet Content
    public ScrollRect scrollRect;     // ScrollRect sur le root
    [Range(1, 500)] public int maxLines = 200;

    [Header("Couleurs (rich text TMP)")]
    public string allyColor    = "#00FF00"; // vert
    public string enemyColor   = "#FF5555"; // rouge
    public string neutralColor = "#FFFFFF"; // blanc
    public string deathColor   = "#FFFFFF"; // blanc

    private readonly List<string> lines = new();

    void Awake()
    {
        I = this;
        if (logText) logText.text = "";
    }

    public void Log(SlimeUnit subject, string message, bool isDeath = false)
    {
        string color = neutralColor;

        if (isDeath)
        {
            color = deathColor;
        }
        else if (subject != null && BattleSystem.I != null)
        {
            bool ally = BattleSystem.I.IsAlly(subject);
            color = ally ? allyColor : enemyColor;
        }

        string colored = $"<color={color}>{message}</color>";
        lines.Add(colored);
        while (lines.Count > maxLines)
            lines.RemoveAt(0);

        if (!logText) return;

        logText.text = string.Join("\n", lines);

        // force la taille du rect du texte
        LayoutRebuilder.ForceRebuildLayoutImmediate(logText.rectTransform);

        // scroll automatiquement tout en bas
        if (scrollRect)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; // 0 = bas
        }
    }
}