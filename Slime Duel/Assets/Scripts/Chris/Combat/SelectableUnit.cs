using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SlimeUnit))]
public class SelectableUnit : MonoBehaviour
{
    public SlimeUnit unit;
    public SpriteRenderer sprite;   // si tu utilises SpriteRenderer 2D
    public float pulseSpeed = 6f;
    public Color pulseColor = new Color(1f, 0.85f, 0.2f); // doré

    Coroutine pulseCo;
    Color baseColor = Color.white;

    void Reset()
    {
        unit = GetComponent<SlimeUnit>();
        sprite = GetComponent<SpriteRenderer>();
    }
    void Awake()
    {
        if (!unit) unit = GetComponent<SlimeUnit>();
        if (!sprite) sprite = GetComponent<SpriteRenderer>();
        if (sprite) baseColor = sprite.color;
    }

    void OnMouseDown()
    {
        if (TargetClickSelector.I && TargetClickSelector.I.CanSelect(unit))
            TargetClickSelector.I.Select(unit);
    }

    public void SetHighlight(bool on)
    {
        if (on)
        {
            if (pulseCo == null && sprite) pulseCo = StartCoroutine(Pulse());
        }
        else
        {
            if (pulseCo != null) { StopCoroutine(pulseCo); pulseCo = null; }
            if (sprite) sprite.color = baseColor;
        }
    }

    IEnumerator Pulse()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * pulseSpeed;
            float k = (Mathf.Sin(t) + 1f) * 0.5f; // 0..1
            if (sprite) sprite.color = Color.Lerp(baseColor, pulseColor, k);
            yield return null;
        }
    }
}