using System;
using Manager.Level;
using UnityEngine;
namespace Manager.Image
{
    [Serializable]
    public class ImageInfo
    {
        [SerializeField]
        public IconType m_iconType;
        [SerializeField]
        public Sprite m_sprite;
    }
}