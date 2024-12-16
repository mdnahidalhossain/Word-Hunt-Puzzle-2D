using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGameManager : MonoBehaviour
{
    [SerializeField] private Text[] charTextField;
    //[SerializeField] public Text uiMessage;
    [SerializeField] public GameObject uiMessage;
    [SerializeField] public GameObject removeButton;
    [SerializeField] private Button[] buttons;
    [SerializeField] protected Color normalColor; // Default button color
    [SerializeField] protected Color pressedColor;
    [SerializeField] private string targetWord; // The correct word to guess
    [SerializeField] private Text descriptionText;
    [SerializeField] private string playerGuess = ""; // Stores the player's current guess
    [SerializeField] private bool isPuzzleComplete = false;
    [SerializeField] private TransitionManager waveTransitionManager;
    [SerializeField] private int currentWaveIndex = 0;

    public AudioClip errorAudioClip;
    public AudioSource audioSource;

    protected bool[] isButtonPressed;

    protected void Start()
    {
        isButtonPressed = new bool[buttons.Length];

        // Make sure to fetch the target word for the first wave.
        UpdateTargetWord();
        UpdateWavePuzzleHint(); 

        if (buttons != null && buttons.Length > 0)
        {
            foreach (Button button in buttons)
            {
                button.GetComponent<Image>().color = normalColor;
            }
        }

        InitializePuzzle();

    }

    private void UpdateTargetWord()
    {
        // Ensure that JsonManager instance is available
        if (JsonManager.instance == null)
        {
            Debug.LogError("JsonManager instance is not assigned!");
            return;
        }

        // Fetch the target word from JsonManager based on the current wave index
        targetWord = JsonManager.instance.GetTargetWordForWave(currentWaveIndex);

        if (string.IsNullOrEmpty(targetWord))
        {
            Debug.LogError("Target word not found for wave index: " + currentWaveIndex);
        }
        else
        {
            Debug.Log("Target word for wave " + currentWaveIndex + ": " + targetWord);
        }
    }

    private void UpdateWavePuzzleHint()
    {
        string description = JsonManager.instance.GetPuzzleHintForWave(currentWaveIndex);
        if (descriptionText != null && !string.IsNullOrEmpty(description))
        {
            descriptionText.text = description; // Display the description
        }
    }

    private void InitializePuzzle()
    {
        if (targetWord.Length > buttons.Length)
        {
            Debug.LogError("Target word length exceeds the number of available buttons!");
            return;
        }

        // Generate shuffled letters
        string shuffledLetters = GenerateShuffledLetters(targetWord, buttons.Length);

        // Assign letters to buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            Text buttonText = buttons[i].GetComponentInChildren<Text>();
            if (i < shuffledLetters.Length)
            {
                buttonText.text = shuffledLetters[i].ToString();
            }
            else
            {
                buttonText.text = ""; // Clear any extra buttons
            }
        }
    }

    private string GenerateShuffledLetters(string word, int buttonCount)
    {
        List<char> letters = new List<char>(word);

        // Add random letters to fill extra buttons
        int extraLettersNeeded = buttonCount - word.Length;
        for (int i = 0; i < extraLettersNeeded; i++)
        {
            char randomLetter = (char)UnityEngine.Random.Range('A', 'Z' + 1);
            letters.Add(randomLetter);
        }

        // Shuffle the letters
        for (int i = letters.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            char temp = letters[i];
            letters[i] = letters[randomIndex];
            letters[randomIndex] = temp;
        }

        return new string(letters.ToArray());
    }

    public void OnButtonClicked(int index)
    {

        if (isButtonPressed[index])
        {
            // Deselect the letter
            //UnselectLetter(index);
            
        }
        else
        {
            // Select the letter
            SelectLetter(index);
            removeButton.SetActive(true);
        }

    }

    private void SelectLetter(int index)
    {
        if (playerGuess.Length < targetWord.Length)
        {
            // Mark the button as pressed
            isButtonPressed[index] = true;
            //removeButton.SetActive(true);

            

            // Update the display field with the letter
            for (int i = 0; i < charTextField.Length; i++)
            {
                if (charTextField[i].text == "")
                {
                    charTextField[i].text = buttons[index].GetComponentInChildren<Text>().text;
                    break;
                }
            }

            // Add the letter to the player's guess
            playerGuess += buttons[index].GetComponentInChildren<Text>().text;
            buttons[index].GetComponent<Image>().color = pressedColor;

            if (playerGuess.Length == targetWord.Length)
            {
                //CheckGuess();
                StartCoroutine(CheckGuessWithDelay());

                if (playerGuess == targetWord)
                {
                    isPuzzleComplete = true;
                    Debug.Log("Puzzle Completed: " + isPuzzleComplete);

                }
                else
                {

                    isPuzzleComplete = false;
                    Debug.Log("Puzzle Completed: " + isPuzzleComplete);

                }

            }
        }
    }

    private IEnumerator CheckGuessWithDelay()
    {
        // Wait for few second before showing the result
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < charTextField.Length; i++)
        {
            if (playerGuess == targetWord)
            {
                //uiMessage.text = "Correct!";
                uiMessage.SetActive(true);
                isPuzzleComplete = true;
                waveTransitionManager.CompleteCurrentWave();

            }
            else
            {
                //uiMessage.text = "Incorrect!";
                audioSource.clip = errorAudioClip;
                audioSource.Play();
                isPuzzleComplete = false;

                //ResetGameOnCLick();
                //UnselectLetter();
            }
        }

    }


    public void ResetGameOnCLick()
    {
        // Clear display fields and reset variables
        foreach (Text field in charTextField)
        {
            field.text = "";
        }
        playerGuess = "";

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<Image>().color = normalColor;
            isButtonPressed[i] = false;
        }

        removeButton.SetActive(false);

        //if (removeButton != null)
        //{
        //    removeButton.SetActive(false); // Deactivate remove button when resetting
        //}

    }
}
