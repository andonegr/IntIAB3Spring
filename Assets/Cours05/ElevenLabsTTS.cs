using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ElevenLabsTTS : MonoBehaviour
{
    [Header("ElevenLabs")]
    public string apiKey = "PASTE_YOUR_API_KEY";
    public string voiceId = "PASTE_VOICE_ID";
    public string modelId = "eleven_multilingual_v2";

    [Header("Audio")]
    public AudioSource audioSource;

    public void Speak(string text)
    {
        StartCoroutine(GenerateAndPlay(text));
    }

    IEnumerator GenerateAndPlay(string text)
    {
        string url = "https://api.elevenlabs.io/v1/text-to-speech/"
                     + voiceId
                     + "?output_format=mp3_44100_128";

        string json =
            "{ \"text\": \"" + EscapeJson(text) + "\", " +
            "\"model_id\": \"" + modelId + "\", " +
            "\"voice_settings\": { " +
                "\"stability\": 0.45, " +
                "\"similarity_boost\": 0.75 " +
            "} }";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);

            request.SetRequestHeader("xi-api-key", apiKey);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("ElevenLabs error: " + request.error);
                Debug.LogError(request.downloadHandler.text);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    string EscapeJson(string text)
    {
        return text.Replace("\\", "\\\\")
                   .Replace("\"", "\\\"")
                   .Replace("\n", "\\n");
    }
}