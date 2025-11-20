using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource sfxSource;

    [Header("Sounds effects")]
    public AudioClip[] naviagationscenes;
    public AudioClip[] pause;
    public AudioClip[] depause;
    public AudioClip[] lancementjeu;
    public AudioClip[] machinepull;
    public AudioClip[] equipslime;
    public AudioClip[] ouvertureinventaire;


    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}