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

    public GameObject progressBarFill; // Reference to the progress bar fill object
    public GameObject[] whiteCheckPoints;
    public GameObject[] greyCheckPoints;
    public GameObject[] greenCheckPoints;
    public float progressBarFillTime = 2f;

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

        // Activate the next wave
        currentWaveIndex++;
        if (currentWaveIndex < waves.Length)
        {
            
            //waves[currentWaveIndex].SetActive(true);
            ActivateWave(currentWaveIndex);
            greenCheckPoints[currentWaveIndex].SetActive(true);


        }

        isTransitioning = false;
        gameManager.uiMessage.gameObject.SetActive(false);
        //greenCheckPoints[currentWaveIndex].SetActive(true);


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
            whiteCheckPoints[index].SetActive(true);
            greyCheckPoints[index-1].SetActive(false);
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
            progressBarFill.transform.localScale = new Vector3(scale, 1f, 1f);
        }
    }
}

