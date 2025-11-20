using UnityEngine;

[CreateAssetMenu(menuName="SlimeGame/Status/Burn")]
public class Status_Burn : StatusSO { private void OnEnable(){ statusName="Brûlure"; tags=StatusTag.Burn; dotPercentPVMax=0.05f; } }

[CreateAssetMenu(menuName="SlimeGame/Status/Bleed")]
public class Status_Bleed : StatusSO { private void OnEnable(){ statusName="Hémorragie"; tags=StatusTag.Bleed; dotPercentPVMax=0.04f; } }

[CreateAssetMenu(menuName="SlimeGame/Status/Stun")]
public class Status_Stun : StatusSO { private void OnEnable(){ statusName="Stun"; tags=StatusTag.Stun; } }

[CreateAssetMenu(menuName="SlimeGame/Status/Root")]
public class Status_Root : StatusSO { private void OnEnable(){ statusName="Racines"; tags=StatusTag.Root; } }

[CreateAssetMenu(menuName="SlimeGame/Status/Taunt")]
public class Status_Taunt : StatusSO { private void OnEnable(){ statusName="Provocation"; tags=StatusTag.Taunt; } }

[CreateAssetMenu(menuName="SlimeGame/Status/Untargetable")]
public class Status_Untargetable : StatusSO { private void OnEnable(){ statusName="Intouchable"; tags=StatusTag.Untargetable; } }

[CreateAssetMenu(menuName="SlimeGame/Status/Confuse")]
public class Status_Confuse : StatusSO { private void OnEnable(){ statusName="Confusion"; tags=StatusTag.Confused; } }

[CreateAssetMenu(menuName="SlimeGame/Status/DmgShieldMode")]
public class Status_Turtle : StatusSO
{
    private void OnEnable(){ statusName="Formation tortue"; tags=StatusTag.DmgInDown|StatusTag.DmgOutDown; }
}

[CreateAssetMenu(menuName="SlimeGame/Status/Immunity")]
public class Status_Immunity : StatusSO
{
    private void OnEnable(){ statusName="Bouclier absolu"; tags=StatusTag.Immunity; }
}