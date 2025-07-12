using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiImage : MonoBehaviour
{
    [SerializeField] List<Image> images = new List<Image>();
    [SerializeField] Images imagesConfig;
    public void SetImage(string imageName)
    {
        if (imagesConfig != null)
        {
            Sprite sprite = imagesConfig.GetImage(imageName);
            if (images[0] != null) images[0].sprite = sprite;
        }
    }
    public void SetImage(int index, string imageName)
    {
        if(imagesConfig != null)
        {
            Sprite sprite = imagesConfig.GetImage(imageName);
            if(sprite != null && images[index] != null) images[index].sprite = sprite;
        }
    }
    public void SetImage(int index, Sprite sprite)
    {
        if (index >= images.Count) return;
        if (images[index] != null) images[index].sprite = sprite;
    }
    public void SetImage(Sprite sprite)
    {
        if (images.Count == 0) return;
        if (images[0] != null) images[0].sprite = sprite;
    }
}
