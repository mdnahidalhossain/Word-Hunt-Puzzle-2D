using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGameManager : MonoBehaviour
{
    public Text[] charTextField;
    public Text uiMessage;
    public Button[] buttons;
    [SerializeField] protected Color normalColor; // Default button color
    [SerializeField] protected Color pressedColor;
    [SerializeField] public string targetWord; // The correct word to guess
    [SerializeField] public string playerGuess = ""; // Stores the player's current guess

    protected bool[] isButtonPressed;
    public bool isPuzzleComplete = false;

    protected void Start()
    {
        isButtonPressed = new bool[buttons.Length];

        foreach (Button button in buttons)
        {
            button.GetComponent<Image>().color = normalColor;
        }

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
        }

    }

    private void SelectLetter(int index)
    {
        if (playerGuess.Length < targetWord.Length)
        {
            // Mark the button as pressed
            isButtonPressed[index] = true;

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

                //if (playerGuess == targetWord)
                //{
                //    isPuzzleComplete = true;
                //    Debug.Log("Puzzle Completed: " + isPuzzleComplete);

                //}
                //else
                //{
                //    isPuzzleComplete = false;
                //    Debug.Log("Puzzle Completed: " + isPuzzleComplete);
                //}

            }
        }
    }

    private void UnselectLetter(int index)
    {
        // Mark the button as unpressed
        isButtonPressed[index] = false;

        // Remove the letter from the player's guess
        playerGuess = playerGuess.Replace(buttons[index].GetComponentInChildren<Text>().text, "");

        // Clear the corresponding display field
        for (int i = 0; i < charTextField.Length; i++)
        {
            if (charTextField[i].text == buttons[index].GetComponentInChildren<Text>().text)
            {
                charTextField[i].text = "";
                break;
            }
        }
        buttons[index].GetComponent<Image>().color = normalColor;

    }

    private IEnumerator CheckGuessWithDelay()
    {
        // Wait for few second before showing the result
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < charTextField.Length; i++)
        {
            if (playerGuess == targetWord)
            {
                uiMessage.text = "Correct!";
                uiMessage.gameObject.SetActive(true);
                isPuzzleComplete = true;
                Debug.Log("Puzzle Completed: " + isPuzzleComplete);

            }
            else
            {
                uiMessage.text = "Incorrect!";
                uiMessage.gameObject.SetActive(true);
                isPuzzleComplete = false;
                Debug.Log("Puzzle Completed: " + isPuzzleComplete);
                //Debug.Log(isPuzzleComplete);

                ResetGame();
                UnselectLetter(i);
            }
        }

    }

    private void ResetGame()
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

    }
}
