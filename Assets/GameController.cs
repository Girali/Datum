using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController playerController;

    [SerializeField]
    private LevelController levelController;

    private static GameController _instance;

    public static GameController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameController>();
            return _instance;
        }
    }
    
    private void Awake()
    {
        Time.fixedDeltaTime = 1f / 60f;
        Application.targetFrameRate = 60;

        SoundController.Instance.Init();
        playerController.Init();
        levelController.Init();
        
        Debug.Log ("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }

    public void QuitApp()
    {
            Application.Quit();
    }

    public void FadeIn()
    {
        GUI_Controller.Instance.fadeIn.Play();
    }

    public void FadeOut()
    {
        GUI_Controller.Instance.fadeOut.Play();
    }
}
