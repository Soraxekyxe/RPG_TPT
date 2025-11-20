using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetClickSelector : MonoBehaviour
{
    public static TargetClickSelector I;
    void Awake(){ I = this; }

    HashSet<SlimeUnit> candidates = new();
    Action<SlimeUnit> onChosen;
    public bool IsActive { get; private set; }

    public void Begin(IEnumerable<SlimeUnit> units, Action<SlimeUnit> onChosen)
    {
        End();
        IsActive = true;
        this.onChosen = onChosen;
        candidates = new HashSet<SlimeUnit>(units);

        foreach (var u in candidates)
            u.GetComponent<SelectableUnit>()?.SetHighlight(true);
    }

    public bool CanSelect(SlimeUnit u) => IsActive && u && candidates.Contains(u);

    public void Select(SlimeUnit u)
    {
        if (!CanSelect(u)) return;
        onChosen?.Invoke(u);
        End();
    }

    public void End()
    {
        if (!IsActive) return;
        foreach (var u in candidates)
            u?.GetComponent<SelectableUnit>()?.SetHighlight(false);
        candidates.Clear();
        onChosen = null;
        IsActive = false;
    }

    void Update()
    {
        if (IsActive && Input.GetKeyDown(KeyCode.Escape))
            End();
    }
}