using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Audio
{
    public class SFXManager : MonoBehaviour
    {
        public static SFXManager Instance { get; private set; }

        [Header("音效資料庫")]
        public SFXDatabase database;

        [Header("Audio Source Pool")]
        public int poolSize = 10;

        private List<AudioSource> audioPool;

        [Range(0f, 1f)]
        public float sfxVolume = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            CreateAudioPool();
        }

        private void CreateAudioPool()
        {
            audioPool = new List<AudioSource>();

            for (int i = 0; i < poolSize; i++)
            {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.loop = false;
                audioPool.Add(src);
            }
        }

        private AudioSource GetAvailableSource()
        {
            foreach (var s in audioPool)
                if (!s.isPlaying)
                    return s;
            return audioPool[0];
        }

        // 🔊 播放一次（使用 enum）
        [Button]
        public void PlaySFX(SFXType type, float volume = 1f)
        {
            var clip = database?.GetClip(type);
            if (clip == null) return;

            AudioSource src = GetAvailableSource();
            src.loop = false;
            src.volume = volume * sfxVolume;
            src.clip = clip;
            src.Play();
        }

        // 🔁 播放循環（使用 enum）
        public AudioSource PlayLoop(SFXType type, float volume = 1f)
        {
            var clip = database?.GetClip(type);
            if (clip == null) return null;

            AudioSource src = GetAvailableSource();
            src.loop = true;
            src.volume = volume * sfxVolume;
            src.clip = clip;
            src.Play();

            return src;
        }

        public void StopSFX(AudioSource src)
        {
            if (src != null)
                src.Stop();
        }
    }

}