using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class OllamaNPCChat : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text responseText;
    public Animator npcAnimator;

    public string model = "llama3.2";
    public string ollamaUrl = "http://localhost:11434/api/chat";

    public void SendMessageToNPC()
    {
        StartCoroutine(CallOllamaChat(inputField.text));
    }

    IEnumerator CallOllamaChat(string userMessage)
    {
        responseText.text = "Sending message to NPC...";

        string prompt =
            "You are Mick-o, a tiny young blue mouse NPC in a fantasy video game. " +
            "You are curious, cheerful, a little clumsy, and easily impressed. " +
            "You live near a magical forest and dream of becoming a brave adventurer, " +
            "even though you are still small and inexperienced. " +
            "You speak in a cute, friendly, simple way. " +
            "Answer briefly, in one or two short sentences. " +
            "Do not mention that you are an AI or a language model. " +
            "Stay in character.";

        string jsonBody =
            "{ \"model\": \"" + model + "\", " +
            "\"stream\": false, " +
            "\"messages\": [" +
                "{ \"role\": \"system\", \"content\": \"" + EscapeJson(prompt) + "\" }," +
                "{ \"role\": \"user\", \"content\": \"" + EscapeJson(userMessage) + "\" }" +
            "] }";

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                responseText.text = "Error: " + request.error;
                yield break;
            }

            string rawJson = request.downloadHandler.text;
            string answer = ExtractChatMessage(rawJson);

            responseText.text = answer;

            if (npcAnimator != null)
                StartCoroutine(PlayTalkingAnimation(answer));
        }
    }

    IEnumerator PlayTalkingAnimation(string answer)
    {
        npcAnimator.SetBool("IsTalking", true);

        float duration = Mathf.Clamp(answer.Length * 0.04f, 1.5f, 5f);
        yield return new WaitForSeconds(duration);

        npcAnimator.SetBool("IsTalking", false);
    }

    string ExtractChatMessage(string json)
    {
        string key = "\"content\":\"";
        int start = json.IndexOf(key);

        if (start == -1)
            return "";

        start += key.Length;
        int end = json.IndexOf("\"", start);

        if (end == -1)
            return "";

        return json.Substring(start, end - start)
            .Replace("\\n", "\n")
            .Replace("\\\"", "\"");
    }

    string EscapeJson(string text)
    {
        return text.Replace("\\", "\\\\")
                   .Replace("\"", "\\\"")
                   .Replace("\n", "\\n");
    }
}