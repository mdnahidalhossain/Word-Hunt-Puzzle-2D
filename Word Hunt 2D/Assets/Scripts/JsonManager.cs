using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.Networking;

[System.Serializable]
public class WaveData
{
    public int waveIndex;
    public string targetWord;
    public string puzzleHint;
}

[System.Serializable]
public class WaveDataList
{
    public List<WaveData> waves;
}

public class JsonManager : MonoBehaviour
{
    public static JsonManager instance;

    public WaveDataList waveDataList;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadWaveData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadWaveData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "targetWords.json");

        // Check platform-specific loading
        if (filePath.Contains("://") || filePath.Contains(":///")) // For Android and WebGL
        {
            StartCoroutine(LoadJsonFromWeb(filePath));
        }
        else if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            waveDataList = JsonUtility.FromJson<WaveDataList>(json);
        }
        else
        {
            Debug.LogError("Wave data file not found.");
        }
    }

    private IEnumerator LoadJsonFromWeb(string path)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                waveDataList = JsonUtility.FromJson<WaveDataList>(json);
                Debug.Log("Wave data loaded successfully from StreamingAssets.");
            }
            else
            {
                Debug.LogError("Failed to load wave data: " + request.error);
            }
        }
    }

    public string GetTargetWordForWave(int waveIndex)
    {
        if (waveDataList == null)
        {
            Debug.LogError("Wave data is not loaded.");
            return null;
        }

        foreach (var wave in waveDataList.waves)
        {
            if (wave.waveIndex == waveIndex)
            {
                return wave.targetWord;
            }
        }
        return null;
    }

    // New method to get the description for a wave
    public string GetPuzzleHintForWave(int waveIndex)
    {
        if (waveDataList == null)
        {
            Debug.LogError("Wave data is not loaded.");
            return null;
        }

        foreach (var wave in waveDataList.waves)
        {
            if (wave.waveIndex == waveIndex)
            {
                return wave.puzzleHint;
            }
        }
        return null;
    }
}

