using System.Collections;
using System.Collections.Generic;
using SimpleFileBrowser;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.IO;

public class SamplePlayer : Part
{
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] ParticleSystem playAudioPS;
    
    private string pathToSample = "";
    private AudioClip audioClip;
    private float volume;

    public override void ReceiveTrigger(int? value)
    {
        if (value.HasValue)
        {
            float newVolume = Mathf.Clamp(value.GetValueOrDefault(), 0, 100);
            audioPlayer.volume = newVolume / 100;
            contextMenu.UpdateContextMenu("Volume", newVolume);
        }
        else 
        {
            SetActive();
        }
    }

    public override void UpdateParameters(Dictionary<string, object> parameters)
    {
        audioPlayer.volume = (float)parameters["Volume"] / 100;

        if (parameters["PathToSample"].ToString() != pathToSample)
        {
            pathToSample = parameters["PathToSample"].ToString();
            GetSample(pathToSample);
        }
        else if (parameters["PathToSample"].ToString() == "")
        {
            audioPlayer.clip = null;
        }
    }

    public async void GetSample(string path)
    {
        audioClip = await LoadSample(path);
        audioPlayer.clip = audioClip;
    }

    private async Task<AudioClip> LoadSample(string path)
    {
        AudioClip clip = null;

        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            uwr.SendWebRequest();

            try
            {
                while (!uwr.isDone) await Task.Delay(5);

                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"{uwr.error}");
                }
                else
                {
                    contextMenu.UpdateContextMenu("SampleName", Path.GetFileName(path));
                    clip = DownloadHandlerAudioClip.GetContent(uwr);
                }
            }
            catch (Exception err)
            {
                Debug.Log($"{err.Message}, {err.StackTrace}");
            }
        }
        return clip;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive && other.TryGetComponent(out Marble marble) && audioPlayer.clip)
        {
            LeanTween.cancel(gameObject);
            transform.localScale = Vector2.one;
            LeanTween.scale(gameObject, new Vector2(1.2f, 1.2f), 0.4f).setEasePunch();

            playAudioPS.Stop();
            playAudioPS.Play();
            
            audioPlayer.Play();
        }
    }
}
