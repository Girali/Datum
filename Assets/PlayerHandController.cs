using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private int raycastLayer;

    private bool portal = false;
    private bool interacting = false;

    private float thickness = 0.1f;
    private float maxDist = 5f;

    private Interactable interactable = null;
    private GameObject interacbleObject = null;

    [SerializeField]
    private GameObject portalObject;
    [SerializeField]
    private Animator animator;

    private ControllerInputs controllerInputs;

    private RaycastHit hit;
    public RaycastHit RaycastHit { get { return hit; } }

    private void Awake()
    {
        raycastLayer = LayerMask.GetMask("Interactable");
    }

    public void InteruptCurrentInteract() 
    {
        interacting = false;
        interactable.EndInteract(player, controllerInputs);

        interactable = null;
        interacbleObject = null;
    }

    public void Motor(ControllerInputs ci)
    {
        controllerInputs = ci;

        if (!portal)
        {
            if (!interacting)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                bool b = Physics.SphereCast(ray, thickness, out hit, maxDist, raycastLayer);
                if (b == false)
                {
                    Collider[] cldrs = Physics.OverlapSphere(transform.position, thickness * 3,  raycastLayer);
                    if (cldrs.Length > 0)
                    {
                        Collider c = cldrs[0];
                        Vector3 v = c.ClosestPoint(transform.position);
                        Vector3 dir = v - transform.position;
                        b = Physics.Raycast(transform.position, dir, out hit, maxDist, raycastLayer);
                    }
                }

                if (b)
                {
                    Interactable i = hit.transform.GetComponent<Interactable>();

                    if (i.CanInteract(hit.distance))
                    {
                        if (interactable != i)
                        {
                            interactable = hit.transform.GetComponent<Interactable>();
                            interacbleObject = hit.transform.gameObject;

                            interactable.EnterInteract();

                            animator.SetBool("Selected", true);
                        }
                        else
                        {
                            interactable.HoverInteract();
                        }
                    }
                }
                else
                {
                    if (interactable != null)
                    {
                        interactable.ExitInteract();

                        interactable = null;
                        interacbleObject = null;

                        animator.SetBool("Selected", false);
                    }
                }

                if (controllerInputs.select.activatedThisFrame)
                {
                    portal = true;
                    animator.SetBool("Casting", true);
                    portalObject.SetActive(true);
                }
            }

            if (interactable != null)
            {
                if (controllerInputs.active.activatedThisFrame)
                {
                    interacting = true;
                    interactable.StartInteract(player, controllerInputs, this);
                }

                if (controllerInputs.active.active)
                {
                    interactable.Interacting(player, controllerInputs);
                }

                if (controllerInputs.active.deactivatedThisFrame)
                {

                    interacting = false;
                    interactable.EndInteract(player, controllerInputs);

                    interactable = null;
                    interacbleObject = null;
                }
            }
        }
        else
        {
            if (controllerInputs.select.deactivatedThisFrame)
            {
                portal = false;
                animator.SetBool("Casting", false);
                portalObject.SetActive(false);
            }
        }
    }
}
