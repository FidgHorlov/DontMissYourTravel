using UnityEngine;

namespace DontMissTravel.Audio
{
    [CreateAssetMenu (menuName = "FidgetLand/AudioLibrary")]
    public class AudioLibrary : ScriptableObject
    {
        [SerializeField] private GroupAudioData _ambient;
        public GroupAudioData AmbientSfx => _ambient;
        
        [SerializeField] private AudioData _music;
        public AudioData Music => _music;
        
        [Space]
        [SerializeField] private AudioData _attentionSfx;
        public AudioData AttentionSfx => _attentionSfx;

        [SerializeField] private AudioData _gateOpenSfx;
        public AudioData GateOpenSfx => _gateOpenSfx;

        [SerializeField] private AudioData _menuClickSfx;
        public AudioData MenuClickSfx => _menuClickSfx;
        
        [Header("Hide Game")]
        [SerializeField] private GroupAudioData _positiveGrumbling;
        public GroupAudioData PositiveSfx => _positiveGrumbling;
        
        [SerializeField] private GroupAudioData _negativeGrumbling;
        public GroupAudioData NegativeSfx => _negativeGrumbling;
    }
}
