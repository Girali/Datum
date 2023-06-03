using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public abstract bool CanInteract(float dist);
    public abstract void StartInteract(GameObject player, ControllerInputs hand, PlayerHandController phc);
    public abstract void Interacting(GameObject player, ControllerInputs hand);
    public abstract void EndInteract(GameObject player, ControllerInputs hand);
    public abstract void EnterInteract();
    public abstract void HoverInteract();
    public abstract void ExitInteract();
}
