using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position)
    {
        GameObject go = new GameObject("OneShotAudio");
        go.transform.position = position;
        AudioSource newAudio  =  go.AddComponent<AudioSource>();

        newAudio.clip = clip;
        newAudio.loop = false;
        newAudio.Play();

        if (GameSettings.instance != null)
        {
            newAudio.volume = GameSettings.instance.GetSFXVolume();
        }

        Destroy(go, clip.length);

        return newAudio;
    }

    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, Transform parent)
    {
        AudioSource newAudio = PlayClipAtPoint(clip, position);

        newAudio.gameObject.transform.parent = parent;

        return newAudio;
    }
}
