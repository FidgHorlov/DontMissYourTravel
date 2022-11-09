using System;
using UnityEngine;

namespace DontMissTravel.Audio
{
    [Serializable]
    public class AudioData
    {
        [SerializeField] private AudioType _audioType;
        [SerializeField] private AudioClip _audioClip;
        
        public AudioType AudioType => _audioType;
        public AudioClip AudioClip => _audioClip;
    }
}