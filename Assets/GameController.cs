using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private LevelController levelController;

    private void Awake()
    {
        Time.fixedDeltaTime = 1f / 60f;
        Application.targetFrameRate = 60;

        SoundController.Instance.Init();
        playerController.Init();
        levelController.Init();
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
