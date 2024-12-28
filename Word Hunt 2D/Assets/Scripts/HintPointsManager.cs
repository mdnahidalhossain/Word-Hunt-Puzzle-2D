using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class HintPointsData
{
    public int totalHintPoints = 10;
}

public class HintPointsManager : MonoBehaviour
{
    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "hintPoint.json");
    public static HintPointsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveHintPoints(int hintPoints)
    {
        try
        {
            HintPointsData data = new HintPointsData { totalHintPoints = hintPoints };
            string json = JsonUtility.ToJson(data);

            // Ensure directory exists (for some platforms)
            Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath));

            // Write data to the file
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"Hint points saved successfully: {json}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving hint points: {ex.Message}");
        }
    }

    public int LoadHintPoints()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            HintPointsData data = JsonUtility.FromJson<HintPointsData>(json);
            Debug.Log($"Hint points loaded: {data.totalHintPoints}");
            return data.totalHintPoints;
        }
        Debug.LogWarning("Save file not found. Initializing hint points to default value (5).");
        return 5; // Default hint points if no save file is found
    }

    public void ResetHintPoints(int defaultHintPoints = 10)
    {
        SaveHintPoints(defaultHintPoints);
        Debug.Log($"Hint points reset to default: {defaultHintPoints}");
    }
}
