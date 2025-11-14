using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MenuSelctionClasse : MonoBehaviour


{
    public SlimeUnit slime;                // Le slime lié à ce panel
    public TextMeshProUGUI classText;      // Affiche la classe actuelle

    void Start()
    {
        UpdateUI();
    }

    public void SetClass(int classIndex)
    {
        Debug.Log("change de classe putin");
        SlimeClass newClass = (SlimeClass)classIndex;
        slime.ChangeClass(newClass);
        UpdateUI();
    }

    void UpdateUI()
    {
        classText.text = slime.classe.ToString();
    }
}
