using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DontMissTravel.Audio
{
    public enum AudioType
    {
        None,
        MenuClick,
        Gate,
        Attention,
        HideGamePositive,
        HideGameNegative
    }

    public class AudioManager : Singleton<AudioManager>
    {
        private const float AmbientVolume = 0.6f;
        private const float DefaultPitch = 1f;
        private const float PitchMin = -3f;
        private const float PitchMax = 3f;

        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private AudioSource _ambientAudioSource;
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private AudioLibrary _audioLibrary;

        private bool _isSoundOn;
        private bool _isMusicOn;
        
        private void Start()
        {
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != this)
            {
                Destroy(audioManager);
            }
            
            DontDestroyOnLoad(this);
        }
        
        public void PlayMusic()
        {
            SetAudio(_musicAudioSource, _audioLibrary.Music.AudioClip);
            _musicAudioSource.Play();
        }

        public void PlayAmbient()
        {
            SetAudio(_ambientAudioSource, GetRandomAudio(_audioLibrary.AmbientSfx), AmbientVolume);
            _ambientAudioSource.Play();
        }

        public void PauseMusic()
        {
            _musicAudioSource.Pause();
        }

        public void PauseAmbient()
        {
            _ambientAudioSource.Pause();
        }

        public void PlaySfx(AudioType audioType, bool defaultPitch = true)
        {
            AudioClip targetAudioClip = null;
            switch (audioType)
            {
                case AudioType.MenuClick:
                    targetAudioClip = GetAudioClip(_audioLibrary.MenuClickSfx);
                    break;
                case AudioType.Gate:
                    targetAudioClip = GetAudioClip(_audioLibrary.GateOpenSfx);
                    break;
                case AudioType.Attention:
                    targetAudioClip = GetAudioClip(_audioLibrary.AttentionSfx);
                    break;
                case AudioType.HideGamePositive:
                    targetAudioClip = GetRandomAudio(_audioLibrary.PositiveSfx);
                    break;
                case AudioType.HideGameNegative:
                    targetAudioClip = GetRandomAudio(_audioLibrary.NegativeSfx);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioType), audioType, null);
            }

            _sfxAudioSource.pitch = defaultPitch ? DefaultPitch : Random.Range(PitchMin, PitchMax);;
            PlayOneShot(targetAudioClip);
        }

        public bool ToggleSound()
        {
            _isSoundOn = !_isSoundOn;
            _sfxAudioSource.mute = _isSoundOn;
            return _isSoundOn;
        }

        public bool ToggleMusic()
        {
            _isMusicOn = !_isMusicOn;
            _musicAudioSource.mute = _isMusicOn;
            _ambientAudioSource.mute = _isMusicOn;
            return _isMusicOn;
        }

        private void PlayOneShot(AudioClip audioClip)
        {
            _sfxAudioSource.PlayOneShot(audioClip);
        }

        private AudioClip GetRandomAudio(GroupAudioData groupAudioData)
        {
            int index = Random.Range(0, groupAudioData.AudioClips.Count - 1);
            if (groupAudioData.AudioClips[index] == null)
            {
                Debug.LogError($"Can't find audio clip");
                return null;
            }

            return groupAudioData.AudioClips[index];
        }

        private AudioClip GetAudioClip(AudioData audioData)
        {
            return audioData.AudioClip;
        }

        private void SetAudio(AudioSource audioSource, AudioClip audioClip, float volume = 1f)
        {
            if (audioSource.clip != null)
            {
                return;
            }

            audioSource.clip = audioClip;
            audioSource.loop = true;
            audioSource.volume = volume;
        }
    }
}