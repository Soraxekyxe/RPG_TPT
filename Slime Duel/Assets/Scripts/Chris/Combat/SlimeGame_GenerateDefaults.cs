using System.IO;
using UnityEditor;
using UnityEngine;

public class SlimeGame_GenerateDefaults : Editor
{
    const string Root = "Assets/SlimeGame";
    const string StatFolder = Root + "/Statuses";
    const string SkillFolder = Root + "/Skills";

    [MenuItem("Tools/SlimeGame/Generate Default Statuses & Skills")]
    public static void GenerateAll()
    {
        EnsureFolder(Root);
        EnsureFolder(StatFolder);
        EnsureFolder(SkillFolder);

        // ---------- STATUSES ----------
        // Nécessite les classes Status_* (voir mes messages précédents)
        var burn  = GetOrCreate<Status_Burn>         (StatFolder + "/Burn.asset",   s => {});
        var bleed = GetOrCreate<Status_Bleed>        (StatFolder + "/Bleed.asset",  s => {});
        var stun  = GetOrCreate<Status_Stun>         (StatFolder + "/Stun.asset",   s => {});
        var root  = GetOrCreate<Status_Root>         (StatFolder + "/Root.asset",   s => {});
        var taunt = GetOrCreate<Status_Taunt>        (StatFolder + "/Taunt.asset",  s => {});
        var untgt = GetOrCreate<Status_Untargetable> (StatFolder + "/Untargetable.asset", s => {});
        var turtle= GetOrCreate<Status_Turtle>       (StatFolder + "/Turtle.asset", s => {});
        var confuse=GetOrCreate<Status_Confuse>      (StatFolder + "/Confuse.asset", s => {});
        var immune=GetOrCreate<Status_Immunity>      (StatFolder + "/Immunity.asset", s => {});
        // Optionnel : CleanseOnce (si tu as ajouté Status_CleanseOnce.cs)
        var cleanse = AssetDatabase.LoadAssetAtPath<Status_CleanseOnce>(StatFolder + "/CleanseOnce.asset");
        if (cleanse == null)
            cleanse = CreateAsset<Status_CleanseOnce>(StatFolder + "/CleanseOnce.asset", s => { });

        // ---------- SKILLS ----------
        // Helper local
        ActionSO S(string path, System.Action<ActionSO> edit) =>
            GetOrCreate<ActionSO>(SkillFolder + "/" + path + ".asset", a => { a.actionName = path; edit(a); });

        // ===== GUERRIER =====
        S("Guerrier - Coup de graisse", a => {
            a.manaCost = 2; a.targetMode = TargetMode.EnemySingle;
            a.doesDamage = true; a.damageKind = DamageKind.Physical;
            a.useDef = true; a.percentDef = 0.33f;
            a.applyStatus = stun; a.applyStatusTurns = 1;
            a.description = "Dégâts basés sur 1/3 DEF, stun 1 tour.";
        });

        S("Guerrier - Quoicoubeh", a => {
            a.manaCost = 5; a.targetMode = TargetMode.Self;
            a.applyStatus = taunt; a.applyStatusTurns = 3;
            a.description = "Se provoque : les ennemis le focus 3 tours.";
        });

        S("Guerrier - Formation tortue", a => {
            a.manaCost = 10; a.targetMode = TargetMode.Self;
            a.applyStatus = turtle; a.applyStatusTurns = 3;
            a.description = "Reçoit -10% dmg / Inflige -20% dmg pendant 3 tours.";
        });

        S("Guerrier - Gonflette", a => {
            a.manaCost = 15; a.targetMode = TargetMode.Self;
            // Buff DEF +60% pendant 3 tours (non-cumulatif à gérer par logique si besoin)
            a.addPercentDef = 0.60f; a.applyStatusTurns = 3;
            a.description = "Augmente DEF de 60% (éviter le cumul plusieurs fois).";
        });

        S("Guerrier - ZAC E", a => {
            a.manaCost = 20; a.targetMode = TargetMode.EnemyAll;
            a.doesDamage = true; a.damageKind = DamageKind.True;
            a.usePVMax = true; a.percentPVMax = 0.15f;
            a.description = "AOE 15% des PV max (dégâts vrais).";
        });

        S("Guerrier - Signature - Projet à rendre dans 17 minutes", a => {
            a.isSignature = true; a.targetMode = TargetMode.EnemyAll;
            a.doesDamage = true; a.damageKind = DamageKind.True;
            a.useMissingPV = true; a.percentMissingPV = 1.0f; // 100% PV manquants
            a.description = "AOE basée sur les PV manquants du lanceur (100%).";
        });

        // ===== MAGE =====
        S("Mage - Boule de feu", a => {
            a.manaCost = 10; a.targetMode = TargetMode.EnemyAll;
            a.doesDamage = true; a.damageKind = DamageKind.Magical;
            a.useInt = true; a.percentInt = 0.5f;
            a.applyStatus = burn; a.applyStatusTurns = 2;
            a.description = "AOE 50% Int, applique Brûlure (2t).";
        });

        S("Mage - Lancer de caillou", a => {
            a.manaCost = 13; a.targetMode = TargetMode.EnemySingle;
            a.doesDamage = true; a.damageKind = DamageKind.Magical;
            a.useInt = true; a.percentInt = 0.5f; // progression approximée
            a.hits = 5; a.randomSplitHits = true;
            a.description = "Lance 2 à 5 cailloux (progressif).";
        });

        S("Mage - Friction", a => {
            a.manaCost = 12; a.targetMode = TargetMode.EnemySingle;
            a.doesDamage = true; a.damageKind = DamageKind.Magical;
            a.useInt = true; a.percentInt = 1.2f;
            a.description = "Gros dégâts monocible : 120% Int.";
        });

        S("Mage - Boule de feu +", a => {
            a.manaCost = 20; a.targetMode = TargetMode.EnemyAll;
            a.doesDamage = true; a.damageKind = DamageKind.Magical;
            a.useInt = true; a.percentInt = 1.0f;
            a.applyStatus = burn; a.applyStatusTurns = 2;
            a.description = "AOE 100% Int, applique Brûlure.";
        });

        S("Mage - Zapper", a => {
            a.manaCost = 24; a.targetMode = TargetMode.EnemySingle;
            a.doesDamage = true; a.damageKind = DamageKind.Magical;
            a.useInt = true; a.percentInt = 0.5f; a.hits = 3;
            a.applyStatus = stun; a.applyStatusTurns = 1;
            a.description = "3 éclairs 50% Int chacun, stun 1 tour.";
        });

        S("Mage - Énorme Boule de feu", a => {
            a.manaCost = 50; a.targetMode = TargetMode.EnemyAll;
            a.doesDamage = true; a.damageKind = DamageKind.Magical;
            a.useInt = true; a.percentInt = 1.5f;
            a.applyStatus = burn; a.applyStatusTurns = 2;
            a.description = "AOE 150% Int, applique Brûlure.";
        });

        S("Mage - Suce", a => {
            a.manaCost = 0; a.targetMode = TargetMode.EnemySingle;
            a.drainMana = true; a.drainManaPercentOfTargetRemaining = 0.12f;
            a.description = "Draine 12% du mana restant de la cible.";
        });

        S("Mage - Surcharge", a => {
            a.manaCost = 0; a.targetMode = TargetMode.Self;
            a.addPercentInt = 0.5f; a.applyStatusTurns = 3;
            a.description = "Augmente la magie (Int) de 50% pendant 3 tours.";
        });

        S("Mage - Signature - Sabotage", a => {
            a.isSignature = true; a.manaCost = 60;
            a.targetMode = TargetMode.EnemyAll;
            a.applyStatus = confuse; a.applyStatusTurns = 1;
            a.description = "À la prochaine action, ennemis confus (ciblent un allié).";
        });

        // ===== ASSASSIN =====
        S("Assassin - Danse étrange", a => {
            a.manaCost = 20; a.targetMode = TargetMode.Self;
            a.applyStatus = untgt; a.applyStatusTurns = 2;
            a.description = "Intouchable pendant 2 tours.";
        });

        S("Assassin - Grosse frappe", a => {
            a.manaCost = 10; a.targetMode = TargetMode.EnemySingle;
            a.doesDamage = true; a.damageKind = DamageKind.Physical;
            a.useFor = true; a.percentFor = 2.0f;
            a.description = "200% For monocible.";
        });

        S("Assassin - Frappe très légère", a => {
            a.manaCost = 13; a.targetMode = TargetMode.EnemySingle;
            a.doesDamage = true; a.damageKind = DamageKind.Physical;
            a.useFor = true; a.percentFor = 0.8f;
            a.applyStatus = bleed; a.applyStatusTurns = 2;
            a.description = "80% For + applique Hémorragie (2t).";
        });

        S("Assassin - Combot", a => {
            a.manaCost = 25; a.targetMode = TargetMode.EnemySingle;
            a.doesDamage = true; a.damageKind = DamageKind.Physical;
            a.useFor = true; a.percentFor = 1.0f;
            a.hits = 6; a.randomSplitHits = true; // 2 à 6 approx
            a.description = "2 à 6 coups (~répartis), 100% For total approx.";
        });

        // ===== CLERC =====
        S("Clerc - Petite gâterie", a => {
            a.manaCost = 10; a.targetMode = TargetMode.AllySingle;
            a.doesHeal = true; a.healPercentInt = 0.6f;
            a.description = "Soigne 60% Int un allié.";
        });

        S("Clerc - Guérison", a => {
            a.manaCost = 12; a.targetMode = TargetMode.AllyAll;
            a.applyStatus = cleanse; a.applyStatusTurns = 1; // retire tous les statuts à l'activation
            a.description = "Retire les statuts de tous les alliés.";
        });

        S("Clerc - Partouze", a => {
            a.manaCost = 16; a.targetMode = TargetMode.AllyAll;
            a.doesHeal = true; a.healPercentInt = 0.6f;
            a.description = "Soigne tout le groupe (60% Int).";
        });

        S("Clerc - Grande gâterie", a => {
            a.manaCost = 18; a.targetMode = TargetMode.AllySingle;
            a.doesHeal = true; a.healPercentInt = 1.2f;
            a.description = "Soigne beaucoup un allié (120% Int).";
        });

        S("Clerc - Orgie", a => {
            a.manaCost = 30; a.targetMode = TargetMode.AllyAll;
            a.doesHeal = true; a.healPercentInt = 1.5f;
            a.description = "Soigne beaucoup tous les alliés (150% Int).";
        });

        S("Clerc - Bouclier", a => {
            a.manaCost = 15; a.targetMode = TargetMode.AllySingle;
            a.addPercentDef = 0.2f; a.applyStatusTurns = 3;
            a.description = "Augmente DEF de 20% (3 tours).";
        });

        S("Clerc - Dopage", a => {
            a.manaCost = 15; a.targetMode = TargetMode.AllySingle;
            a.addPercentFor = 0.2f; a.applyStatusTurns = 3;
            a.description = "Augmente FOR de 20% (3 tours).";
        });

        S("Clerc - Branlette intellectuelle", a => {
            a.manaCost = 15; a.targetMode = TargetMode.AllySingle;
            a.addPercentInt = 0.2f; a.applyStatusTurns = 3;
            a.description = "Augmente INT de 20% (3 tours).";
        });

        S("Clerc - Bouclier absolu", a => {
            a.manaCost = 35; a.targetMode = TargetMode.AllySingle;
            a.applyStatus = immune; a.applyStatusTurns = 2;
            a.description = "Immunité aux statuts pendant 2 tours.";
        });

        // ===== DRUIDE =====
        S("Druide - Racine", a => {
            a.manaCost = 16; a.targetMode = TargetMode.EnemySingle;
            a.applyStatus = root; a.applyStatusTurns = 3;
            a.description = "Empêche d'agir 3 tours.";
        });

        S("Druide - Érosion", a => {
            a.manaCost = 20; a.targetMode = TargetMode.EnemySingle;
            a.addPercentDef = -0.2f; a.applyStatusTurns = 3;
            a.description = "Baisse DEF de 20% (3 tours).";
        });

        S("Druide - Ralentissement", a => {
            a.manaCost = 20; a.targetMode = TargetMode.EnemySingle;
            a.addPercentAgi = -0.2f; a.applyStatusTurns = 3;
            a.description = "Baisse AGI de 20% (3 tours).";
        });

        S("Druide - Culture du vide", a => {
            a.manaCost = 20; a.targetMode = TargetMode.EnemySingle;
            a.addPercentInt = -0.2f; a.applyStatusTurns = 3;
            a.description = "Baisse INT de 20% (3 tours).";
        });

        S("Druide - Impuissance", a => {
            a.manaCost = 20; a.targetMode = TargetMode.EnemySingle;
            a.addPercentFor = -0.2f; a.applyStatusTurns = 3;
            a.description = "Baisse FOR de 20% (3 tours).";
        });

        S("Druide - Site en 3D sur bloc-note", a => {
            a.manaCost = 23; a.targetMode = TargetMode.EnemySingle;
            a.applyStatus = confuse; a.applyStatusTurns = 1;
            a.description = "Rend confus 1 ennemi (1 tour).";
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✔ SlimeGame: Statuts & Compétences générés dans Assets/SlimeGame.");
    }

    // ---------- helpers ----------
    static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            var parent = Path.GetDirectoryName(path).Replace('\\','/');
            var name = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    static T GetOrCreate<T>(string path, System.Action<T> edit) where T : ScriptableObject
    {
        var obj = AssetDatabase.LoadAssetAtPath<T>(path);
        if (obj == null) obj = CreateAsset<T>(path, edit);
        else edit?.Invoke(obj);
        EditorUtility.SetDirty(obj);
        return obj;
    }

    static T CreateAsset<T>(string path, System.Action<T> edit) where T : ScriptableObject
    {
        var obj = ScriptableObject.CreateInstance<T>();
        edit?.Invoke(obj);
        AssetDatabase.CreateAsset(obj, path);
        return obj;
    }
}
