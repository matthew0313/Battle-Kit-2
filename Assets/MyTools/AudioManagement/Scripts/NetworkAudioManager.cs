using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace NetworkAudio
{
    public class NetworkAudioManager : NetworkBehaviour
    {
        public static NetworkAudioManager Instance { get; private set; }
        public NetworkAudioManager() => Instance = this;

        [SerializeField] SoundDatabase m_soundDatabase;
        [SerializeField] StringSoundPair[] soundPresets;
        [SerializeField] internal float locationUpdateThreshold = 0.01f;
        public SoundDatabase soundDatabase => m_soundDatabase;

        Pooler<AudioSource> audioSourcePool;
        readonly List<PlayingSound> playingSounds = new(), removeQueue = new();
        private void Awake()
        {
            audioSourcePool = new(() =>
            {
                AudioSource tmp = new GameObject().AddComponent<AudioSource>();
                tmp.transform.SetParent(transform);
                return tmp;
            }, 200);
        }
        private void Update()
        {
            foreach(var i in playingSounds) if (!i.source.isPlaying) removeQueue.Add(i);
            foreach(var i in removeQueue)
            {
                playingSounds.Remove(i); audioSourcePool.ReleaseObject(i.source);
            } removeQueue.Clear();
        }
        public PlayingSound PlaySound(Sound sound, Vector3 position = new())
        {
            PlayingSound tmp = new(audioSourcePool.GetObject(position, Quaternion.identity), sound, MathUtilities.GenerateRandomID());
            playingSounds.Add(tmp);
            PlaySoundRpc(soundDatabase.GetIndex(sound), position, tmp.id);
            return tmp;
        }

        [Rpc(SendTo.NotMe)]
        void PlaySoundRpc(int soundIndex, Vector3 position, ulong id)
        {
            Sound sound = soundDatabase.GetSound(soundIndex);
            PlayingSound tmp = new(audioSourcePool.GetObject(position, Quaternion.identity), sound, MathUtilities.GenerateRandomID());
            playingSounds.Add(tmp);
        }
        public void StopSound(PlayingSound sound) 
        { 
            if (sound != null) StopSoundRpc(sound.id); 
        }

        [Rpc(SendTo.Everyone)]
        void StopSoundRpc(ulong id)
        {
            for(int i = 0; i < playingSounds.Count; i++)
            {
                if (playingSounds[i].id == id)
                {
                    playingSounds.Remove(playingSounds[i]); audioSourcePool.ReleaseObject(playingSounds[i].source);
                    return;
                }
            }
        }
    }

    [System.Serializable]
    public struct StringSoundPair
    {
        public string name;
        public Sound sound;
    }
    public class PlayingSound
    {
        public readonly AudioSource source;
        public readonly ulong id;
        public PlayingSound(AudioSource source, Sound sound, ulong id)
        {
            this.source = source;
            sound.PasteTo(source);
            this.id = id;
            source.Play();
        }
        public void Stop() => NetworkAudioManager.Instance.StopSound(this);
    }
}