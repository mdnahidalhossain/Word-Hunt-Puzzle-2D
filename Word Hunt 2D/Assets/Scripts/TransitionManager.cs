using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86;

public class TransitionManager : MonoBehaviour
{
    public PuzzleGameManager gameManager;
    public GameObject[] waves;
    private int currentWaveIndex = 0;
    private bool isTransitioning = false;


    // Start is called before the first frame update
    void Start()
    {

        if (waves.Length > 0)
        {
            waves[currentWaveIndex].SetActive(true);
        }


    }


    public void CompleteCurrentWave()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("Transition already in progress!");

            
            return;
        }

        if (currentWaveIndex < waves.Length - 1)
        {
            StartCoroutine(TransitionToNextWave());
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    private IEnumerator TransitionToNextWave()
    {
        
        isTransitioning = true;

        // Deactivate the current wave

        yield return new WaitForSeconds(1.5f);

        waves[currentWaveIndex].SetActive(false);// Optional delay for smooth transitions

        // Activate the next wave
        currentWaveIndex++;
        if (currentWaveIndex < waves.Length)
        {
            
            //waves[currentWaveIndex].SetActive(true);
            ActivateWave(currentWaveIndex);
            
        }

        isTransitioning = false;
        gameManager.uiMessage.gameObject.SetActive(false);

        if (currentWaveIndex >= waves.Length)
        {
            isTransitioning = false;
        }

    }

    private void ActivateWave(int index)
    {
        if (index < waves.Length)
        {
            Debug.Log($"Activating wave: {index}");
            waves[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning("Wave index is out of range.");
        }
    }
}

