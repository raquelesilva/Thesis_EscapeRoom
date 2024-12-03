using CoreSystems.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Unity.FantasyKingdom
{
    public class InputGame : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] GameObject gameUI;
        [SerializeField] List<Answers> allAnswers = new();

        public void StartGame()
        {
            gameUI.SetActive(true);
            FirstPersonController.instance.SetPause(true);
        }

        public void SetCurrentAnswer(TMP_InputField currentInput)
        {
            Debug.Log("JBREHGLIJB RJGEJÇBFNJL");

            foreach (var answer in allAnswers)
            {
                if (currentInput == answer.inputField)
                {
                    answer.currentAnswer = currentInput.text;
                }
            }
        }

        public void CheckAnswer()
        {
            int correct = 0;

            foreach (var answer in allAnswers)
            {
                if (answer.currentAnswer.ToLower().Trim() == answer.correctAnswer.ToLower().Trim())
                {
                    answer.inputField.image.color = Color.green;
                    correct++;
                }
                else
                {
                    answer.inputField.image.color = Color.red;
                }
            }

            if (correct == allAnswers.Count)
            {
                NotificationManager.instance.SetMessage("Boa conseguiste!", Color.green);
                CloseGame();
            }
            else if (correct == 0)
            {
                NotificationManager.instance.SetMessage("Revê todas as tuas respostas!", Color.red);
            }
            else
            {
                NotificationManager.instance.SetMessage("Revê algumas das tuas respostas!", Color.yellow);
            }
        }

        public void CloseGame()
        {
            gameUI.SetActive(false);
            FirstPersonController.instance.SetPause(false);
        }
    }
}

[Serializable]
public class Answers
{
    public TMP_InputField inputField;
    public string correctAnswer;
    public string currentAnswer;

    public Answers(TMP_InputField inputField, string correctAnswer, string currentAnswer)
    {
        this.inputField = inputField;
        this.correctAnswer = correctAnswer;
        this.currentAnswer = currentAnswer;
    }
}