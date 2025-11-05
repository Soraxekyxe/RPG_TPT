using UnityEngine;
using UnityEngine.SceneManagement;

namespace Yuu
{
    public class ButtonStart : MonoBehaviour
    {
        public void OnStartClick()
        {
            Debug.Log("Bouton Start Appuyé");
            SceneManager.LoadScene("Lobby");
        }

        public void OnExitClick()
        {
#if UNITY_EDITOR
            Debug.Log("QUITTE LA SIMU FRR");
            // quitte playmode quand tu test dans editor
            UnityEditor.EditorApplication.isPlaying = false;
            
#else
        Application.Quit();
#endif
        }
    }
}