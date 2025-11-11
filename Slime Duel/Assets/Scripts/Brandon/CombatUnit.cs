public enum UnitRole {
    Player,
    Boss,
    Monster
}

public class CombatUnit {
    public SlimeUnit unit;
    public UnitRole role;

    public CombatUnit(SlimeUnit unit, UnitRole role) {
        this.unit = unit;
        this.role = role;
    }

    public bool IsAlive {
        get {
            return unit != null && unit.IsAlive;
        }
    }

    public int Agi {
        get {
            return unit != null ? unit.Agi : 0;
        }
    }
}