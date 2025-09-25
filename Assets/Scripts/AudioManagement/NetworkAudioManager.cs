using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace NetworkAudio
{
    public class NetworkAudioManager : NetworkBehaviour
    {
        public static NetworkAudioManager Instance { get; private set; }
        public NetworkAudioManager() => Instance = this;

        [SerializeField] ClipDatabase m_clipDatabase;
        [SerializeField] StringSoundPair[] soundPresets;
        [SerializeField] internal float locationUpdateThreshold = 0.01f;
        public ClipDatabase clipDatabase => m_clipDatabase;

        Pooler<AudioSource> audioSourcePool;
        readonly Dictionary<ulong, PlayingSound> playingSounds = new();
        readonly List<ulong> removeQueue = new();
        private void Awake()
        {
            audioSourcePool = new(() =>
            {
                AudioSource tmp = new GameObject().AddComponent<AudioSource>();
                tmp.transform.SetParent(transform);
                return tmp;
            }, 200);
        }
        private void OnEnable()
        {
            NetworkManager.OnConnectionEvent += OnConnectionEvent;
        }
        private void OnDisable()
        {
            NetworkManager.OnConnectionEvent -= OnConnectionEvent;
        }
        private void OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
        {
            if (data.EventType == ConnectionEvent.PeerDisconnected)
            {
                foreach (var i in playingSounds)
                {
                    if (!i.Value.IsOwner && i.Value.OwnerID == data.ClientId)
                    {
                        removeQueue.Add(i.Key);
                    }
                }
                foreach (var i in removeQueue) StopSound(i); removeQueue.Clear();
            }
            else if (data.EventType == ConnectionEvent.PeerConnected)
            {
                foreach (var i in playingSounds)
                {
                    if (i.Value.IsOwner)
                    {
                        PlaySoundRpc(i.Key, i.Value.sound.ToSoundData(clipDatabase), i.Value.source.time, RpcTarget.Single(data.ClientId, RpcTargetUse.Temp));
                        i.Value.OnPeerJoin(data.ClientId);
                    }
                }
            }
        }
        private void Update()
        {
            foreach (var i in playingSounds)
            {
                if (!i.Value.source.isPlaying && i.Value.IsOwner)
                {
                    removeQueue.Add(i.Key);
                }
                else i.Value.Update();
            }
            foreach (var i in removeQueue) StopSoundRpc(i); removeQueue.Clear();
        }
        public PlayingSound PlaySound(string soundName, RpcParams rpcParams = new())
        {
            foreach (var i in soundPresets)
            {
                if (i.name == soundName)
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
            PlaySoundRpc(id, sound.ToSoundData(clipDatabase), 0.0f, rpcParams);
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
        void PlaySoundRpc(ulong id, SoundData soundData, float start, RpcParams rpcParams = new())
        {
            Sound sound = soundData.ToSound(clipDatabase);
            PlayingSound tmp = new(id, audioSourcePool.GetObject(), sound, rpcParams.Receive.SenderClientId);
            playingSounds.Add(id, tmp);
            tmp.source.Play();
            tmp.source.time = start;
        }
        [Rpc(SendTo.Everyone)]
        public void StopSoundRpc(ulong id) => StopSound(id);
        void StopSound(ulong id)
        {
            if (!playingSounds.ContainsKey(id)) return;
            playingSounds[id].removed = true;
            playingSounds[id].source.Stop();
            audioSourcePool.ReleaseObject(playingSounds[id].source);
            playingSounds.Remove(id);
        }
        [Rpc(SendTo.NotMe, AllowTargetOverride = true)]
        internal void SetLoopRpc(ulong id, bool loop, RpcParams rpcParams = new())
        {
            if (!playingSounds.ContainsKey(id)) return;
            playingSounds[id].SetLoop(loop);
        }
        [Rpc(SendTo.NotMe, AllowTargetOverride = true)]
        internal void SetLocationSettingsRpc(ulong id, float spatialBlend, float minDist, float maxDist, RpcParams rpcParams = new())
        {
            if (!playingSounds.ContainsKey(id)) return;
            playingSounds[id].SetLocationSettings(spatialBlend, minDist, maxDist);
        }
        [Rpc(SendTo.NotMe, AllowTargetOverride = true)]
        internal void SetLocationRpc(ulong id, Vector3 location, RpcParams rpcParams = new())
        {
            if (!playingSounds.ContainsKey(id)) return;
            playingSounds[id].SetLocation(location);
        }
    }
    [System.Serializable]
    [Flags]
    public enum SoundGroup
    {

    }
    public class PlayingSound
    {
        public readonly ulong id;
        public readonly AudioSource source;
        public readonly Sound sound;

        public readonly bool IsOwner = true;
        public readonly ulong OwnerID = 0;

        public bool removed = false;
        public PlayingSound(ulong id, AudioSource source, Sound sound)
        {
            this.id = id;
            this.source = source;
            this.sound = sound;
            source.clip = sound.clip;
        }
        public PlayingSound(ulong id, AudioSource source, Sound sound, ulong OwnerID)
        {
            this.id = id;
            this.source = source;
            this.sound = sound;
            source.clip = sound.clip;
            source.loop = false;
            source.spatialBlend = 0.0f;

            IsOwner = false;
            this.OwnerID = OwnerID;
        }
        NetworkAudioManager manager => NetworkAudioManager.Instance;
        public void Update()
        {
            if (hasLocation)
            {
                if(Vector3.Distance(source.transform.position, locationGetter.Invoke()) > manager.locationUpdateThreshold)
                {
                    SetLocation(locationGetter.Invoke());
                }
            }
        }
        public void OnPeerJoin(ulong clientID)
        {
            var target = manager.RpcTarget.Single(clientID, RpcTargetUse.Persistent);
            if(source.loop) manager.SetLoopRpc(id, source.loop, target);
            if(hasLocation) manager.SetLocationSettingsRpc(id, source.spatialBlend, source.minDistance, source.maxDistance);
            target.Dispose();
        }
        public void Stop()
        {
            if(!removed) manager.StopSoundRpc(id);
        }
        public PlayingSound SetLoop(bool loop)
        {
            if (IsOwner) manager.SetLoopRpc(id, loop);
            source.loop = loop;
            if (!source.isPlaying) source.Play();
            return this;
        }
        bool hasLocation = false;
        Func<Vector3> locationGetter;
        public PlayingSound SetLocationSettings(Func<Vector3> locationGetter, float spatialBlend, float minDist, float maxDist)
        {
            this.locationGetter = locationGetter;
            SetLocationSettings(spatialBlend, minDist, maxDist);
            SetLocation(locationGetter.Invoke());
            return this;
        }
        internal void SetLocationSettings(float spatialBlend, float minDist, float maxDist)
        {
            if (IsOwner) manager.SetLocationSettingsRpc(id, spatialBlend, minDist, maxDist);
            hasLocation = true;
            source.spatialBlend = spatialBlend;
            source.minDistance = minDist; source.maxDistance = maxDist;
        }
        internal void SetLocation(Vector3 location)
        {
            if (IsOwner) manager.SetLocationRpc(id, location);
            source.transform.position = location;
        }
    }
    [System.Serializable]
    public struct StringSoundPair
    {
        public string name;
        public string sound;
    }
}