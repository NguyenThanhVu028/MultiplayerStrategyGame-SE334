using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public enum AIMode { Easy, Hard}
    public static AIManager Instance { get; private set; }
    [SerializeField] private AIMode mode = AIMode.Easy;
    [SerializeField] private float lockWaitingTime = 5;
    [SerializeField] private List<Lock> locks;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        foreach(var item in locks)
        { 
            item?.UpdateLockTime(Time.deltaTime);
        }
    }
    public AIMode GetMode() {  return mode; }
    public bool AcquireLock(string color)
    {
        // Each AI has to ask to acquire the lock of its color in order to attack
        foreach(var item in locks)
        {
            if(item.GetLockColor() == color)
            {
                return item.AcquireLock();
            } 
        }
        locks.Add(new Lock(color, true, lockWaitingTime));
        return true;
    }
    public void ReturnLock(string color)
    {
        // Normally return lock, other cores have to wait an specified amount of time to acquire this lock
        foreach (var item in locks)
        {
            if (item.GetLockColor() == color)
            {
                item.ReturnLock();
                return;
            }
        }
        locks.Add(new Lock(color, false, lockWaitingTime));
    }
    public void ReturnLockImmediately(string color)
    {
        // Return lock and that lock can be reacquired by other cores immediately
        foreach (var item in locks)
        {
            if (item.GetLockColor() == color)
            {
                item.ReturnLockImmediately();
                return;
            }
        }
        locks.Add(new Lock(color, false, lockWaitingTime));
    }
    [Serializable]
    class Lock
    {
        [SerializeField] private string color;
        [SerializeField] private bool isLocked = false;
        [SerializeField] private float waitingTime = 5;
        [SerializeField] private float maxLockedTime = 5; // Automatically release this lock when exceed max locked time
        private float timeCounter;
        private float lastLockedTime = 0; // Used to check max locked time
        private float lastReturnedTime = 0; // Used to check waiting time

        public Lock(string color, bool isLocked, float waitingTime) { this.color = color; this.isLocked = isLocked;  timeCounter = 0; this.waitingTime = this.maxLockedTime = waitingTime; }
        public string GetLockColor() { return color; }
        public bool IsLocked() {  return isLocked; }
        public bool AcquireLock()
        {
            if (timeCounter - lastReturnedTime < waitingTime) return false;
            if (isLocked) return false;
            int rand = UnityEngine.Random.Range(0, 100);
            if(rand % 2 == 0) // A random AI will acquire this lock
            {
                isLocked = true; lastLockedTime = timeCounter;
                return true;
            }
            return false;
        }
        public void ReturnLock()
        {
            if(isLocked) lastReturnedTime = timeCounter;
            isLocked = false; 
        }
        public void ReturnLockImmediately()
        {
            isLocked = false;
        }
        public void UpdateLockTime(float time) 
        { 
            // Get called in an Update function
            timeCounter += time;
            if(timeCounter - lastLockedTime > maxLockedTime && isLocked)
            {
                isLocked = false; lastReturnedTime = timeCounter;
            }
        }

    }
}
