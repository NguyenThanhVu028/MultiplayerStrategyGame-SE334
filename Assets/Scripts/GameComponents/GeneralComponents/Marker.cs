using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    // Markers are objects that can disappear and reappear

    [SerializeField] float appearingSpeed;
    [SerializeField] float disappearingSpeed;
    [SerializeField] float maxOpaque;
    [SerializeField] float minOpaque = 0;
    [SerializeField] float initialOpaque = 0;

    Coroutine AppearCoroutine = null;
    Coroutine DisappearCoroutine = null;

    private void Start()
    {
        float targetOpaue = (initialOpaque >= minOpaque) ? initialOpaque : minOpaque;
        targetOpaue = (initialOpaque <= maxOpaque) ? initialOpaque : maxOpaque;
        GetComponent<ICustomColor>()?.SetOpaque(initialOpaque);
    }
    public void StartAppearing()
    {
        if(AppearCoroutine != null) StopCoroutine(AppearCoroutine);
        if(DisappearCoroutine != null) StopCoroutine(DisappearCoroutine);
        AppearCoroutine = StartCoroutine(AppearingCoroutine());
    }
    public void StartDisappearing()
    {
        if (AppearCoroutine != null) StopCoroutine(AppearCoroutine);
        if (DisappearCoroutine != null) StopCoroutine(DisappearCoroutine);
        DisappearCoroutine = StartCoroutine(DisappearingCoroutine());
    }
    IEnumerator AppearingCoroutine()
    {
        if (GetComponent<ICustomColor>() == null) yield return null;
        while(GetComponent<ICustomColor>().GetColor().a < maxOpaque)
        {
            float opaque = GetComponent<ICustomColor>().GetColor().a;
            opaque += Time.deltaTime * appearingSpeed;
            if(opaque > maxOpaque) opaque = maxOpaque;
            GetComponent<ICustomColor>().SetOpaque(opaque);
            yield return null;
        }
    }
    IEnumerator DisappearingCoroutine()
    {
        if (GetComponent<ICustomColor>() == null) yield return null;

        while (GetComponent<ICustomColor>().GetColor().a > minOpaque)
        {
            float opaque = GetComponent<ICustomColor>().GetColor().a;
            opaque -= Time.deltaTime * disappearingSpeed;
            if(opaque < minOpaque) opaque = minOpaque;
            GetComponent<ICustomColor>().SetOpaque(opaque);
            yield return null;
        }
    }
}
