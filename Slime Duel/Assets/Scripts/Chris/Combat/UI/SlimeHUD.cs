using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlimeHUD : MonoBehaviour
{
    [Header("Binding (laisse vide, auto si noms par défaut)")]
    public SlimeUnit unit;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public Slider hpSlider;
    public Slider mpSlider;

    public void Bind(SlimeUnit u)
    {
        unit = u;
        TryAutoWire();
        RefreshImmediate();
    }

    void Awake() => TryAutoWire();

    void TryAutoWire()
    {
        // si on laisse vide, on tente par noms standard dans le prefab
        if (!nameText) nameText = transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
        if (!hpText)   hpText   = transform.Find("HPText")?.GetComponent<TextMeshProUGUI>();
        if (!mpText)   mpText   = transform.Find("MPText")?.GetComponent<TextMeshProUGUI>();
        if (!hpSlider) hpSlider = transform.Find("HP")?.GetComponent<Slider>();
        if (!mpSlider) mpSlider = transform.Find("MP")?.GetComponent<Slider>();
    }

    void LateUpdate()
    {
        if (!unit) return;

        if (hpSlider)
        {
            if (hpSlider.maxValue != unit.PVMax) hpSlider.maxValue = unit.PVMax;
            if (hpSlider.value    != unit.PV)    hpSlider.value    = unit.PV;
        }
        if (mpSlider)
        {
            if (mpSlider.value != unit.Mana) mpSlider.value = unit.Mana;
        }

        if (nameText) nameText.text = unit.slimeName;
        if (hpText)   hpText.text   = $"{unit.PV}/{unit.PVMax}";
        if (mpText)   mpText.text   = $"{unit.Mana}";
    }

    public void RefreshImmediate() => LateUpdate();
}