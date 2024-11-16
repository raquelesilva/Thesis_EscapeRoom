using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;

    [SerializeField] List<Messages> messages = new();

    [SerializeField] QuestionToWrite questionToWrite;

    // ### A duração total da animação quando este abre/fecha
    [SerializeField] float duration = 1f;

    // ### Tipo de função que terá um efeito especifico na animação de Easing
    [SerializeField] EaseFunctions.Ease easeFunction;

    // ### Referência do objeto da janela
    [SerializeField] Transform window = null;

    // ### Referência do objeto que terá mensagem
    [SerializeField] TextMeshProUGUI message = null;

    [SerializeField] Color32 colorRight = Color.white;
    [SerializeField] Color32 colorWrong = Color.white;


    private void Awake()
    {
        instance = this;
    }

    public void GetMessage(int i)
    {
        SetMessage(messages[i].message, messages[i].messageColor);
    }


    /// <summary>
    /// Define a mensagem, a cor da mensagem e abre a janela da notificação
    /// </summary>
    public void SetMessage(string text, Color32 messageColor, float duration = 1f)
    {
        window.gameObject.SetActive(false);

        this.duration = duration;
        StopAllCoroutines();
        window.gameObject.SetActive(true);
        questionToWrite.WriteQuestions(text);
        //window.localScale = Vector3.zero;

        //message.color = messageColor;
        var img = window.gameObject.GetComponent<Image>();
        img.color = messageColor;

        StartCoroutine(nameof(StartMessage));
    }

    /// <summary>
    /// Retorna a duração definida
    /// </summary>
    public float GetDuration()
    {
        return duration;
    }

    public Color32 GetColorRight()
    {
        return colorRight;
    }

    public Color32 GetColorWrong()
    {
        return colorWrong;
    }

    /// <summary>
    /// Contém o processo de animação da janela
    /// </summary>
    private IEnumerator StartMessage()
    {
        float currentDuration = 0f;
        float currentPercentage = 0f;

        while (currentPercentage < 2f)
        {
            currentDuration += Time.deltaTime;

            currentPercentage = currentDuration / duration;

            var scaleMode = EaseFunctions.GetEasingFunction(easeFunction);

            float value = scaleMode(0f, 1f, currentPercentage);

            //window.localScale = new Vector3(value, value, value);

            yield return null;
        }

        window.gameObject.SetActive(false);
    }

    public void DisableWindow()
    {
        window.gameObject.SetActive(false);
    }


    [Serializable]
    class Messages
    {
        public string message;
        public Color messageColor;
        public bool rightColor;
        public bool wrongColor;

        public Messages(string message, Color messageColor)
        {
            this.message = message;
            this.messageColor = messageColor;
        }
    }
}