using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class OllamaTest : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text outputText;

    string url = "http://localhost:11434/api/generate";

    public void SendPrompt()
    {
        StartCoroutine(CallOllama(inputField.text));
    }

    IEnumerator CallOllama(string userInput)
    {
        string prompt = "Answer with one word: calm or aggressive.\nUser: " + userInput;

        string json = "{ \"model\": \"llama3.2\", " +
                      "\"prompt\": \"" + EscapeJson(prompt) + "\", " +
                      "\"stream\": false }";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                outputText.text = "Error: " + request.error;
                yield break;
            }

            string response = ExtractResponse(request.downloadHandler.text);
            outputText.text = "AI: " + response;
        }
    }

    string ExtractResponse(string json)
    {
        string key = "\"response\":\"";
        int start = json.IndexOf(key);

        if (start == -1) return "";

        start += key.Length;
        int end = json.IndexOf("\"", start);

        if (end == -1) return "";

        return json.Substring(start, end - start).Replace("\\n", "");
    }

    string EscapeJson(string text)
    {
        return text.Replace("\\", "\\\\")
                   .Replace("\"", "\\\"")
                   .Replace("\n", "\\n");
    }
}