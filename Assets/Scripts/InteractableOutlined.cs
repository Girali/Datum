using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableOutlined : MonoBehaviour, Interactable
{
    protected PlayerHandController playerHandController;
    protected ControllerInputs controllerInputs;

    [SerializeField]
    protected MeshRenderer meshRenderer;
    [SerializeField]
    protected Material outline;

    protected Material[] outlinedMat;
    protected Material[] normalMat;

    protected bool interactable = true;

    public bool Interactable { get => interactable; set => interactable = value; }

    void Start()
    {
        UpdateMaterials();
    }

    protected virtual void UpdateMaterials()
    {
        outlinedMat = new Material[] { meshRenderer.material , outline };
        normalMat = new Material[] { meshRenderer.material };
    }

    public virtual bool CanInteract(float dist)
    {
        return interactable;
    }

    public virtual void StartInteract(GameObject player, ControllerInputs hand, PlayerHandController phc)
    {
        playerHandController = phc;
        controllerInputs = hand;
    }

    public virtual void Interacting(GameObject player, ControllerInputs hand)
    {
    }

    public virtual void EndInteract(GameObject player, ControllerInputs hand)
    {
        playerHandController = null;
        controllerInputs = null;
    }

    public virtual void EnterInteract()
    {
        meshRenderer.materials = outlinedMat;
    }

    public virtual void HoverInteract()
    {
    }

    public virtual void ExitInteract()
    {
        meshRenderer.materials = normalMat;
    }
}
