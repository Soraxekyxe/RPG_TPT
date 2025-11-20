using UnityEngine;

namespace Yuu
{
    public class GameStartManager : MonoBehaviour
    {
        [Tooltip("Panel de sélection du Slime")]
        public GameObject selectionPanel;

        [Tooltip("Panel ou contenu du lobby principal")]
        public GameObject lobbyPanel;

        void Start()
        {
            if (PlayerPrefs.GetInt("SelectionDone", 0) == 1)
            {
                selectionPanel.SetActive(false);
                lobbyPanel.SetActive(true);
            }
            else
            {
                selectionPanel.SetActive(true);
                lobbyPanel.SetActive(false);
            }
        }
        public void ValiderSelection()
        {
            PlayerPrefs.SetInt("SelectionDone", 1);
            PlayerPrefs.Save();

            selectionPanel.SetActive(false);
            lobbyPanel.SetActive(true);

            Debug.Log("Sélection validée, panel de sélection fermé !");
        }
        
    }
}