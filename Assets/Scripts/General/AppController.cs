using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class AppController : MonoBehaviour
{
    public static bool IsSteamVRActive = false;
    private void Awake()
    {
        IsSteamVRActive = "Open VR Loader" == XRGeneralSettings.Instance.AssignedSettings.activeLoader.name;
        
        Debug.Log("Initiate as " + (IsSteamVRActive ? "SteamVR Rig" : "OpenXR Rig"));
    }
}
