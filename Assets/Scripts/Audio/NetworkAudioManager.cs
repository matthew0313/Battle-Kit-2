using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkAudioManager : NetworkBehaviour
{
    public static NetworkAudioManager Instance { get; private set; }
    public NetworkAudioManager() => Instance = this;

    [SerializeField] ClipDatabase m_clipDatabase;
    [SerializeField] StringSoundPair[] soundPresets;
    public ClipDatabase clipDatabase => m_clipDatabase;

    Pooler<AudioSource> audioSourcePool;
    readonly Dictionary<ulong, PlayingSound> playingSounds = new();
    readonly List<ulong> removeQueue = new();
    private void Awake()
    {
        audioSourcePool = new(() =>
        {
            return gameObject.AddComponent<AudioSource>();
        }, 100)
        {
            onTakeout = item => item.enabled = true,
            onRelease = item => item.enabled = false,
        };
    }
    private void Update()
    {
        foreach(var i in playingSounds)
        {
            if (!i.Value.source.isPlaying)
            {
                removeQueue.Add(i.Key);
                audioSourcePool.ReleaseObject(i.Value.source);
            }
        }
        foreach (var i in removeQueue) playingSounds.Remove(i); removeQueue.Clear();
    }
    public PlayingSound PlaySound(string soundName, RpcParams rpcParams = new())
    {
        foreach(var i in soundPresets)
        {
            if(i.name == soundName)
            {
                return PlaySound(i.sound, rpcParams);
            }
        }
        return null;
    }
    public PlayingSound PlaySound(Sound sound, RpcParams rpcParams = new(), bool includeLocal = true)
    {
        if (sound.clip == null) return null;
        ulong id; do
        {
            id = MathUtilities.GenerateRandomID();
        } while (playingSounds.ContainsKey(id));
        PlaySoundRpc(id, sound.ToSoundData(), rpcParams);
        if (includeLocal)
        {
            PlayingSound tmp = new(id, audioSourcePool.GetObject(), sound);
            playingSounds.Add(id, tmp);
            tmp.source.Play();
            return tmp;
        }
        return null;
    }
    [Rpc(SendTo.NotMe, AllowTargetOverride = true)]
    public void PlaySoundRpc(ulong id, SoundData soundData, RpcParams rpcParams)
    {
        Sound sound = soundData.ToSound();
        PlayingSound tmp = new(id, audioSourcePool.GetObject(), sound);
        playingSounds.Add(id, tmp);
        tmp.source.Play();
    }
    public void StopSound(PlayingSound sound)
    {
        if(sound != null) StopSoundRpc(sound.id);
    }
    [Rpc(SendTo.Everyone)]
    public void StopSoundRpc(ulong id)
    {
        if(!playingSounds.ContainsKey(id)) return;
        playingSounds[id].source.Stop();
    }
}
public class PlayingSound
{
    public readonly ulong id;
    public readonly AudioSource source;
    public readonly Sound sound;
    public PlayingSound(ulong id, AudioSource source, Sound sound)
    {
        this.id = id;
        this.source = source;
        this.sound = sound;
        source.clip = sound.clip;
    }
}
[System.Serializable]
public struct StringSoundPair
{
    public string name;
    public string sound;
}