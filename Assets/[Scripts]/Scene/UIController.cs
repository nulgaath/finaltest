using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject miniMap;

    void Start()
    {
        miniMap = GameObject.Find("MiniMap");
    }

    public void OnRestartButton_Pressed()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnYButton_Pressed()
    {
        miniMap.SetActive(!miniMap.activeInHierarchy);
    }
}
