using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Sound", menuName = "AudioManagement/Sound")]
public class Sound : ScriptableObject
{
    public AudioResource audioSource;
    public AudioMixerGroup output;
    public bool bypassEffects, bypassListenerEffects, bypassReverbZones;
    public int priority = 128;
    public float volume = 1.0f, pitch = 1.0f, stereoPan = 0.0f, spatialBlend = 0.0f, reverbZoneMix = 1.0f;
    public float dopplerLevel = 1.0f, spread;
    public AudioRolloffMode volumeRolloff;
    public float minDistance, maxDistance;
}