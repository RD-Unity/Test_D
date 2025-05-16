using System.Collections.Generic;
using UnityEngine;
namespace Manager.Sound
{
    public class SoundManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SoundManager instance { get; private set; }
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        [SerializeField]
        List<SoundInfo> m_soundsInfo = null;

        Dictionary<string, SoundInfo> m_dictSoundData = new Dictionary<string, SoundInfo>();

        [SerializeField]
        AudioSource m_sfxAudioSource = null;

        void Start()
        {
            foreach (SoundInfo i_soundInfo in m_soundsInfo)
            {
                if (m_dictSoundData.ContainsKey(i_soundInfo.m_strAudioID))
                {
                    Debug.LogError("SoundManager::Start::Duplicate key found '" + i_soundInfo.m_strAudioID + "', so updating sound info");
                }
                m_dictSoundData.Add(i_soundInfo.m_strAudioID, i_soundInfo);
            }
        }

        /// <summary>
        /// Play audio as SFX
        /// </summary>
        /// <param name="a_strAudioID">audio id</param>
        public void PlaySFX(string a_strAudioID)
        {
            SoundInfo l_soundInfo = m_dictSoundData[a_strAudioID];
            if (m_dictSoundData.TryGetValue(a_strAudioID, out l_soundInfo))
            {
                m_sfxAudioSource.PlayOneShot(l_soundInfo.m_audioClip);
            }
            else
            {
                Debug.LogError("SoundManager::PlaySFX::key does not exist '" + a_strAudioID + "'");
            }
        }
    }
}