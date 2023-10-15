using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnchoredPhysicLeverObject : InteractableOutlined
{
    private Rigidbody rb;
    private Transform contactPoint;

    private Vector3 startContactPosition;
    private Vector3 startPosition;
    private Vector3 direction;

    private float totalAngle = 0;

    private Vector2 limits;

    [SerializeField]
    private HingeJoint pivot;

    [SerializeField]
    private UnityEvent<float> onValueChange;

    private float interactDistance = 0.4f;
    private float angle = 0;
    private Vector3 startAngle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        contactPoint = new GameObject("Contact Point").transform;
        contactPoint.localPosition = Vector3.zero;
        contactPoint.parent = transform;
        totalAngle = Mathf.Abs(Mathf.Abs(pivot.limits.min) - Mathf.Abs(pivot.limits.max));

        startAngle = transform.forward;

        Vector3 v = transform.localRotation.eulerAngles;
        limits = new Vector2( v.x - pivot.limits.max, v.x);
    }

    public override bool CanInteract(float dist)
    {
        return interactable && dist < interactDistance;
    }

    public override void StartInteract(GameObject player, ControllerInputs hand, PlayerHandController phc)
    {
        base.StartInteract(player, hand, phc);
        contactPoint.position = phc.RaycastHit.point;
        startPosition = phc.transform.position;
        startContactPosition = contactPoint.position;

        SoundController.Instance.PlaySFX(SFXController.Sounds.Lever_grab, hand.globalPos);
    }

    public override void Interacting(GameObject player, ControllerInputs hand)
    {
        base.Interacting(player, hand);
        direction = playerHandController.transform.position - pivot.transform.position;

        Vector3 v = pivot.transform.right;
        v = new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        Vector3 s = v - Vector3.one;
        s = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));

        direction = Vector3.Scale(direction, s);
        direction.Normalize();

        transform.rotation = Quaternion.LookRotation(direction);

        Vector3 f = transform.localRotation.eulerAngles;
        f = new Vector3(Mathf.Clamp(f.x, limits.x, limits.y), f.y, f.z);

        transform.localRotation = Quaternion.Euler(f);

        angle = Vector3.Angle(startAngle, transform.forward);

        onValueChange.Invoke(angle / totalAngle);
    }

    public override void EndInteract(GameObject player, ControllerInputs hand)
    {
        base.EndInteract(player, hand);

        SoundController.Instance.PlaySFX(SFXController.Sounds.Lever_release, hand.globalPos);
    }
}
