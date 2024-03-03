using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class AppController : MonoBehaviour
{
    public static bool IsSteamVRActive = false;

    private void Awake()
    {

    }

    private void Start()
    {
        Debug.Log("Initiate as " + XRGeneralSettings.Instance.AssignedSettings.activeLoader.name);
        IsSteamVRActive = "Open VR Loader" == XRGeneralSettings.Instance.AssignedSettings.activeLoader.name;
        Debug.Log("Initiate as " + (IsSteamVRActive ? "SteamVR Rig" : "OpenXR Rig"));
        
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = 1 / 60f;
        QualitySettings.vSyncCount = 0;
    }
}
