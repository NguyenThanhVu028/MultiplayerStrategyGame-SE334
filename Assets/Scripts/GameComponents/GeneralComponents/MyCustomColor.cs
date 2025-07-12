using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class MyCustomColor : MonoBehaviour, ICustomColor
{
    [Header("Properties")]
    [SerializeField] private bool autoSetColor = true;
    [SerializeField] private SetColorType setColorType;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float opaque = 1;

    [Header("Color palette")]
    [SerializeField] private ColorPalette paletteConfig;

    [Header("Components to change color")]
    [SerializeField] private List<Components> componentsToChangeColor = new List<Components>();

    private string currentColorTag;

    private enum SetColorType { ByTag, ByColor}
    private enum Components { SpriteRenderer, Renderer, Image, TextMeshPro, TextMeshProUGUI}

    void Update()
    {
        if (!autoSetColor) return;
        switch (setColorType)
        {
            case SetColorType.ByTag:
                ChangeColorByTag();
                break;
        }

        color.a = opaque;
        foreach (var component in componentsToChangeColor)
        {
            switch (component)
            {
                case Components.SpriteRenderer:
                    if (GetComponent<SpriteRenderer>()?.color != color) GetComponent<SpriteRenderer>().color = color;
                    break;
                case Components.Renderer:
                    if (GetComponent<Renderer>()?.material.color != color) GetComponent<Renderer>().material.color = color;
                    break;
                case Components.Image:
                    if (GetComponent<Image>()?.color != color) GetComponent<Image>().color = color;
                    break;
                case Components.TextMeshProUGUI:
                    if (GetComponent<TextMeshProUGUI>()?.color != color) GetComponent<TextMeshProUGUI>().color = color;
                    break;
                case Components.TextMeshPro:
                    if (GetComponent<TextMeshPro>()?.color != color) GetComponent<TextMeshPro>().color = color;
                    break;
            }
        }
    }
    public void SetColor(string color)
    {
        Color newColor = paletteConfig.GetColor(color);
        newColor.a = opaque;
        this.color = newColor;
    }
    public void SetColor(Color color)
    {
        color.a = opaque;
        this.color = color;
    }
    public void SetColor(Vector3 colorRGB)
    {
        Color newColor = new Color(colorRGB.x, colorRGB.y, colorRGB.z, opaque);
        this.color = newColor;
        opaque = this.color.a;
    }
    public void SetColor(float r, float g, float b)
    {
        Color newColor = new Color(r, g, b, opaque);
        this.color = newColor;
    }
    public void SetColor(float r, float g, float b, float a)
    {
        opaque = a;
        Color newColor = new Color(r, g, b, a);
        this.color = newColor;
    }
    public Color GetColor() { return color; }
    public void SetOpaque(float opaque)
    {
        this.opaque = opaque;
    }
    private void ChangeColorByTag()
    {
        if(currentColorTag.IsNullOrEmpty() || currentColorTag != gameObject.tag)
        {
            currentColorTag = gameObject.tag;
            Color newColor = paletteConfig.GetColor(gameObject.tag);
            this.color = newColor;
        }
    }
}
