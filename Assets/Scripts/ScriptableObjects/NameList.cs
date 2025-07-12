using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NameList", menuName = "NameList")]
public class NameList : ScriptableObject
{
    [SerializeField] private List<string> names = new();

    public List<string> GetNamesList() {  return new List<string>(names); }
    public string GetAt(int index)
    {
        if(index < 0 || index >= names.Count) return null;
        return names[index];
    }
    public int Find(string name)
    {
        int res = -1;
        for(int i=0; i<names.Count; i++)
        {
            if (names[i].Equals(name))
            {
                res = i; break;
            }
        }
        return res;
    }
}
