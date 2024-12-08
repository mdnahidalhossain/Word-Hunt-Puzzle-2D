using System.Collections;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class ScriptManager : MonoBehaviour
{
    public PuzzleGameManager gameManager;
    //public GameObject wave1GameObject;
    //public GameObject wave2GameObject;
    //public GameObject wave3GameObject;

    public GameObject[] wavesGameObject;

    private int currentWave = 0;
    private bool isTransitioning = false;

    //private bool isWave1Active = true;

    // Start is called before the first frame update
    void Start()
    {
        //wave1GameObject.SetActive(true);
        //wave2GameObject.SetActive(false);
        //wave3GameObject.SetActive(false);

        for (int i = 0; i < wavesGameObject.Length; i++)
        {
            wavesGameObject[i].SetActive(i == currentWave);
        }
    }

    // Tracks the current wave

    void Update()
    {
        if (gameManager.isPuzzleComplete && !isTransitioning)
        {
            //if (isWave1Active)
            //{
            //    //gameManager.isPuzzleComplete = false;
            //    StartCoroutine(SwitchtoNextPuzzle());

            //};

            StartCoroutine(SwitchtoNextPuzzle());

        }

    }

    private IEnumerator SwitchtoNextPuzzle()
    {

        isTransitioning = true; // Prevent concurrent transitions
        yield return new WaitForSeconds(1.5f); // Delay before switching

        // Deactivate the current wave
        if (currentWave < wavesGameObject.Length)
        {
            Debug.Log($"Deactivating Wave {currentWave + 1}...");
            wavesGameObject[currentWave].SetActive(false);
            currentWave++;
        }

        Debug.Log($"Activating Wave {currentWave}... {wavesGameObject.Length}");

        // Activate the next wave
        //currentWave++; // Increment the current wave
        if (currentWave <= wavesGameObject.Length)
        {
            //Debug.Log($"Activating Wave {currentWave}...");
            Debug.Log($"Activating Wave {currentWave + 1}...");
            wavesGameObject[currentWave].SetActive(true);
            //currentWave++;
        }
        else
        {
            Debug.Log("All waves completed!");
        }

        currentWave++;

        isTransitioning = false;
        gameManager.isPuzzleComplete = false;

    }


}




//// Deactivate the current puzzle and move to the next
//switch (currentWave)
//{
//    case 1: // Wave 1 to Wave 2
//        wave1.SetActive(false);
//        wave2.SetActive(true);
//        //currentWave++;
//        break;

//    case 2: // Wave 2 to Wave 3
//        wave2.SetActive(true);
//        wave3.SetActive(true);
//        //currentWave++;
//        break;

//    case 3: // Wave 3 Completion
//        wave3.SetActive(false);
//        Debug.Log("All puzzles completed!");
//        break;

//    default:
//        Debug.LogError("Invalid wave number!");
//        break;
//}