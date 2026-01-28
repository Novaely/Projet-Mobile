using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void LoadSceneLevel(int index)
    {
        Debug.Log("Loading Level" + index);
        SceneManager.LoadScene("Level"+index);
    }

    public void LoadSceneMainMenu()
    {
        Debug.Log("Loading MainMenu");
        SceneManager.LoadScene("MainMenu");
    }
}
