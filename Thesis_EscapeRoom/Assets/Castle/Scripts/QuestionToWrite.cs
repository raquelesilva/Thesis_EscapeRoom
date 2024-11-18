using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionToWrite : MonoBehaviour
{
    //Numero de letras por segundo
    [SerializeField] int LPS = 100;
    //Quais as respostas para escrever
    [SerializeField] string AnswersToWrite;
    //O sitio para escrever a mensagem se for especial
    [SerializeField] TextMeshProUGUI PlaceToWrite;

    public static QuestionToWrite instance;

    private void Start()
    {
        instance = this;
    }

    //Função para animar a escrita de perguntas
    public void WriteQuestions(string inputText)
    {
        StopAllCoroutines();
        AnswersToWrite =  inputText;
        PlaceToWrite.text = "";
        StartCoroutine(TypeText(PlaceToWrite, AnswersToWrite, LPS));
    }

    //função que escreve texto letra a letra
    IEnumerator TypeText(TextMeshProUGUI area, string text, int lettersPerSec)
    {
        foreach (var letter in text.ToCharArray())
        {
            string result = area.text;
            area.text = result + letter;
            yield return new WaitForSeconds(2f / lettersPerSec);
            StringBuilder sb = new(area.text);
            area.text = sb.ToString();
        }

        string result1 = area.text;
        area.text = result1;
    }
}