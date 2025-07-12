using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Images", menuName = "Configs/Images")]
public class Images : ScriptableObject
{
    [SerializeField] private List<ImageInfo> images = new List<ImageInfo>();

    public Sprite GetImage(string imageName)
    {
        foreach(var item in images) if(item?.GetImageName() == imageName) return item?.GetSprite();
        return null;
    }

    [Serializable]
    public class ImageInfo
    {
        [SerializeField] private string imageName;
        [SerializeField] private Sprite sprite;

        public string GetImageName() { return imageName; }
        public Sprite GetSprite() { return sprite; }
    }
}
