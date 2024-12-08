using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGame : MonoBehaviour
{
    public Button[] characterButtons; // Buttons for random characters
    public Text[] letterBoxes; // Empty boxes where characters will go
    public Text uiMessage; // UI message when the word is correct
    private string correctWord = "WORD"; // Example word
    private string currentWord = ""; // Word that the player is forming

    void Start()
    {
        GenerateShuffledCharacters();
        uiMessage.gameObject.SetActive(false); // Hide the UI message initially
    }

    void GenerateShuffledCharacters()
    {
        // Ensure that there are enough buttons to display all characters of the word
        if (characterButtons.Length != correctWord.Length)
        {
            Debug.LogError("The number of buttons does not match the number of characters in the word.");
            return;
        }

        // Convert the correct word to a char array and shuffle it
        char[] shuffledLetters = correctWord.ToCharArray();
        ShuffleArray(shuffledLetters);

        // Assign shuffled letters to the buttons
        for (int i = 0; i < characterButtons.Length; i++)
        {
            characterButtons[i].GetComponentInChildren<Text>().text = shuffledLetters[i].ToString();
            // Add listener to button clicks
            int index = i; // Local copy of the index to avoid closure issues
            characterButtons[i].onClick.AddListener(() => OnCharacterClicked(shuffledLetters[index].ToString()));
        }
    }

    void ShuffleArray(char[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            char temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    void OnCharacterClicked(string character)
    {
        Debug.Log("Clicked Character: " + character);

        // Find the first empty letter box to place the character
        for (int i = 0; i < letterBoxes.Length; i++)
        {
            if (letterBoxes[i].text == "")
            {
                letterBoxes[i].text = character;

                Debug.Log("Placing character: " + character + " in box " + i);

                currentWord += character;
                i++;
                break;
            }
        }

        CheckWord();
    }


    void CheckWord()
    {
        // Check if the current word is correct
        if (currentWord.Length == correctWord.Length)
        {
            if (currentWord == correctWord)
            {
                uiMessage.text = "Correct!";
                uiMessage.gameObject.SetActive(true);
            }
            else
            {
                uiMessage.text = "Incorrect! Try again.";
                uiMessage.gameObject.SetActive(true);
            }
        }
    }
}


//using System;
//using UnityEngine;
//using UnityEngine.UI;

//public class PuzzleGame : MonoBehaviour
//{
//    public InputField[] letterBoxes; // Array of InputFields for the letters
//    public Button[] characterButtons; // Buttons for the shuffled characters
//    private int currentInputIndex = 0; // Tracks the next available InputField
//    private string currentWord = ""; // Stores the word being formed

//    // Called when a button is clicked
//    public void OnCharacterClicked(string character)
//    {
//        Debug.Log("Clicked Character: " + character);

//        // Check if we still have an empty InputField to fill
//        if (currentInputIndex < letterBoxes.Length)
//        {
//            // Set the text of the current InputField
//            letterBoxes[currentInputIndex].text = character;

//            // Append the character to the current word
//            currentWord += character;

//            // Move to the next InputField
//            currentInputIndex++;
//        }
//        else
//        {
//            Debug.Log("All input fields are filled!");
//        }

//        // Check if the word is correct
//        CheckWord();
//    }

//    // Check if the word is correct
//    void CheckWord()
//    {
//        string correctWord = "WORD"; // Replace with your desired word
//        if (currentWord.Equals(correctWord))
//        {
//            Debug.Log("Correct word!");
//            // Display a success message in the UI
//            // Example: Show a panel or text indicating success
//        }
//    }

//    // Set up the buttons with shuffled characters
//    void Start()
//    {
//        string correctWord = "WORD"; // Example correct word
//        string[] shuffledCharacters = ShuffleString(correctWord);

//        for (int i = 0; i < characterButtons.Length; i++)
//        {
//            int buttonIndex = i; // Local variable for closure
//            characterButtons[i].GetComponentInChildren<Text>().text = shuffledCharacters[i]; // Set button text

//            // Add click event to button
//            characterButtons[i].onClick.AddListener(() => OnCharacterClicked(shuffledCharacters[buttonIndex]));
//        }
//    }

//    // Shuffle characters in a string and return as an array
//    string[] ShuffleString(string word)
//    {
//        char[] characters = word.ToCharArray();

//        for (int i = 0; i < characters.Length; i++)
//        {
//            char temp = characters[i];
//            int randomIndex = UnityEngine.Random.Range(i, characters.Length);
//            characters[i] = characters[randomIndex];
//            characters[randomIndex] = temp;
//        }

//        return Array.ConvertAll(characters, c => c.ToString()); // Convert to string array for buttons
//    }
//}
