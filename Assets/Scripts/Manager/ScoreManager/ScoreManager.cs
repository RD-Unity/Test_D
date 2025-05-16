using System;
using UnityEngine;
namespace Manager.Score
{
    public class ScoreManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ScoreManager instance { get; private set; }
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
        int m_iFlipCount = 0;
        int m_iBestFlipCount = int.MaxValue;

        /// <summary>
        /// event on score update
        /// </summary>
        public event Action<int> OnScoreUpdated;

        /// <summary>
        /// event on best score update
        /// </summary>
        public event Action<int> OnBestScoreUpdated;

        /// <summary>
        /// Set flip count
        /// </summary>
        /// <param name="a_iFlipCount">flip count</param>
        public void SetFlipCount(int a_iFlipCount)
        {
            m_iFlipCount = a_iFlipCount;
            OnScoreUpdated?.Invoke(m_iFlipCount);
        }

        /// <summary>
        /// Set Best Flip Count
        /// </summary>
        /// <param name="a_iFlipCount">flip count</param>
        public void SetBestFlipCount(int a_iFlipCount)
        {
            m_iBestFlipCount = a_iFlipCount;
            OnBestScoreUpdated?.Invoke(m_iBestFlipCount);
        }

        /// <summary>
        /// increase flip count
        /// </summary>
        public void IncreaseFlipCount()
        {
            m_iFlipCount++;
            OnScoreUpdated?.Invoke(m_iFlipCount);
        }

        /// <summary>
        /// get flip count
        /// </summary>
        /// <returns>flip count</returns>
        public int GetFlipCount()
        {
            return m_iFlipCount;
        }

        /// <summary>
        /// get best flip count
        /// </summary>
        /// <returns>best flip count</returns>
        public int GetBestFlipCount()
        {
            return m_iBestFlipCount;
        }

        /// <summary>
        /// reset count
        /// </summary> 
        public void Reset()
        {
            m_iFlipCount = 0;
            m_iBestFlipCount = int.MaxValue;
            OnScoreUpdated?.Invoke(m_iFlipCount);
            OnBestScoreUpdated?.Invoke(m_iBestFlipCount);
        }
    }
}