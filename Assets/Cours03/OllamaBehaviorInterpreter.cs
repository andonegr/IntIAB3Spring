using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class OllamaBehaviorInterpreter : MonoBehaviour
{
    public TMP_InputField promptInput;
    public TMP_Text responseText;
    public GameBehaviorController behaviorController;

    public string model = "llama3.2";
    public string ollamaUrl = "http://localhost:11434/api/generate";

    public void SendPrompt()
    {
        StartCoroutine(CallOllama(promptInput.text));
    }

    IEnumerator CallOllama(string userDescription)
    {
        string prompt =
            "You control a Unity game object.\n" +
            "Choose exactly one behavior mode from this list:\n" +
            "calm, fast, aggressive, chase, flee, orbit.\n\n" +
            "User description:\n" +
            "\"" + userDescription + "\"\n\n" +
            "Answer with only one word.";

        string jsonBody =
            "{ \"model\": \"" + model + "\", " +
            "\"prompt\": \"" + EscapeJson(prompt) + "\", " +
            "\"stream\": false }";

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            responseText.text = "Sending prompt to Ollama...";
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                responseText.text = "Error: " + request.error;
                yield break;
            }
            responseText.text = "Prompt sent successfully.";

            string rawJson = request.downloadHandler.text;
            string mode = ExtractResponse(rawJson).ToLower().Trim();
            responseText.text += "\nAI selected mode: " + mode;

            behaviorController.ApplyMode(mode);
            responseText.text += "\nApplying behavior in Unity...";

        }
    }

    string ExtractResponse(string json)
    {
        string marker = "\"response\":\"";
        int start = json.IndexOf(marker);

        if (start == -1)
            return "";

        start += marker.Length;
        int end = json.IndexOf("\"", start);

        if (end == -1)
            return "";

        return json.Substring(start, end - start)
                   .Replace("\\n", "")
                   .Replace("\\\"", "\"");
    }

    string EscapeJson(string text)
    {
        return text.Replace("\\", "\\\\")
                   .Replace("\"", "\\\"")
                   .Replace("\n", "\\n");
    }
}