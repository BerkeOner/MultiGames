using System;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
    public bool loop;

    [HideInInspector] public AudioSource source;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public Sound[] sounds;

    float volume;

    void Awake()
    {
        Instance = this;

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
        }
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, x => x.name == name);

        if (!sound.source.loop)
            sound.source.Play();
        else if (sound.source.loop && !sound.source.isPlaying)
            sound.source.Play();
    }

    public void Stop(string name)
    {
        Sound sound = Array.Find(sounds, x => x.name == name);

        sound.source.Stop();
    }
}
