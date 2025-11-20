using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CombatResultUI : MonoBehaviour
{
    public static CombatResultUI I;

    [Header("Panels")]
    public GameObject winPanel;   // image + 2 boutons
    public GameObject losePanel;  // image + 1 bouton

    [Header("Win Buttons")]
    public Button winNextButton;
    public Button winBackButton;

    [Header("Lose Buttons")]
    public Button loseBackButton;

    void Awake()
    {
        I = this;
        HideAll();
    }

    void Start()
    {
        if (winNextButton)  winNextButton.onClick.AddListener(OnNextClicked);
        if (winBackButton)  winBackButton.onClick.AddListener(OnBackToLobbyClicked);
        if (loseBackButton) loseBackButton.onClick.AddListener(OnBackToLobbyClicked);
    }

    public void ShowWin()
    {
        HideAll();
        if (winPanel) winPanel.SetActive(true);
    }

    public void ShowLose()
    {
        HideAll();
        if (losePanel) losePanel.SetActive(true);
    }

    public void ShowDraw()
    {
        // pour l’instant : même affichage qu’une défaite
        ShowLose();
    }

    public void HideAll()
    {
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    // ==================== BOUTONS ====================

    void OnNextClicked()
    {
        // On cache l’UI de résultat et on demande au BattleSystem
        // de lancer une nouvelle vague
        HideAll();
        if (BattleSystem.I != null)
        {
            BattleSystem.I.StartNextWave();
        }
        else
        {
            Debug.LogWarning("BattleSystem.I est null, impossible de lancer la prochaine vague.");
        }
    }

    void OnBackToLobbyClicked()
    {
        Debug.Log("Back to lobby → à brancher avec ton système de scènes.");
        // Quand tu auras un lobby :
        // SceneManager.LoadScene("NomDeTaSceneLobby");
    }
}