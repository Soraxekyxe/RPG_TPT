using UnityEngine;

public class SelectionSlimeUI : MonoBehaviour
{
    public SlimeSelection manager; // drag dans inspector

    public void SetSlime(SlimeUnit s)
    {
        manager.SetSlime(s);
    }

    public void ChoisirClasse(int classeIndex)
    {
        manager.ChoisirClasse(classeIndex);
    }
}