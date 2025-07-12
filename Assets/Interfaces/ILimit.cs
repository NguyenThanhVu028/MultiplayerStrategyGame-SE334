using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILimit
{
    void CheckConditionAndGetLimit(ref int value);
}
