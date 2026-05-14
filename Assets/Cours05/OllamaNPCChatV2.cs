using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class OllamaNPCChatV2 : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;
    public TMP_Text responseText;

    [Header("NPC")]
    public Animator npcAnimator;

    [Header("Ollama")]
    public string model = "llama3.2";
    public string ollamaUrl = "http://localhost:11434/api/chat";

    [Header("Message History")]
    private List<string> conversationHistory = new List<string>();
    public int maxHistoryMessages = 10;

    public void SendMessageToNPC()
    {
        StartCoroutine(CallOllamaChat(inputField.text));
    }

    public PiperTTS piperTTS;


    IEnumerator CallOllamaChat(string userMessage)
    {
        responseText.text = "Mick-o is thinking...";

        string prompt =
            "You are Mick-o, a tiny young blue mouse NPC in a fantasy video game. " +
            "You are curious, cheerful, a little clumsy, and easily impressed. " +
            "You live near a magical forest and dream of becoming a brave adventurer, " +
            "even though you are still small and inexperienced. " +
            "You speak in a cute, friendly, simple way. " +
            "Answer briefly, in one or two short sentences. " +
            "Speak only as dialogue. " +
            "Do not describe actions, emotions, gestures, facial expressions, or stage directions. " +
            "Do not use parentheses or asterisks. " +
            "Do not mention that you are an AI or a language model. " +
            "Stay in character.";

        conversationHistory.Add("User: " + userMessage);

        if (conversationHistory.Count > maxHistoryMessages)
        {
            conversationHistory.RemoveAt(0);
        }

        string history = "";

        foreach (string message in conversationHistory)
        {
            history += message + "\n";
        }

        prompt = prompt + "\n\nConversation so far:\n" + history + "\nNPC:";

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

            conversationHistory.Add("NPC: " + answer);

            if (conversationHistory.Count > maxHistoryMessages)
            {
                conversationHistory.RemoveAt(0);
            }

            responseText.text = answer;

            if (npcAnimator != null)
                StartCoroutine(PlayTalkingAnimation(answer));

            if (piperTTS != null)
            {
                piperTTS.Speak(answer);
            }

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