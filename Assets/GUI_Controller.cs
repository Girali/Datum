using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI_Controller : MonoBehaviour
{
    private static GUI_Controller instance;
    public static GUI_Controller Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GUI_Controller>();
            }
            return instance;
        }
    }

    public Jun_TweenRuntime fadeIn;
    public Jun_TweenRuntime fadeOut;
}
