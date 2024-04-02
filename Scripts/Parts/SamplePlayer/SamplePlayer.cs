using System.Collections;
using System.Collections.Generic;
using SimpleFileBrowser;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine;
using System;

public class SamplePlayer : Part
{
    private AudioClip audioClip;
    [SerializeField] AudioSource audioPlayer;

    public override void ReceiveContextMenuData(ContextMenuData contextMenuData)
    {
        GetSample(contextMenuData.GetParameter<string>("PathToSample"));
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
        if (other.TryGetComponent(out Marble marble))
        {
            LeanTween.cancel(gameObject);
            transform.localScale = Vector2.one;
            LeanTween.scale(gameObject, new Vector2(1.2f, 1.2f), 0.4f).setEasePunch();
            
            audioPlayer.Play();
        }
    }
}
