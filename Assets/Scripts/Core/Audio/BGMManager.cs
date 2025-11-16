using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Core.Audio
{
    public class BGMManager : MonoBehaviour
    {
        public static BGMManager Instance;

        [Header("音樂來源")]
        public AudioSource audioSource;

        [Header("音樂 Clip")]
        public AudioClip bgmClip;

        [Header("可選：到哪個場景停止播放")]
        public List<string> stopAtSceneNames ; // 空白代表不停止

        private void Awake()
        {
            // 單例模式
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            PlayBGM();
        }

        public void PlayBGM()
        {
            if (audioSource.clip != null && !audioSource.isPlaying)
                audioSource.Play();
        }

        public void StopBGM()
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (stopAtSceneNames.Contains(scene.name) )
            {
                StopBGM();
                Destroy(gameObject); // 可選，音樂停止後移除
            }
        }
    }
}