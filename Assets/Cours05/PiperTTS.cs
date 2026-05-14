using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class PiperTTS : MonoBehaviour
{
    [Header("Piper")]
    public string piperPath = "C:/Piper/piper.exe";
    public string modelPath = "C:/Piper/en_US-lessac-medium.onnx";
    public string outputPath = "C:/Piper/output.wav";

    [Header("Audio")]
    public AudioSource audioSource;

    public void Speak(string text)
    {
        StartCoroutine(GenerateSpeech(text));
    }

    IEnumerator GenerateSpeech(string text)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = piperPath,
            Arguments = "--model \"" + modelPath + "\" --output_file \"" + outputPath + "\"",
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new Process();
        process.StartInfo = startInfo;

        process.Start();

        // Send text directly to Piper
        process.StandardInput.Write(text);
        process.StandardInput.Close();

        process.WaitForExit();

        yield return new WaitForSeconds(0.2f);

        if (!File.Exists(outputPath))
        {
            UnityEngine.Debug.LogError("Audio file not found: " + outputPath);
            yield break;
        }

        string audioUrl = "file:///" + outputPath.Replace("\\", "/");

        using (UnityWebRequest request =
               UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.WAV))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                UnityEngine.Debug.LogError(request.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}