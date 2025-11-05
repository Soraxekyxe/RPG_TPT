using UnityEngine;

public class SlimeStatsDisplay : MonoBehaviour
{
    private Slime slime;

    void Start()
    {
        slime = GetComponent<Slime>();
        if (slime == null)
            Debug.LogError("Aucun script 'Slime' trouvé sur " + gameObject.name);
    }

    void OnMouseEnter()
    {
        if (slime == null) return;

        Debug.Log(
            $"--- {slime.classe} ---\n" +
            $"PV : {slime.PV}\n" +
            $"Mana : {slime.Mana}\n" +
            $"Agilité : {slime.Agi}\n" +
            $"Force : {slime.For}\n" +
            $"Intelligence : {slime.Int}\n" +
            $"Défense : {slime.Def}"
        );
    }
}