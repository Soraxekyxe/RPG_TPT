using UnityEngine;
using Yuu;

public class SlimeSelection : MonoBehaviour
{
    public SlimeUnit slimeActif;

    int slimeChoisis = 0;

    public GameObject panel; // ton panel UI main
    public GameStartManager startManager;

    public void SetSlime(SlimeUnit s)
    {
        slimeActif = s;
    }

    public void ChoisirClasse(int classeIndex)
    {
        if (slimeActif == null) return;

        slimeActif.classe = (SlimeClass)classeIndex;
        slimeActif.SendMessage("InitialiserStats");

        slimeChoisis++;

        if(slimeChoisis >= 3)
        {
            FermerPanel();
        }
    }

    void FermerPanel()
    {
        startManager.ValiderSelection();
        panel.SetActive(false);
        Debug.Log("tout les slimes sot choisit il se ferme le panel");
    }
}