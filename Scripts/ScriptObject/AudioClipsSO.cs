using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu()]
public class AudioClipsSO : ScriptableObject
{
    public AudioClip handgunFire;
    public AudioClip[] handgunReload;
    public AudioClip rifleFire;
    [FormerlySerializedAs("footStep")] public AudioClip[] footstep;
    public AudioClip hit;
}