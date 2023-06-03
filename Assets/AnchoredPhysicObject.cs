using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnchoredPhysicObject : InteractableOutlined
{
    private Rigidbody rb;
    private Transform contactPoint;

    private Vector3 startPosition;
    private Vector3 direction;

    private float totalAngle = 0;

    [SerializeField]
    private HingeJoint pivot;

    [SerializeField]
    private UnityEvent<float> onValueChange;

    private float angle = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        contactPoint = new GameObject("Contact Point").transform;
        contactPoint.localPosition = Vector3.zero;
        contactPoint.parent = transform;
        totalAngle = Mathf.Abs(Mathf.Abs(pivot.limits.min) - Mathf.Abs(pivot.limits.max));
    }

    public override void StartInteract(GameObject player, ControllerInputs hand, PlayerHandController phc)
    {
        base.StartInteract(player, hand, phc);
        contactPoint.position = phc.RaycastHit.point;
        startPosition = phc.transform.position;
        rb.isKinematic = false;
    }


    public override void Interacting(GameObject player, ControllerInputs hand)
    {
        base.Interacting(player, hand);
        direction = (hand.globalPos - startPosition).normalized;
        rb.AddForceAtPosition(direction, contactPoint.position);

        angle = pivot.angle / totalAngle;
        onValueChange.Invoke(angle);
    }

    public override void EndInteract(GameObject player, ControllerInputs hand)
    {
        base.EndInteract(player, hand);
        rb.isKinematic = true;
    }
}
