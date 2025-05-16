using System;
using UnityEngine;
namespace Manager.Sound
{
    [Serializable]
    public class SoundInfo
    {
        [SerializeField]
        public string m_strAudioID;
        [SerializeField]
        public AudioClip m_audioClip;
    }
}