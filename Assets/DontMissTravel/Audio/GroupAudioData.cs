using System;
using System.Collections.Generic;
using UnityEngine;

namespace DontMissTravel.Audio
{
    [Serializable]
    public class GroupAudioData
    {
        [SerializeField] private AudioType _audioType;
        [SerializeField] private List<AudioClip> _audioClips;
        
        public AudioType AudioType => _audioType;
        public List<AudioClip> AudioClips => _audioClips;
    }
}