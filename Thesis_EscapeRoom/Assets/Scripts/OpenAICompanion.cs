using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Unity.FantasyKingdom;
using Newtonsoft.Json.Linq;

public class OpenAICompanion : MonoBehaviour
{
    private string apiKey = "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\r\n".Trim();
    private string OLDapiKey = "sk - proj - _XxAWUWUTd11gpd4mUdfg0iEf_OQotdFQXWAuHIjBeigJqxiqnulJwA6rdsxcANv7E5ZPU6ucdT3BlbkFJqnS - GkFq5o96H7i8cr4cb0Y2zNf0iTuP7T4NNWph66wL - Yzen1ot9K18WFtRh6z0sM - OGiK2EA\r\n";
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    public static OpenAICompanion instance;

    private void Awake()
    {
        instance = this;
    }

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
                new { role = "system", content = "És um tutor num jogo da descoberta do caminho para a Índia. Dá dicas curtas somente sobre este tema em vez de respostas." },
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
            // Parse the response and extract the AI's answer
            string jsonResponse = request.downloadHandler.text;
            JObject parsedResponse = JObject.Parse(jsonResponse);
            string aiResponse = parsedResponse["choices"]?[0]?["message"]?["content"]?.ToString();

            AdamastorManager.instance.RecieveMessage(aiResponse);
        }
        else
        {
            Debug.LogError("Erro: " + request.error);
        }
    }
}