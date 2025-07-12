using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dinosaur : MonoBehaviour
{
    public enum DinoName { Triceratop, TRex, Spinosaur, Pteranodon}
    [SerializeField] private DinoName dinoName;
     public DinoName GetName() {  return dinoName; }
}
