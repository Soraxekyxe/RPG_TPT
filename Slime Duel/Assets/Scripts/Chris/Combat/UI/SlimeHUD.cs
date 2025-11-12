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
        // désabonne l'ancien si rebind
        if (unit) unit.Died -= OnUnitDied;

        unit = u;
        if (unit) unit.Died += OnUnitDied;   // << NEW
        TryAutoWire();
        RefreshImmediate();
    }

    void OnDestroy()
    {
        if (unit) unit.Died -= OnUnitDied;   // propre
    }

    private void OnUnitDied(SlimeUnit u)
    {
        Destroy(gameObject);                  // << NEW : le HUD disparaît
    }

    void Awake() => TryAutoWire();

    void TryAutoWire()
    {
        if (!nameText) nameText = transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
        if (!hpText)   hpText   = transform.Find("HPText")?.GetComponent<TextMeshProUGUI>();
        if (!mpText)   mpText   = transform.Find("MPText")?.GetComponent<TextMeshProUGUI>();
        if (!hpSlider) hpSlider = transform.Find("HP")?.GetComponent<Slider>();
        if (!mpSlider) mpSlider = transform.Find("MP")?.GetComponent<Slider>();
    }

    void LateUpdate()
    {
        if (!unit) return;

        if (hpSlider) { hpSlider.maxValue = unit.PVMax; hpSlider.value = unit.PV; }
        if (mpSlider) { mpSlider.maxValue = unit.ManaMax; mpSlider.value = unit.Mana; }

        if (nameText) nameText.text = unit.slimeName;
        if (hpText)   hpText.text   = $"{unit.PV}/{unit.PVMax}";
        if (mpText)   mpText.text   = $"{unit.Mana}/{unit.ManaMax}";
    }

    public void RefreshImmediate() => LateUpdate();
}