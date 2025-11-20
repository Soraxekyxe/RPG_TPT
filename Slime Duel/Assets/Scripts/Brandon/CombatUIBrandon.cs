using UnityEngine;
using UnityEngine.UI;

public class CombatUIBrandon : MonoBehaviour
{
    public Button boutonAttaque;
    public Button boutonCompetence;
    public Text infoText;

    [SerializeField] public PlayerController playerController;

    void Start()
    {
        boutonAttaque.onClick.AddListener(OnAttaqueClicked);
        boutonCompetence.onClick.AddListener(OnCompetenceClicked);
    }
    
    void OnAttaqueClicked()
    {
        infoText.text = "Attaque choisie !";
        playerController.ChoisirAction(0); // 0 = attaque
    }

    void OnCompetenceClicked()
    {
        infoText.text = "Compétence choisie !";
        playerController.ChoisirAction(1); // 1 = compétence
    }
}