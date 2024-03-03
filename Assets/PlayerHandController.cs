using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private int raycastLayer;

    private bool interacting = false;

    private float thickness = 0.1f;
    private float maxDist = 5f;

    private Interactable interactable = null;
    private GameObject interacbleObject = null;

    [SerializeField]
    private Animator animator;

    private ControllerInputs controllerInputs;

    private RaycastHit hit;
    public RaycastHit RaycastHit { get { return hit; } }

    public DUI_WeaponCarousel weaponCarousel;
    private bool inCarousel = false;

    private Transform pointer;

    public Weapon[] weapons;
    private int currentWeaponIndex;

    public Transform firePosition;
    public Transform initialOffset;
    public Transform offset;

    public bool carouselEnable = true;
    
    public Transform Pointer
    {
        set => pointer = value;
    }
    
    public void AddAmmo(float f)
    {
        foreach (Weapon w in weapons)
        {
            w.AddAmmo((int)f);
        }
    }
    
    private void Awake()
    {
        raycastLayer = LayerMask.GetMask("Interactable");
        weaponCarousel.SetActive(false);

        foreach (Weapon weapon in weapons)
        {
            weapon.Init(this);
        }
        
        weapons[0].Switch(true);
    }

    public void InteruptCurrentInteract() 
    {
        interacting = false;
        interactable.EndInteract(player, controllerInputs);

        interactable = null;
        interacbleObject = null;
    }

    private void Update()
    {
        offset.localPosition = Vector3.Lerp(offset.localPosition, initialOffset.localPosition, 0.1f);
    }

    public PlayerController.PlayerState Motor(ControllerInputs ci, PlayerController.PlayerState playerState)
    {
        controllerInputs = ci;
        if (carouselEnable)
        {
            if (controllerInputs.select.activatedThisFrame)
            {
                weaponCarousel.SetActive(true);
                inCarousel = true;
            }

            if (inCarousel)
            {
                playerState.weaponCarouselOpened = true;
                int weaponIndex = weaponCarousel.UpdateView(controllerInputs.joystick.x, controllerInputs.joystick.y);

                if (weaponIndex != -1)
                {
                    if (currentWeaponIndex != weaponIndex)
                    {
                        for (int i = 0; i < weapons.Length; i++)
                        {
                            weapons[i].Switch(weaponIndex == i);
                        }

                        currentWeaponIndex = weaponIndex;
                    }
                }
            }
            
            if (controllerInputs.select.deactivatedThisFrame)
            {
                weaponCarousel.SetActive(false);
                inCarousel = false;
            }
        }

        if (currentWeaponIndex != -1)
        {
            playerState = weapons[currentWeaponIndex].Fire(ci, playerState);
            offset.localPosition = firePosition.localPosition;

            foreach (Weapon w in weapons)
            {
                w.ExternalUpdate();
            }
        }
        
        // if (!interacting)
        // {
        //     Ray ray = new Ray(pointer.position, pointer.forward);
        //     bool b = Physics.SphereCast(ray, thickness, out hit, maxDist, raycastLayer);
        //     if (b == false)
        //     {
        //         Collider[] cldrs = Physics.OverlapSphere(transform.position, thickness * 3,  raycastLayer);
        //         if (cldrs.Length > 0)
        //         {
        //             Collider c = cldrs[0];
        //             Vector3 v = c.ClosestPoint(transform.position);
        //             Vector3 dir = v - transform.position;
        //             b = Physics.Raycast(transform.position, dir, out hit, maxDist, raycastLayer);
        //         }
        //     }
        //
        //     if (b)
        //     {
        //         Interactable i = hit.transform.GetComponent<Interactable>();
        //
        //         if (i.CanInteract(hit.distance))
        //         {
        //             if (interactable != i)
        //             {
        //                 interactable = hit.transform.GetComponent<Interactable>();
        //                 interacbleObject = hit.transform.gameObject;
        //
        //                 interactable.EnterInteract();
        //
        //                 animator.SetBool("Selected", true);
        //             }
        //             else
        //             {
        //                 interactable.HoverInteract();
        //             }
        //         }
        //     }
        //     else
        //     {
        //         if (interactable != null)
        //         {
        //             interactable.ExitInteract();
        //
        //             interactable = null;
        //             interacbleObject = null;
        //
        //             animator.SetBool("Selected", false);
        //         }
        //     }
        // }
        //
        // if (interactable != null)
        // {
        //     if (controllerInputs.active.activatedThisFrame)
        //     {
        //         interacting = true;
        //         interactable.StartInteract(player, controllerInputs, this);
        //     }
        //
        //     if (controllerInputs.active.active)
        //     {
        //         interactable.Interacting(player, controllerInputs);
        //     }
        //
        //     if (controllerInputs.active.deactivatedThisFrame)
        //     {
        //
        //         interacting = false;
        //         interactable.EndInteract(player, controllerInputs);
        //
        //         interactable = null;
        //         interacbleObject = null;
        //     }
        // }

        return playerState;
    }
}
