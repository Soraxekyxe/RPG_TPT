using System.Collections;                    // <<< important pour les coroutines
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem I;

    void Awake() => I = this;

    [Header("Équipe A (Joueur)")]
    public SlimeUnit[] teamA = new SlimeUnit[3];

    [Header("Équipe B (IA)")]
    public SlimeUnit[] teamB = new SlimeUnit[3];

    [Header("Génération équipe joueur")]
    public BaseItemSO[] playerItemPool;   // objets possibles pour les slimes joueur
    public Skin[]       playerSkinPool;   // skins possibles pour les slimes joueur

    [Header("Génération de vagues ennemies")]
    public BaseItemSO[] enemyItemPool;   // objets possibles pour les ennemis
    public Skin[]       enemySkinPool;   // skins possibles pour les ennemis

    [Header("HUD ennemis pour les vagues suivantes")]
    public SlimeHUD enemyHudPrefab;      // prefab de HUD (SlimeHUD)
    public Transform enemyHudParent;     // parent (colonne de droite dans le Canvas)
    private readonly List<SlimeHUD> enemySpawnedHUDs = new();

    private readonly List<SlimeUnit> turnOrder = new();
    private int turnIndex = -1;
    public SlimeUnit Active { get; private set; }

    // Bloque Update pendant le choix du joueur
    private bool waitingForPlayer = false;
    // empêche de lancer plusieurs coroutines IA
    private bool enemyTurnRunning = false;

    // numéro de vague (0 = combat initial, 1 = deuxième vague, etc.)
    private int waveIndex = 0;

    void Start()
    {
        // =====================================================
        //   Génère l’équipe du joueur de façon aléatoire (lvl 1)
        // =====================================================
        InitRandomPlayerTeam();

        // (les ennemis du premier combat restent ceux placés dans la scène)

        BuildTurnOrder();

        // orientation des sprites
        foreach (var s in teamA)
            if (s) s.SetFacingLeft(false);   // équipe joueur → regarde vers la droite

        foreach (var s in teamB)
            if (s) s.SetFacingLeft(true);    // équipe ennemie → regarde vers la gauche

        NextTurnStart();
    }

    // ---------------------------------------------------------
    //  Génère / configure les 3 slimes de l’équipe A
    // ---------------------------------------------------------
    void InitRandomPlayerTeam()
    {
        if (teamA == null) return;

        for (int i = 0; i < teamA.Length; i++)
        {
            var s = teamA[i];
            if (!s) continue;

            // Classe aléatoire
            var cls = GetRandomClass();

            // Objet aléatoire dans le pool joueur
            BaseItemSO item = null;
            if (playerItemPool != null && playerItemPool.Length > 0)
                item = playerItemPool[Random.Range(0, playerItemPool.Length)];

            // Skin aléatoire dans le pool joueur
            Skin skin = null;
            if (playerSkinPool != null && playerSkinPool.Length > 0)
                skin = playerSkinPool[Random.Range(0, playerSkinPool.Length)];

            // Configure le slime pour ce combat – niveau 1, côté joueur (isEnemy = false)
            s.ConfigureForBattle(cls, 1, item, skin, false);
        }
    }

    void Update()
    {
        if (Active == null) return;

        // Tour joueur : on ne fait rien ici, c'est l'UI qui gère
        if (teamA.Contains(Active)) return;

        // Si un tour IA est déjà en cours → on ne relance pas
        if (enemyTurnRunning) return;

        // Lance la coroutine du tour IA
        StartCoroutine(EnemyTurnRoutine());
    }

    // === Tour de l'IA avec délai ===
    IEnumerator EnemyTurnRoutine()
    {
        enemyTurnRunning = true;

        // Si stun/root : il ne joue pas
        if (Active.HasTag(StatusTag.Stun) || Active.HasTag(StatusTag.Root))
        {
            CombatLogUI.I?.Log(Active, $"{Active.slimeName} ne peut pas jouer !");
            yield return new WaitForSeconds(0.8f);
            EndTurn();
            enemyTurnRunning = false;
            yield break;
        }

        var usable = Active.actions.Where(a => a != null && a.CanPay(Active)).ToList();
        if (usable.Count == 0)
        {
            CombatLogUI.I?.Log(Active, $"{Active.slimeName} ne peut rien faire.");
            yield return new WaitForSeconds(0.8f);
            EndTurn();
            enemyTurnRunning = false;
            yield break;
        }

        // Petit délai AVANT l'action (pour le suspense)
        yield return new WaitForSeconds(1.0f);

        var act = usable[Random.Range(0, usable.Count)];

        var allies  = teamB.Contains(Active) ? teamB.ToList() : teamA.ToList();
        var enemies = teamB.Contains(Active) ? teamA.ToList() : teamB.ToList();

        CombatLogUI.I?.Log(Active, $"{Active.slimeName} utilise {act.actionName}");
        act.Execute(Active, allies, enemies);

        // Petit délai APRÈS l'action (optionnel)
        yield return new WaitForSeconds(0.2f);

        EndTurn();
        enemyTurnRunning = false;
    }

    // ======== Tour suivant ========
    void NextTurnStart()
    {
        if (IsBattleOver())
        {
            bool aAlive = TeamAlive(teamA);
            bool bAlive = TeamAlive(teamB);

            string res;
            if (aAlive && !bAlive)      res = "ÉQUIPE A GAGNE";
            else if (!aAlive && bAlive) res = "ÉQUIPE B GAGNE";
            else                        res = "Match nul";

            Debug.Log($"=== FIN DU COMBAT — {res} ===");
            CombatLogUI.I?.Log(null, res); // neutre

            // Affiche l’écran de résultat
            if (CombatResultUI.I != null)
            {
                if (aAlive && !bAlive)
                    CombatResultUI.I.ShowWin();   // joueur gagne
                else if (!aAlive && bAlive)
                    CombatResultUI.I.ShowLose();  // joueur perd
                else
                    CombatResultUI.I.ShowDraw();  // égalité
            }

            enabled = false;              // stoppe le BattleSystem
            CombatUI.I?.HideAll();        // masque les boutons
            return;
        }

        do
        {
            turnIndex = (turnIndex + 1) % turnOrder.Count;
            Active = turnOrder[turnIndex];
        }
        while (Active == null || !Active.IsAlive);

        Active.TickStartOfTurn();
        Debug.Log($"--- Tour de {Active.slimeName} ({Active.classe}) ---");
        CombatLogUI.I?.Log(Active, $"Tour de {Active.slimeName}");

        enemyTurnRunning = false;   // reset au cas où

        // Tour joueur → afficher commandes
        if (teamA.Contains(Active))
        {
            waitingForPlayer = true;
            CombatUI.I.ShowCommands(Active);
        }
    }

    public void EndTurn()
    {
        Active.TickEndOfTurn();
        waitingForPlayer = false;
        NextTurnStart();
    }

    // ======== Setup de l'ordre ========
    void BuildTurnOrder()
    {
        turnOrder.Clear();
        var all = teamA.Concat(teamB).Where(u => u != null).ToList();
        turnOrder.AddRange(all.OrderByDescending(u => u.Agi));
        turnIndex = -1;
    }

    bool TeamAlive(SlimeUnit[] t) => t.Any(u => u && u.IsAlive);
    bool IsBattleOver() => !TeamAlive(teamA) || !TeamAlive(teamB);

    // ======== SÉLECTION DE CIBLES POUR UI (ancienne API) ========
    public List<SlimeUnit> GetValidTargetsFor(ActionSO act, SlimeUnit user)
    {
        var allies  = teamA.Contains(user) ? teamA.ToList() : teamB.ToList();
        var enemies = teamA.Contains(user) ? teamB.ToList() : teamA.ToList();

        switch (act.targetMode)
        {
            case TargetMode.Self:        return new List<SlimeUnit> { user };
            case TargetMode.AllySingle:  return allies.Where(a => a && a.IsAlive).ToList();
            case TargetMode.AllyAll:     return allies.Where(a => a && a.IsAlive).ToList();
            case TargetMode.EnemySingle: return enemies.Where(e => e && e.IsAlive).ToList();
            case TargetMode.EnemyAll:    return enemies.Where(e => e && e.IsAlive).ToList();
        }

        return new List<SlimeUnit> { user };
    }

    // ======== Appelé par l'UI (multi-cible / self) ========
    public void PlayerCastsSkill(SlimeUnit user, ActionSO act, SlimeUnit target)
    {
        var allies  = teamA.Contains(user) ? teamA.ToList() : teamB.ToList();
        var enemies = teamA.Contains(user) ? teamB.ToList() : teamA.ToList();

        CombatLogUI.I?.Log(user, $"{user.slimeName} utilise {act.actionName}");
        act.Execute(user, allies, enemies);
        EndTurn();
    }

    // ======== Mort d'une unité ========
    public void OnUnitDied(SlimeUnit u)
    {
        if (Active == u)
        {
            CombatUI.I?.HideAll();
            EndTurn();
        }
    }

    // ======== Helpers équipes ========
    public List<SlimeUnit> GetAlliesOf(SlimeUnit u)
        => (teamA.Contains(u) ? teamA : teamB).Where(x => x != null).ToList();

    public List<SlimeUnit> GetEnemiesOf(SlimeUnit u)
        => (teamA.Contains(u) ? teamB : teamA).Where(x => x != null).ToList();

    public bool IsAlly(SlimeUnit u)
        => teamA != null && System.Array.IndexOf(teamA, u) >= 0;

    // ======== Attaque de base joueur ========
    public void PlayerBasicAttack(SlimeUnit user, SlimeUnit target)
    {
        int raw = Mathf.Max(1, user.For);
        int dealt = target.TakeDamage(raw, DamageKind.Physical);

        CombatLogUI.I?.Log(user, $"{user.slimeName} attaque {target.slimeName} ({dealt} dégâts)");

        EndTurn();
    }

    // ======== Compétence mono-cible avec cible choisie ========
    public void PlayerCastsSkillOnTarget(SlimeUnit user, ActionSO act, SlimeUnit target)
    {
        var allies  = GetAlliesOf(user);
        var enemies = GetEnemiesOf(user);

        CombatLogUI.I?.Log(user, $"{user.slimeName} utilise {act.actionName} sur {target.slimeName}");

        act.ExecuteOnTarget(user, target, allies, enemies);
        EndTurn();
    }

    // =====================================================================
    //              GESTION DES VAGUES  (bouton "Suivant")
    // =====================================================================

    public void StartNextWave()
    {
        // si l'équipe du joueur est morte, on ne relance pas
        if (!TeamAlive(teamA))
        {
            Debug.Log("Impossible de lancer une nouvelle vague : l'équipe A est vaincue.");
            return;
        }

        waveIndex++;

        // niveau de départ = plus haut niveau des ennemis actuels (ou 1)
        int previousEnemyLevel = 1;
        foreach (var e in teamB)
            if (e != null)
                previousEnemyLevel = Mathf.Max(previousEnemyLevel, e.Lvl);

        int newLevel = previousEnemyLevel + 1;

        // reconfigure TOUTE l'équipe ennemie
        foreach (var e in teamB)
        {
            if (e == null) continue;

            var cls  = GetRandomClass();
            var item = (enemyItemPool != null && enemyItemPool.Length > 0)
                ? enemyItemPool[Random.Range(0, enemyItemPool.Length)]
                : null;
            var skin = (enemySkinPool != null && enemySkinPool.Length > 0)
                ? enemySkinPool[Random.Range(0, enemySkinPool.Length)]
                : null;

            e.ConfigureForBattle(cls, newLevel, item, skin, true); // méthode dans SlimeUnit
        }

        // ré-oriente les sprites
        foreach (var s in teamA)
            if (s) s.SetFacingLeft(false);
        foreach (var s in teamB)
            if (s) s.SetFacingLeft(true);

        // HUD ENNEMIS : on recrée des HUD pour la nouvelle vague
        if (enemyHudPrefab && enemyHudParent)
        {
            foreach (var hud in enemySpawnedHUDs)
                if (hud) Destroy(hud.gameObject);
            enemySpawnedHUDs.Clear();

            foreach (var e in teamB)
            {
                if (!e) continue;
                var hud = Instantiate(enemyHudPrefab, enemyHudParent);
                hud.Bind(e);
                enemySpawnedHUDs.Add(hud);
            }
        }

        waitingForPlayer = false;
        enemyTurnRunning = false;
        Active = null;
        enabled = true;

        BuildTurnOrder();
        CombatResultUI.I?.HideAll();
        NextTurnStart();
    }

    SlimeClass GetRandomClass()
    {
        var values = System.Enum.GetValues(typeof(SlimeClass));
        return (SlimeClass)values.GetValue(Random.Range(0, values.Length));
    }
}








