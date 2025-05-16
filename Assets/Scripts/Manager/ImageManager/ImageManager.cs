using System.Collections;
using System.Collections.Generic;
using Manager.Level;
using UnityEngine;
namespace Manager.Image
{
    public class ImageManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ImageManager instance { get; private set; }
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
        List<ImageInfo> m_ImagesInfo = null;
        public Sprite GetSprite(IconType a_iconType)
        {
            foreach (ImageInfo i_imageInfo in m_ImagesInfo)
            {
                if (a_iconType == i_imageInfo.m_iconType)
                {
                    return i_imageInfo.m_sprite;
                }
            }
            Debug.LogError("ImageManager::GetSprite::icon type does not exist '" + a_iconType.ToString() + "'");
            return null;
        }
    }
}