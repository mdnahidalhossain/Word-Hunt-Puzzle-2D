using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

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

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            waveDataList = JsonUtility.FromJson<WaveDataList>(json);
        }
        else
        {
            Debug.LogError("Wave data file not found.");
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

