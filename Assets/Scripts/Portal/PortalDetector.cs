using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortalDetector : MonoBehaviour
{
    private Vector3 lastVelocity;
    private Rigidbody rb;
    [SerializeField]
    private Collider cldr;

    private Portal portalToTpNextStep = null;
    private bool isFrontPortalToTpNextStep = true;

    private Portal portalToIgnoreRenter = null;
    private float timeRenter = 0;
    private bool isFrontPortalToIgnoreRenter = false;

    private Portal portalToExitNextStep = null;
    private List<Portal> insideOf = new List<Portal>();

    [SerializeField]
    private Renderer render;
    [SerializeField]
    private GameObject trail;
    private GameObject currentTrail;
    private bool trailNextFrame = false;
    private bool trailNextNextFrame = false;
    private bool sliced = false;
    private bool sliceDirSameAsPortal = false;
    private bool lockSliceDir = false;
    private Renderer clone;
    private bool cloneShowedInAnimation = false;

    public Rigidbody Rb { get => rb; }
    public Renderer Render { get => render; }

    private Vector3 holdPosition;
    private Vector3 holdDirection;
    private Vector3 holdVelocity;
    private int inRangeOf = 0;
    private bool correctingPosition = false;
    private Vector3 positionAfterTeleport = Vector3.zero;
    private bool teleportedLastFrame = false;
    private bool showThisFrame = false;
    private bool hideThisFrame = false;
    private int currentUpdateIndex = 0;

    public void FakeCloneAnimate()
    {
        cloneShowedInAnimation = true;
    }

    public void Trail(bool b)
    {
        if (b)
        {
            currentTrail = Instantiate(trail, transform.position, transform.rotation, transform);
            currentTrail.SetActive(true);
        }
        else
        {
            currentTrail.transform.parent = null;
            Destroy(currentTrail, 2f);
        }
    }

    public void SetHold(bool b, Portal p)
    {
        gameObject.SetActive(!b);
        if (b)
        {
            holdPosition = p.transform.InverseTransformPoint(transform.position);
            holdDirection = p.transform.InverseTransformDirection(transform.forward);
            holdVelocity = rb.velocity;
        }
        else
        {
            transform.position = p.transform.TransformPoint(holdPosition);
            transform.rotation = Quaternion.LookRotation(p.transform.TransformDirection(holdDirection));
            rb.velocity = holdVelocity;
        }
    }

    public void SetSlice(bool b)
    {
        sliced = b;
        if(b && !cloneShowedInAnimation)
            clone.gameObject.SetActive(true);
        else
            clone.gameObject.SetActive(false);
        render.materials[0].SetFloat("_Slice", b ? 1f : 0f);
    }

    public void SetPos(Vector3 pos, Vector3 dir)
    {
        render.materials[0].SetVector("_Pos", pos);
        render.materials[0].SetVector("_Dir", dir);
    }

    public void SetClone(Portal p, Portal n)
    {
        if (!n.gameObject.activeSelf)
        {
            if (clone.gameObject.activeSelf)
            {
                clone.gameObject.SetActive(false);
            }
        }
        else
        {
            if (!clone.gameObject.activeSelf && !cloneShowedInAnimation)
            {
                clone.gameObject.SetActive(true);
            }

            Vector3 v = p.transform.InverseTransformPoint(transform.position);
            Vector3 f = p.transform.InverseTransformDirection(transform.forward);
            clone.transform.position = n.PortalInverse.TransformPoint(v);
            clone.transform.rotation = Quaternion.LookRotation(n.PortalInverse.TransformDirection(f));
            clone.materials[0].SetVector("_Pos", n.transform.position);
            clone.materials[0].SetVector("_Dir", sliceDirSameAsPortal ? n.transform.up : -n.transform.up);
        }
    }


    public void AddNotifyOnDeath(Portal p)
    {
        if (!insideOf.Contains(p))
        {
            insideOf.Add(p);
            inRangeOf++;
        }
    }

    public void SubNotifyOnDeath(Portal p)
    {
        if (insideOf.Contains(p))
        {
            insideOf.Remove(p);
            inRangeOf--;
        }

        if (sliced)
        {
            SetSlice(false);
        }
    }

    private void OnDestroy()
    {
        foreach (Portal p in insideOf)
        {
            p.RemoveFromList(this);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        clone = Instantiate(render.gameObject).GetComponent<Renderer>();
        clone.gameObject.SetActive(false);
        Trail(true);
    }

    public void NextFramePositionCorrection() 
    {
        lastVelocity = rb.velocity * Time.deltaTime;
        positionAfterTeleport = transform.position + lastVelocity;
        correctingPosition = true;
    }


    public void TelerportFrame(Portal p, Portal n, bool isFront)
    {
        isFrontPortalToIgnoreRenter = !isFront;
        timeRenter = Time.time + 0.2f;
        portalToIgnoreRenter = p;
        teleportedLastFrame = true;
        SetClone(p, n);
    }

    public void CheckForExit(Portal p, Portal n)
    {
        if (currentUpdateIndex == 0)
        {
            showThisFrame = false;
            hideThisFrame = false;
        }

        if (correctingPosition)
        {
            correctingPosition = false;
            transform.position = positionAfterTeleport;
        }
        
        lastVelocity = rb.velocity * Time.deltaTime;

        if (portalToExitNextStep == null)
        {
            Vector3 v;
            bool intersected = LinePlaneIntersection(out v, transform.position, lastVelocity.normalized, p.transform.up, p.transform.position);
            float dist = Vector3.Distance(transform.position, v);

            if (cldr.bounds.Intersects(p.Collider.bounds))
            {
                if (!lockSliceDir)
                {
                    sliceDirSameAsPortal = Vector3.Dot(transform.position - p.transform.position, p.transform.up) > 0;
                    lockSliceDir = true;
                }
                showThisFrame = true;
                SetPos(p.transform.position, sliceDirSameAsPortal ? p.transform.up : -p.transform.up);
                SetClone(p, n);
            }
            else
            {
                hideThisFrame = true;
                lockSliceDir = false;
            }

            if (lastVelocity.magnitude > dist)
            {
                portalToExitNextStep = p;
                if (teleportedLastFrame)
                    NextFramePositionCorrection();
            }

            if (teleportedLastFrame)
                teleportedLastFrame = false;
        }
        else if(portalToExitNextStep == p)
        {
            portalToExitNextStep.ExitedObject(this);
            portalToExitNextStep = null;
        }

        currentUpdateIndex++;

        if (currentUpdateIndex == inRangeOf)
        {
            if (showThisFrame)
            {
                if (!sliced)
                    SetSlice(true);
            }
            else if (hideThisFrame)
            {
                if (sliced)
                {
                    SetSlice(false);
                    cloneShowedInAnimation = false;
                }
            }
            currentUpdateIndex = 0;
        }
    }

    public void CheckForTp(Portal p, Portal n)
    {
        if (currentUpdateIndex == 0)
        {
            showThisFrame = false;
            hideThisFrame = false;
        }

        lastVelocity = rb.velocity * Time.deltaTime;

        if (portalToTpNextStep == null)
        {
            Vector3 v;
            bool intersected = LinePlaneIntersection(out v, transform.position, lastVelocity.normalized, p.transform.up, p.transform.position);
            float dist = Vector3.Distance(transform.position, v);
            if(cldr.bounds.Intersects(p.Collider.bounds))
            {
                if (!lockSliceDir)
                {
                    sliceDirSameAsPortal = Vector3.Dot(transform.position - p.transform.position, p.transform.up) > 0;
                    lockSliceDir = true;
                }
                showThisFrame = true;
                SetPos(p.transform.position, sliceDirSameAsPortal ? p.transform.up : -p.transform.up);
                SetClone(p, n);
            }
            else
            {
                hideThisFrame = true;
                lockSliceDir = false;
            }

            bool isFront = Vector3.Dot(rb.velocity.normalized, p.transform.up) < 0; ;

            if (isFront != isFrontPortalToIgnoreRenter || (isFront == isFrontPortalToIgnoreRenter && timeRenter < Time.time && portalToIgnoreRenter == p))
            {
                if (lastVelocity.magnitude > dist)
                {
                    if (intersected)
                    {
                        if (Vector3.Distance(v, p.transform.position) <= p.Radius)
                        {
                            portalToTpNextStep = p;
                            isFrontPortalToTpNextStep = isFront;
                        }   
                    }
                }
            }
        }
        else if(portalToTpNextStep == p)
        {
            portalToTpNextStep.DetectObject(this, isFrontPortalToTpNextStep);
            portalToTpNextStep = null;
        }

        currentUpdateIndex++;

        if (currentUpdateIndex == inRangeOf)
        {
            if (showThisFrame)
            {
                if (!sliced)
                    SetSlice(true);
            }
            else if (hideThisFrame)
            {
                if (sliced)
                {
                    SetSlice(false);
                    cloneShowedInAnimation = false;
                }
            }
            currentUpdateIndex = 0;
        }
    }

    public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
    {
        float length;
        float dotNumerator;
        float dotDenominator;
        Vector3 vector;
        intersection = Vector3.zero;

        dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
        dotDenominator = Vector3.Dot(lineVec, planeNormal);

        if (dotDenominator != 0.0f)
        {
            length = dotNumerator / dotDenominator;
            vector = lineVec.normalized * length;
            intersection = linePoint + vector;
            return true;
        }
        else
            return false;
    }
}
