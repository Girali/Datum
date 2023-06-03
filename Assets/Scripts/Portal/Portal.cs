using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private Portal portal;
    [SerializeField]
    private Transform portalInverse;
    [SerializeField]
    private Collider cldr;
    private float radius = 0.133f;
    private List<PortalDetector> inside = new List<PortalDetector>();
    private List<PortalDetector> holding = new List<PortalDetector>();
    private List<Collider> insideObject = new List<Collider>();
    private List<PortalDetector> insideToIgnore = new List<PortalDetector>();
    [SerializeField]
    private float detectRadius = 1f;
    private int layer = 0;

    public float Radius { get => radius; }
    public Collider Collider { get => cldr; }
    public Transform PortalInverse { get => portalInverse; }

    private void Awake()
    {
        layer = LayerMask.GetMask("Interactable");
    }

    public void OnEnter(Collider other)
    {
        if(other != null)
        {
            if (other.tag == "CanTakePortal")
            {
                PortalDetector pd = other.GetComponent<PortalDetector>();
                if (!inside.Contains(pd))
                {
                    inside.Add(pd);
                    insideObject.Add(other);
                }

                pd.AddNotifyOnDeath(this);
                //Debug.LogError("OnTriggerEnter");
            }
        }
    }

    public void OnExit(Collider other)
    {
        if (other != null)
        {
            if (other.tag == "CanTakePortal")
            {
                PortalDetector pd = other.GetComponent<PortalDetector>();

                if (inside.Contains(pd))
                {
                    inside.Remove(pd);
                    insideObject.Remove(other);
                }

                pd.SubNotifyOnDeath(this);
                if (insideToIgnore.Contains(pd))
                    insideToIgnore.Remove(pd);
                //Debug.LogError("OnTriggerExit");
            }
        }
    }

    public void CreateSpecialClone(PortalDetector pd)
    {
        GameObject go = Instantiate(pd.Render.gameObject,pd.transform.position,pd.transform.rotation);
        PortalObjectClone poc = go.AddComponent<PortalObjectClone>();
        poc.Init(pd, this);
    }

    public void RemoveFromList(PortalDetector pd)
    {
        inside.Remove(pd);
        if (insideToIgnore.Contains(pd))
            insideToIgnore.Remove(pd);
    }

    private void OnEnable()
    {
        foreach (PortalDetector p in holding)
        {
            p.SetHold(false, this);
        }
        holding = new List<PortalDetector>();
    }

    private void Update()
    {
        Collider[] cldrs = Physics.OverlapSphere(transform.position, detectRadius, layer);

        for (int i = 0; i < cldrs.Length; i++)
        {
            Collider c = cldrs[i];
            if (!insideObject.Contains(c))
            {
                OnEnter(c);
            }
        }

        for (int i = 0; i < insideObject.Count; i++)
        {
            Collider c = insideObject[i];
            if (!cldrs.Contains(c))
            {
                OnExit(c);
            }
        }

        for (int i = 0; i < inside.Count; i++)
        {
            PortalDetector p = inside[i];
            if (insideToIgnore.Contains(p))
                p.CheckForExit(this, portal);
            else
                p.CheckForTp(this, portal);
        }
    }

    public void ExitedObject(PortalDetector p)
    {
        if(insideToIgnore.Contains(p))
            insideToIgnore.Remove(p);
        //Debug.LogError("Remove ignore " + p.name);
    }

    public void RecieveObject(PortalDetector p, bool isFront)
    {
        if (!gameObject.activeSelf)
        {
            holding.Add(p);
            p.SetHold(true, this);
        }

        if(!insideToIgnore.Contains(p))
            insideToIgnore.Add(p);
        p.TelerportFrame(this, portal, isFront);

        if (!gameObject.activeSelf)
            p.FakeCloneAnimate();

        p.Trail(true);

        //Debug.LogError("Add ignore " + p.name);
    }

    public void DetectObject(PortalDetector p, bool isFront)
    {
        if (!portal.gameObject.activeSelf)
            CreateSpecialClone(p);

        p.Trail(false);

        //Debug.DrawRay(transform.position, p.Rb.velocity,Color.red);
        Vector3 pos = transform.InverseTransformPoint(p.transform.position);
        Vector3 fwd = transform.InverseTransformDirection(p.transform.forward);
        p.transform.position = portal.PortalInverse.TransformPoint(pos);
        p.transform.rotation = Quaternion.LookRotation(portal.PortalInverse.TransformDirection(fwd));

        Vector3 v = isFront ? portal.transform.up * p.Rb.velocity.magnitude : -portal.transform.up * p.Rb.velocity.magnitude;
        p.Rb.velocity = v;

        //Debug.DrawRay(transform.position, p.Rb.velocity,Color.green);
        portal.RecieveObject(p, isFront);
    }
}
