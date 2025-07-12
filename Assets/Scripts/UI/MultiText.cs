using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiText : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    public void SetText(int index, string text)
    {
        if (index >= texts.Count) return;
        if(texts[index] != null) texts[index].text = text;
    }
    public void SetText(string text)
    {
        if(texts.Count == 0) return;
        if(texts[0] != null) texts[0].text = text;
    }
}
