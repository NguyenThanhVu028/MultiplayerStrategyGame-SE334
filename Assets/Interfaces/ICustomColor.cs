using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public interface ICustomColor
{
    void SetColor(string color);
    void SetColor(UnityEngine.Color color);
    void SetColor(Vector3 colorRGB);
    void SetColor(float r, float g, float b);
    void SetColor(float r, float g, float b, float a);
    UnityEngine.Color GetColor();
    void SetOpaque(float opaque);
}
