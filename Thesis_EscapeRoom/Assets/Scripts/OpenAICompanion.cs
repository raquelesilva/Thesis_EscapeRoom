using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class OpenAICompanion : MonoBehaviour
{
    private string apiKey = "sk - proj - _XxAWUWUTd11gpd4mUdfg0iEf_OQotdFQXWAuHIjBeigJqxiqnulJwA6rdsxcANv7E5ZPU6ucdT3BlbkFJqnS - GkFq5o96H7i8cr4cb0Y2zNf0iTuP7T4NNWph66wL - Yzen1ot9K18WFtRh6z0sM - OGiK2EA\r\n";
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    public void AskOpenAI(string playerMessage)
    {
        StartCoroutine(SendMessageToOpenAI(playerMessage));
    }

    private IEnumerator SendMessageToOpenAI(string message)
    {
        // Criação do payload
        var payload = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = "És um tutor num jogo. Dá dicas em vez de respostas." },
                new { role = "user", content = message }
            }
        };

        string jsonPayload = JsonConvert.SerializeObject(payload);

        // Configuração da requisição HTTP
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Enviar e aguardar resposta
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Resposta da IA: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Erro: " + request.error);
        }
    }
}