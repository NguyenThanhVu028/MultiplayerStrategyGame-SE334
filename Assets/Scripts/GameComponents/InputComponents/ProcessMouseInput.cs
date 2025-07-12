using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ProcessMouseInput : MonoBehaviour
{
    [SerializeField] UnityEvent OnMouseOverDownEvent;
    [SerializeField] UnityEvent OnMouseOverUpEvent;
    [SerializeField] UnityEvent OnMouseDownEvent;
    [SerializeField] UnityEvent OnMouseUpEvent;
    [SerializeField] UnityEvent OnMouseEnterEvent;
    [SerializeField] UnityEvent OnMouseExitEvent;
    [SerializeField] UnityEvent OnMouseClickedEnterEvent;
    [SerializeField] UnityEvent OnMouseNotClickedEnterEvent;
    [SerializeField] UnityEvent OnMouseClickedExitEvent;
    [SerializeField] UnityEvent OnMouseNotClickedExitEvent;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) OnMouseDownEvent.Invoke();
        if (Input.GetMouseButtonUp(0)) OnMouseUpEvent.Invoke();
    }
    private void OnMouseDown()
    {
        OnMouseOverDownEvent.Invoke();
    }
    private void OnMouseUp()
    {
        OnMouseOverUpEvent.Invoke();
    }
    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0)) OnMouseClickedEnterEvent.Invoke();
        else OnMouseNotClickedEnterEvent.Invoke();
        OnMouseEnterEvent.Invoke();
    }
    private void OnMouseExit()
    {
        if (Input.GetMouseButton(0)) OnMouseClickedExitEvent.Invoke();
        else OnMouseNotClickedExitEvent.Invoke();
        OnMouseExitEvent.Invoke();
    }
}
