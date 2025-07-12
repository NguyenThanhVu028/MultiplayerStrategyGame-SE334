using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class OpenScene : MonoBehaviour
{
    [SerializeField]
    private string sceneName;
    
    public void OnActivated()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
