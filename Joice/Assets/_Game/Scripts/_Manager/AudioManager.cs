using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] internal AudioInfo m_AudioInfo;
    [SerializeField] internal AudioMixerGroup m_AmbientMixer;
    [SerializeField] internal AudioMixerGroup m_EffectMixer;
    AudioSource m_mainMusicSource, m_mainBattleSource;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance is null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        PlayMainMusic(0.14f);
    }

    internal AudioSource Play(AudioClip _clip, bool _loop = false, float _volume = 1, float _pitch = 1)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.rolloffMode = AudioRolloffMode.Custom;
        audioSource.clip = _clip;
        audioSource.loop = _loop;
        audioSource.volume = _volume;
        audioSource.maxDistance = 130;
        audioSource.spread = 1;
        audioSource.dopplerLevel = 0;
        audioSource.reverbZoneMix = 1;
        audioSource.spatialBlend = 0;
        audioSource.pitch = _pitch;
        audioSource.Play();
        Destroy(audioSource, _clip.length);

        return audioSource;
    }
    internal static AudioSource Play(AudioSource _source, AudioClip _clip, bool? _loop = null, float? _volume = null, float? _pitch = null)
    {
        _source.clip = _clip;
        if (_loop != null) _source.loop = _loop.Value;
        if (_volume != null) _source.volume = _volume.Value;
        if (_pitch != null) _source.pitch = _pitch.Value;
        _source.Play();

        return _source;
    }

    internal AudioSource[] PlaySequence(params AudioClip[] _clips)
    {
        AudioSource[] audiosources = new AudioSource[_clips.Length];

        for (int i = 0; i < _clips.Length; i++)
        {
            audiosources[i] = gameObject.AddComponent<AudioSource>();
            audiosources[i].clip = _clips[i];

            audiosources[i].volume = 1;
            audiosources[i].rolloffMode = AudioRolloffMode.Custom;
            audiosources[i].spatialBlend = 0.1f;
            audiosources[i].maxDistance = 130;
            audiosources[i].spread = 1;
            audiosources[i].dopplerLevel = 0;
            audiosources[i].reverbZoneMix = 1;
            audiosources[i].pitch = 1;

            ulong delay = 0;
            for (int j = 0; j < i; j++)
                delay += (ulong)_clips[j].length;

            audiosources[i].Play(delay);
            Destroy(audiosources[i], _clips[i].length);
        }

        return audiosources;
    }

    internal void PlayMainMusic(float _volume = 1)
    {
        if (m_mainMusicSource || m_AudioInfo.Music is null)
            return;

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = m_AudioInfo.Music;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = _volume;
        audioSource.pitch = 1;
        audioSource.loop = true;
        audioSource.Play();
        m_mainMusicSource = audioSource;
    }

    internal void StopMainMusic() { if (m_mainMusicSource != null) m_mainMusicSource.Stop(); }
    internal void StartMainMusic() { if (m_mainMusicSource != null) m_mainMusicSource.Play(); }

    internal static T PlayRandomFromList<T>(ref T[] _list) { return _list[Random.Range(0, _list.Length)]; }
}