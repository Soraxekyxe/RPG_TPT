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
    public Image itemIcon;   // <<< icône de l'objet

    public void Bind(SlimeUnit u)
    {
        // désabonne l'ancien si rebind
        if (unit) unit.Died -= OnUnitDied;

        unit = u;
        if (unit) unit.Died += OnUnitDied;

        TryAutoWire();
        RefreshImmediate();
    }

    void OnDestroy()
    {
        if (unit) unit.Died -= OnUnitDied;
    }

    private void OnUnitDied(SlimeUnit u)
    {
        Destroy(gameObject);   // le HUD disparaît quand le slime meurt
    }

    void Awake() => TryAutoWire();

    void TryAutoWire()
    {
        if (!nameText) nameText = transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
        if (!hpText)   hpText   = transform.Find("HPText")?.GetComponent<TextMeshProUGUI>();
        if (!mpText)   mpText   = transform.Find("MPText")?.GetComponent<TextMeshProUGUI>();
        if (!hpSlider) hpSlider = transform.Find("HP")?.GetComponent<Slider>();
        if (!mpSlider) mpSlider = transform.Find("MP")?.GetComponent<Slider>();
        if (!itemIcon) itemIcon = transform.Find("ItemIcon")?.GetComponent<Image>(); // <<< important
    }

    void LateUpdate()
    {
        if (!unit) return;

        // Sliders HP / MP
        if (hpSlider)
        {
            hpSlider.maxValue = unit.PVMax;
            hpSlider.value    = unit.PV;
        }

        if (mpSlider)
        {
            mpSlider.maxValue = unit.ManaMax;
            mpSlider.value    = unit.Mana;
        }

        // Nom + niveau + classe
        if (nameText)
            nameText.text = $"{unit.slimeName}  Lv.{unit.Lvl} — {unit.classe}";

        if (hpText)
            hpText.text = $"{unit.PV}/{unit.PVMax}";

        if (mpText)
            mpText.text = $"{unit.Mana}/{unit.ManaMax}";

        // Icône d'objet
        if (itemIcon)
        {
            var item = unit.equippedItem;
            if (item != null && item.icon != null)
            {
                itemIcon.sprite  = item.icon;
                itemIcon.enabled = true;
            }
            else
            {
                itemIcon.enabled = false;
            }
        }
    }

    public void RefreshImmediate() => LateUpdate();
}
