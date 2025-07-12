using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyboardInputHandler : MonoBehaviour
{
    [SerializeField] List<KeyboardEvents> keyboardEvents = new List<KeyboardEvents>();

    private void Update()
    {
        foreach(var keyboardEvent in keyboardEvents)
        {
            switch (keyboardEvent.GetAction())
            {
                case KeyboardEvents.Action.KeyDown:
                    if(Input.GetKeyDown(keyboardEvent.GetKeyCode())) keyboardEvent.GetKeyboardEvent().Invoke();
                    break;
                case KeyboardEvents.Action.KeyUp:
                    if (Input.GetKeyUp(keyboardEvent.GetKeyCode())) keyboardEvent.GetKeyboardEvent().Invoke();
                    break;
                case KeyboardEvents.Action.KeyPressed:
                    if (Input.GetKey(keyboardEvent.GetKeyCode())) keyboardEvent.GetKeyboardEvent().Invoke();
                    break;
            }
        }
    }

    [Serializable]
    private class KeyboardEvents
    {
        public enum Action { KeyDown, KeyUp, KeyPressed}
        [SerializeField] KeyCode keycode;
        [SerializeField] Action action;
        [SerializeField] UnityEvent keyboardEvent;

        public KeyCode GetKeyCode() { return keycode; }
        public Action GetAction() {  return action; }
        public UnityEvent GetKeyboardEvent() { return keyboardEvent; }
    }
}
