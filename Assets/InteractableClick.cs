using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableClick : InteractableOutlined
{
    [SerializeField]
    private UnityEvent onClick;

    public override void StartInteract(GameObject player, ControllerInputs hand, PlayerHandController phc)
    {
        base.StartInteract(player, hand, phc);

        onClick.Invoke();

        meshRenderer.materials = normalMat;
    }
}
