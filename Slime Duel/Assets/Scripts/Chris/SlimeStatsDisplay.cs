using UnityEngine;
using TMPro;

public class SlimeStatsDisplay : MonoBehaviour
{
    [Header("Référence UI (TextMeshPro) pour CE slime")]
    public TextMeshProUGUI statsText;

    private Slime slime;

    void Awake()
    {
        slime = GetComponent<Slime>();
        if (slime == null)
            Debug.LogError("Aucun script 'Slime' trouvé sur " + gameObject.name);
        if (statsText == null)
            Debug.LogError("Assigne un TextMeshProUGUI dans 'statsText' sur " + gameObject.name);
    }

    void Update()
    {
        if (slime == null || statsText == null) return;

        statsText.text =
            $"<b>{slime.name} — {slime.classe}</b>\n" +
            $"PV : {slime.PV}\n" +
            $"Mana : {slime.Mana}\n" +
            $"Agi : {slime.Agi}\n" +
            $"For : {slime.For}\n" +
            $"Int : {slime.Int}\n" +
            $"Def : {slime.Def}";
    }
}