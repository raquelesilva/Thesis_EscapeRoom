using CoreSystems.Extensions.Attributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Keypad_Input : MonoBehaviour
{
    [Header("Logic")]
    [SerializeField] string correctAnswer;

    [Header("References")]
    [SerializeField] List<MultipurposeScript> keys;
    [SerializeField] TextMeshPro keyPadInput;

    [Header("Events")]
    [SerializeField] UnityEvent winEvents;
    [SerializeField] UnityEvent wrongEvents;

    [Header("Sound")]
    [SerializeField] AudioSource keypadAudioSource;
    [SerializeField] AudioClip clickPadSound;
    [SerializeField] AudioClip winPadSound;
    [SerializeField] AudioClip losePadSound;

    [Header("Focus")]
    [SerializeField] bool canFocus;
    [SerializeField, Disable("canFocus")] Interactable focusInteractable;
    [SerializeField, Disable("canFocus")] Transform focusPoint;
    [SerializeField, Disable("canFocus")] UnityEvent onDefocus;

    [Header("Debug")]
    [SerializeField, Disable] bool completedKeyPad;
    [SerializeField, Disable] bool isFocused;


    private string inputText;
    private int insertedDigits = 0;
    private FirstPersonController player;
    private BoxCollider interactableBox;

    private void Start()
    {
        ErasePassword();

        player = FirstPersonController.instance;
        interactableBox = GetComponent<BoxCollider>();

        if (canFocus)
        {
            focusInteractable.SetInteractable(canFocus);
            interactableBox.enabled = canFocus;
        }

        if (canFocus)
        {
            focusInteractable.interaction.AddListener(() =>
            {
                interactableBox.enabled = false;
                player.AnimateCamera(focusPoint);
                player.SetPlayerState((int)PlayerStates.focused);
                SetIsFocus(true);
            });

            onDefocus.AddListener(() =>
            {
                interactableBox.enabled = true;
                player.GoBackCamera();
                player.SetPlayerStateDelayed((int)PlayerStates.playing);
                SetIsFocus(false);
            });
        }
    }

    private void Update()
    {
        if (canFocus)
        {
            if (isFocused && Input.GetKeyDown(KeyCode.Escape))
            {
                isFocused = false;
                onDefocus?.Invoke();
            }
        }
    }

    public void SetIsFocus(bool decision)
    {
        isFocused = decision;
    }

    public void InsertNumbers(string number)
    {
        if (insertedDigits < correctAnswer.Length)
        {
            inputText += number;
            insertedDigits++;
            keyPadInput.text = inputText;

            if (clickPadSound != null)
            {
                keypadAudioSource.clip = clickPadSound;
                keypadAudioSource.Play();
            }
        }
    }

    public void CheckPassword()
    {
        if (completedKeyPad) return;

        KeyPadBlock(true);

        if (inputText == correctAnswer)
        {

            completedKeyPad = true;
            winEvents?.Invoke();

            StartCoroutine(FlashColor(Color.green, false, true));
            if (winPadSound != null)
            {
                keypadAudioSource.clip = winPadSound;
                keypadAudioSource.Play();
            }

            
        }
        else
        {
            wrongEvents?.Invoke();
            StartCoroutine(FlashColor(Color.red, true, false));

            if (losePadSound != null)
            {
                keypadAudioSource.clip = losePadSound;
                keypadAudioSource.Play();
            }
        }
    }

    IEnumerator FlashColor(Color wantedColor, bool erase, bool keypadState)
    {
        Color originalColor = keyPadInput.color;

        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(.5f);
            keyPadInput.color = wantedColor;

            yield return new WaitForSeconds(.5f);
            keyPadInput.color = originalColor;

        }

        if (erase)
        {
            ErasePassword();
        }

        if(keypadState == true)
        {
            if (canFocus)
            {
                onDefocus?.Invoke();
                interactableBox.enabled = false;
            }
        }

        KeyPadBlock(keypadState);

    }

    public void KeyPadBlock(bool decision)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            keys[i].SetInteractable(!decision);
        }
    }

    public void ErasePassword()
    {
        inputText = "";
        keyPadInput.text = "";
        insertedDigits = 0;
        for (int i = 0; i < correctAnswer.Length; i++)
        {
            keyPadInput.text += i == correctAnswer.Length ? "_" : "_ ";
        }
    }


    public void PlayClickPadSound()
    {
        keypadAudioSource.clip = clickPadSound;
        keypadAudioSource.Play();
    }
    public void PlayWinPadSound()
    {
        keypadAudioSource.clip = winPadSound;
        keypadAudioSource.Play();
    }
    public void PlayLosePadSound()
    {
        keypadAudioSource.clip = losePadSound;
        keypadAudioSource.Play();
    }


}