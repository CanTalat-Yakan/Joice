using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Audio Info", fileName = "Audio Info", order = 1)]
public class AudioInfo : ScriptableObject
{
    [Header("Main")]
    public AudioClip Music;
    public AudioClip Ambient;
    public AudioClip Environment;

    [Header("Menu")]
    public AudioCollection Button;
    public AudioCollection Panel;

    [Header("Player")]
    public AudioCollection[] Footsteps;
    public AudioCollection[] Landing;
    public AudioCollection Jump;
    public AudioCollection Damage;
    public AudioCollection Groan;
    public AudioCollection Swoosh;
    public AudioCollection Sword;
}

[Serializable] public struct AudioCollection { public AudioClip[] clips; }