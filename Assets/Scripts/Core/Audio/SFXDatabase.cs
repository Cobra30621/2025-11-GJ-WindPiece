using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Audio
{
    [CreateAssetMenu(fileName = "SFXDatabase", menuName = "Audio/SFX Database")]
    public class SFXDatabase : ScriptableObject
    {
        [Serializable]
        public class SFXEntry
        {
            public SFXType type;
            public AudioClip clip => clips.Count > 0 ? clips[UnityEngine.Random.Range(0, clips.Count)] : null;
            
            [Tooltip("可加入多個 Clip，每次播放會隨機選擇一個")]
            public List<AudioClip> clips = new List<AudioClip>();
        }

        public List<SFXEntry> sounds = new List<SFXEntry>();

        private Dictionary<SFXType, AudioClip> lookup;

        private void OnEnable()
        {
            lookup = new Dictionary<SFXType, AudioClip>();

            foreach (var entry in sounds)
            {
                if (!lookup.ContainsKey(entry.type))
                    lookup.Add(entry.type, entry.clip);
            }
        }

        /// <summary>
        /// 依照 Enum 取得音效
        /// </summary>
        public AudioClip GetClip(SFXType type)
        {
            if (lookup == null) OnEnable();

            if (lookup.TryGetValue(type, out var clip))
                return clip;

            Debug.LogWarning($"SFXDatabase: 找不到音效 {type}");
            return null;
        }
    }
}