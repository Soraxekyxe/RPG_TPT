using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveAllData(Gacha gacha, Inventaire inventaire)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/GameData.slime";

        try
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                SavedData data = new SavedData(gacha, inventaire);
                formatter.Serialize(stream, data);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors de la sauvegarde des données : " + e.Message);
        }
    }

    public static SavedData LoadData()
    {
        string path = Application.persistentDataPath + "/GameData.slime";
        if (!File.Exists(path))
        {
            Debug.LogWarning("404: fichier de sauvegarde introuvable à " + path);
            return null;
        }

        try
        {
            // Si le fichier est vide, on renvoie null
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Length == 0)
            {
                Debug.LogWarning("Fichier de sauvegarde vide, aucune donnée à charger.");
                return null;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                SavedData data = formatter.Deserialize(stream) as SavedData;
                return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Impossible de charger la sauvegarde (fichier corrompu ou vide) : " + e.Message);
            return null;
        }
    }
    
    public static void DeleteSavedData()
    {
        string path = Application.persistentDataPath + "/GameData.slime";

        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
                Debug.Log("Sauvegarde supprimée avec succès !");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Impossible de supprimer la sauvegarde : " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde à supprimer à " + path);
        }
    }

}