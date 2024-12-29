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
    //[SerializeField] public GameObject removeButton;
    [SerializeField] private Button[] buttons;
    [SerializeField] protected Color normalColor; // Default button color
    [SerializeField] protected Color pressedColor;
    [SerializeField] protected Color hintColor;
    [SerializeField] private string targetWord; // The correct word to guess
    [SerializeField] private Text descriptionText;
    [SerializeField] private string playerGuess = ""; // Stores the player's current guess
    [SerializeField] private bool isPuzzleComplete = false;
    [SerializeField] private TransitionManager waveTransitionManager;
    [SerializeField] private int currentWaveIndex = 0;

    public AudioClip errorAudioClip;
    //public AudioClip ButtonSoundClip;
    public AudioSource audioSource;

    protected bool[] isButtonPressed;

    public int hintCount = 0;
    public int totalHintPoints = 0; // Track total hint points
    [SerializeField] private Text hintPointsText;

    private void UpdateHintPointsText()
    {
        if (hintPointsText != null)
        {
            hintPointsText.text = "Hint: " + totalHintPoints.ToString();
        }
    }

    protected void Start()
    {
        isButtonPressed = new bool[buttons.Length];

        totalHintPoints = HintPointsManager.instance.LoadHintPoints();
        // Make sure to fetch the target word for the first wave.
        UpdateTargetWord();
        UpdateWavePuzzleHint();

        UpdateHintPointsText();


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
            UnselectLetter(index);
            
        }
        else
        {
            // Select the letter
            SelectLetter(index);
            //removeButton.SetActive(true);
        }

    }


    private void SelectLetter(int index)
    {
        if (playerGuess.Length < targetWord.Length)
        {
            // Mark the button as pressed
            isButtonPressed[index] = true;
            //audioSource.clip = ButtonSoundClip;
            //audioSource.Play();

            // Get the letter to select
            string letterToSelect = buttons[index].GetComponentInChildren<Text>().text;

            // Add the letter to the first available slot in playerGuess
            char[] guessArray = playerGuess.PadRight(targetWord.Length, ' ').ToCharArray();
            for (int i = 0; i < guessArray.Length; i++)
            {
                if (guessArray[i] == ' ') // Find the first empty slot
                {
                    guessArray[i] = letterToSelect[0];
                    charTextField[i].text = letterToSelect; // Update the display field
                    break;
                }
            }
            playerGuess = new string(guessArray).TrimEnd();

            // Update the button's appearance
            buttons[index].GetComponent<Image>().color = pressedColor;

            Debug.Log($"Selected letter: {letterToSelect}, updated playerGuess: {playerGuess}");

            // Check if the guess is complete
            if (playerGuess.Replace(" ", "").Length == targetWord.Length)
            {
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


    private void UnselectLetter(int index)
    {
        // Mark the button as unpressed
        isButtonPressed[index] = false;

        // Get the letter to unselect
        string letterToUnselect = buttons[index].GetComponentInChildren<Text>().text;

        // Replace the letter with a space in playerGuess to maintain position
        char[] guessArray = playerGuess.ToCharArray();
        for (int i = 0; i < guessArray.Length; i++)
        {
            if (guessArray[i].ToString() == letterToUnselect)
            {
                guessArray[i] = ' '; // Leave a blank space
                break;
            }
        }
        playerGuess = new string(guessArray);

        // Clear the corresponding text field
        for (int i = 0; i < charTextField.Length; i++)
        {
            if (charTextField[i].text == letterToUnselect)
            {
                charTextField[i].text = ""; // Clear the display
                break;
            }
        }

        // Reset the button's appearance
        buttons[index].GetComponent<Image>().color = normalColor;

        Debug.Log($"Unselected letter: {letterToUnselect}, updated playerGuess: {playerGuess}");
    }


    private IEnumerator CheckGuessWithDelay()
    {
        // Wait for few second before showing the result
        yield return new WaitForSeconds(0.3f);

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
                VibratePhone();
                isPuzzleComplete = false;

                ResetGameOnCLick();
                //UnselectLetter();
            }
        }

    }

    // Award hint points after completing a level
    public void CompleteLevel()
    {
        totalHintPoints += 5; // Award 5 hint points per level
        Debug.Log("Level completed. Total hint points: " + totalHintPoints);
        UpdateHintPointsText();
    }


    public void OnHintButtonClicked()
    {
        if (totalHintPoints > 0) // Ensure there are hint points available
        {
            if (hintCount < targetWord.Length)
            {
                char nextCharToReveal = targetWord[hintCount];

                // Skip over already guessed or pressed characters
                while (hintCount < targetWord.Length && playerGuess.Contains(nextCharToReveal.ToString()))
                {
                    hintCount++;
                    if (hintCount < targetWord.Length)
                    {
                        nextCharToReveal = targetWord[hintCount];
                    }
                }

                // If hintCount exceeds the length of the target word, stop providing hints
                if (hintCount >= targetWord.Length)
                {
                    Debug.Log("All hints used or all characters already guessed!");
                    return;
                }

                // Highlight the correct button that matches the next character
                for (int i = 0; i < buttons.Length; i++)
                {
                    Text buttonText = buttons[i].GetComponentInChildren<Text>();

                    if (buttonText.text == nextCharToReveal.ToString() && !isButtonPressed[i])
                    {
                        buttons[i].GetComponent<Image>().color = hintColor;
                        Debug.Log($"Hint: Press button with '{nextCharToReveal}'");

                        hintCount++;
                        totalHintPoints--; // Decrease hint points when a hint is used
                        Debug.Log("Remaining hint points: " + totalHintPoints);

                        // Update the hint points UI text after using a hint
                        UpdateHintPointsText();

                        return;
                    }
                }

                // Save updated hint points
                //HintPointsManager.instance.SaveHintPoints(totalHintPoints);
            }
            else
            {
                Debug.Log("All hints used!");
            }
            HintPointsManager.instance.SaveHintPoints(totalHintPoints);

        }
        else
        {
            Debug.Log("No hint points available!");
        }
    }



    void VibratePhone()
    {
        // Check if the device supports vibration (for mobile platforms)
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Handheld.Vibrate();
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

        //removeButton.SetActive(false);

        //if (removeButton != null)
        //{
        //    removeButton.SetActive(false); // Deactivate remove button when resetting
        //}

        hintCount = 0;

    }
}
