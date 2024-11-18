using IGameMech.InputFields;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordScreen : MonoBehaviour
{
    public ComputerManager ComputerManager;
    public TMP_InputField PasswordInputField;
    public Button SubmitButton;
    public TextMeshProUGUI MessageText;
    public string CorrectPassword = "1234";

    private void Start()
    {
        SubmitButton.onClick.AddListener(OnSubmitButtonClicked);
        MessageText.text = "Enter Password: ";

    }

    public void OnSubmitButtonClicked()
    {
        if(PasswordInputField.text == CorrectPassword)
        {
            MessageText.text = "Access Granted! ";
            ComputerManager.StartEngine();

        }
        else
        {
            MessageText.text = "Incorrect Password. Try Again.";
            PasswordInputField.text = "";
        }
    }
}
