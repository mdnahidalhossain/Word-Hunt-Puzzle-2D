using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private PuzzleGameManager gameManager;
    [SerializeField] private GameObject[] waves;

    private int currentWaveIndex = 0;
    private bool isTransitioning = false;

    [SerializeField] private GameObject progressBarFill; // Reference to the progress bar fill object
    [SerializeField] private GameObject levelCompletionMessage;
    [SerializeField] private GameObject[] whiteCheckPoints;
    [SerializeField] private GameObject[] greyCheckPoints;
    [SerializeField] private GameObject[] greenCheckPoints; 
    [SerializeField] private float progressBarFillTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

        if (waves.Length > 0)
        {
            waves[currentWaveIndex].SetActive(true);
            whiteCheckPoints[currentWaveIndex].SetActive(true);
        }


    }


    public void CompleteCurrentWave()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("Transition already in progress!");

            
            return;
        }


        if (currentWaveIndex < waves.Length)
        {
            // Increment the wave index to fetch the correct target word for the next wave
            
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
        whiteCheckPoints[currentWaveIndex].SetActive(false);



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

        if (currentWaveIndex < waves.Length)
        {

            ActivateWave(currentWaveIndex);
            greenCheckPoints[currentWaveIndex].SetActive(true);

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
        if (index < waves.Length)
        {
            Debug.Log($"Activating wave: {index}");

            waves[index].SetActive(true);
            
            whiteCheckPoints[index].SetActive(true);
            //greyCheckPoints[index - 1].SetActive(false);

            if (index > 0)
            {
                // Deactivate the current greyCheckPoint and optionally the previous one
                greyCheckPoints[index - 1].SetActive(false);
            }

            if (index > 1)
            {
                // Deactivate an additional greyCheckPoint if applicable
                greyCheckPoints[index - 2].SetActive(false);
            }

        }

        else
        {
            Debug.LogWarning("Wave index is out of range.");
        }
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
}

