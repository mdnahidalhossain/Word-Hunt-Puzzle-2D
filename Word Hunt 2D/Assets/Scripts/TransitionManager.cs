using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private PuzzleGameManager gameManager;
    [SerializeField] private GameObject[] waves;
    [SerializeField] private GameObject progressBarFill; // Reference to the progress bar fill object
    [SerializeField] private GameObject levelCompletionMessage;
    [SerializeField] private GameObject[] whiteCheckPoints;
    [SerializeField] private GameObject[] greyCheckPoints;
    [SerializeField] private GameObject[] greenCheckPoints; 
    [SerializeField] private float progressBarFillTime = 0.5f;
    [SerializeField] private bool isRandomWaveOrder = false; // Enable random wave transitions for the second scene

    private GameObject[] waveOrder;
    private List<int> waveIndices = new List<int>();
    private int currentWaveIndex = 0;
    private bool isTransitioning = false;


    // Start is called before the first frame update
    void Start()
    {
        waveOrder = isRandomWaveOrder ? ShuffleArray(waves) : waves;

        ActivateWave(0);
        ActivateCheckpoint(0, "white");

    }


    public void CompleteCurrentWave()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("Transition already in progress!");

            
            return;
        }

        if (currentWaveIndex < waveOrder.Length)
        {
            StartCoroutine(TransitionToNextWave());

            //HintPointsManager.instance.SaveHintPoints(gameManager.totalHintPoints);
        }
        else
        {
            Debug.Log("All waves completed!");
            gameManager.CompleteLevel();
        }
    }

    private IEnumerator TransitionToNextWave()
    {
        
        isTransitioning = true;
        HintPointsManager.instance.SaveHintPoints(gameManager.totalHintPoints);

        // Deactivate the current wave
        DeactivateWave(currentWaveIndex);
        DeactivateCheckpoint(currentWaveIndex, "white");
        ActivateCheckpoint(currentWaveIndex, "green");

        

        yield return new WaitForSeconds(1.0f);


        if (progressBarFill != null)
        {
            if (currentWaveIndex == 0)
            {
                // Fill halfway after Wave 1
                yield return StartCoroutine(AnimateProgressBar(0.5f));
            }
            else if (currentWaveIndex == 1)
            {
                // Fill fully after Wave 2
                yield return StartCoroutine(AnimateProgressBar(1f));
            }
        }

        //// Activate the next wave
        Debug.Log($"Current Wave Index (before increment): {currentWaveIndex}");
        currentWaveIndex++;
        Debug.Log($"Current Wave Index (after increment): {currentWaveIndex}");

        if (currentWaveIndex < waveOrder.Length)
        {
            ActivateWave(currentWaveIndex);
            ActivateCheckpoint(currentWaveIndex, "white");
            
        }

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("All waves completed! Displaying UI message.");
            if (gameManager.uiMessage != null)
            {
                levelCompletionMessage.SetActive(true); // Activate the UI message
            }
            isTransitioning = false;
            yield break; // Exit the coroutine, as there are no more waves to transition to
        }

        //ActivateWave(currentWaveIndex);
        isTransitioning = false;
        gameManager.uiMessage.SetActive(false);



        if (currentWaveIndex >= waves.Length)
        {
            isTransitioning = false;
            Debug.Log("No transition in progress");
        }

    }

    private void ActivateWave(int index)
    {
        if (index < waveOrder.Length)
        {
            waveOrder[index].SetActive(true);

            // Update grey checkpoint visibility
            if (index > 0)
            {
                DeactivateCheckpoint(index - 1, "grey");
            }
        }
    }

    private void DeactivateWave(int index)
    {
        if (index < waveOrder.Length)
        {
            waveOrder[index].SetActive(false);
        }
    }

    private void ActivateCheckpoint(int index, string type)
    {
        if (type == "white" && index < whiteCheckPoints.Length)
            whiteCheckPoints[index].SetActive(true);
        else if (type == "green" && index < greenCheckPoints.Length)
            greenCheckPoints[index].SetActive(true);
        else if (type == "grey" && index < greyCheckPoints.Length)
            greyCheckPoints[index].SetActive(true);
    }

    private void DeactivateCheckpoint(int index, string type)
    {
        if (type == "white" && index < whiteCheckPoints.Length)
            whiteCheckPoints[index].SetActive(false);
        else if (type == "grey" && index < greyCheckPoints.Length)
            greyCheckPoints[index].SetActive(false);
    }


    private IEnumerator AnimateProgressBar(float targetScaleX)
    {
        bool animationComplete = false;

        // Animate the progress bar fill to the target scale using LeanTween
        LeanTween.scaleX(progressBarFill, targetScaleX, progressBarFillTime).setOnComplete(() =>
        {
            animationComplete = true;
        });

        // Wait until the animation is complete
        while (!animationComplete)
        {
            yield return null;
        }
    }

    private void SetProgressBarScale(float scale)
    {
        if (progressBarFill != null)
        {
            progressBarFill.transform.localScale = new Vector3(scale, 1f, 5f);
        }
    }

    private GameObject[] ShuffleArray(GameObject[] array)
    {
        GameObject[] shuffledArray = (GameObject[])array.Clone();
        for (int i = 0; i < shuffledArray.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, shuffledArray.Length);
            GameObject temp = shuffledArray[i];
            shuffledArray[i] = shuffledArray[randomIndex];
            shuffledArray[randomIndex] = temp;
        }
        //Debug.Log("Shuffled Waves Order: " + string.Join(", ", shuffledArray));
        return shuffledArray;
    }

}

