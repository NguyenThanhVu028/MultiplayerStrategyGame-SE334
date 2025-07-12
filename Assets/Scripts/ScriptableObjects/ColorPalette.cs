using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Configs/ColorPalette")]
public class ColorPalette : ScriptableObject
{
    [SerializeField] List<ColorDetail> colorDetails = new List<ColorDetail>();

    public Color GetColor(string colorName)
    {
        foreach(var colorDetail in colorDetails)
        {
            if(colorDetail.GetColorName() == colorName) return colorDetail.GetColor();
        }
        return Color.white;
    }
    public Vector3 GetColorRGB(string colorName)
    {
        Vector3 temp = new Vector3(1, 1, 1);
        foreach (var colorDetail in colorDetails)
        {
            if (colorDetail.GetColorName() == colorName) return colorDetail.GetColorRGB();
        }
        return temp;
    }

    [Serializable]
    class ColorDetail
    {
        [SerializeField] private string colorName;
        [SerializeField] private Color color;

        public string GetColorName()
        {
            if (colorName.IsNullOrEmpty()) return "";
            return colorName;
        }
        public Color GetColor()
        {
            if (color == null) return Color.white;
            return color;
        }
        public Vector3 GetColorRGB()
        {
            Vector3 temp = new Vector3(1, 1, 1);
            temp.x = color.r;
            temp.y = color.g;
            temp.z = color.b;
            return temp;
        }
    }
}
