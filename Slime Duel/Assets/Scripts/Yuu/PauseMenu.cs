using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    [SerializeField] private AudioClip pauseSoundClip;

    private MusicManager musicManager;

    private void Awake()
    {
        musicManager = FindObjectOfType<MusicManager>();

        if (musicManager == null)
            Debug.LogError("Aucun MusicManager trouvé dans la scène !");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            musicManager.PlaySFX(pauseSoundClip);

            Debug.Log("TES EN PAUSE");

            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        Debug.Log("REPRENDS LA PARTIE");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Restart()
    {
        Debug.Log("RECOMMANCE TAFANARI");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}