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
    private bool sliced = false;

    private Renderer clone;
    private TrailRenderer trail;

    public Rigidbody Rb { get => rb; }

    public void SetSlice(bool b)
    {
        sliced = b;
        clone.gameObject.SetActive(b);
        render.materials[0].SetFloat("_Slice", b ? 1f : 0f);
        //Debug.LogError(b);
    }

    public void SetPos(Vector3 pos, Vector3 dir)
    {
        render.materials[0].SetVector("_Pos", pos);
        render.materials[0].SetVector("_Dir", dir);
        //Debug.LogError(pos + "  " + dir);
    }

    public void SetClone(Portal p, Portal n)
    {
        Vector3 v = p.transform.InverseTransformPoint(transform.position);
        clone.transform.position = n.PortalInverse.TransformPoint(v);
        clone.transform.rotation = transform.rotation;
        clone.materials[0].SetVector("_Pos", n.transform.position);
        clone.materials[0].SetVector("_Dir", Vector3.Dot(clone.transform.position - n.transform.position, n.transform.up) > 0 ? -n.transform.up : n.transform.up);
    }

    int inRangeOf = 0;

    public void AddNotifyOnDeath(Portal p)
    {
        insideOf.Add(p);
        inRangeOf++;
    }

    public void SubNotifyOnDeath(Portal p)
    {
        insideOf.Remove(p);
        inRangeOf--;

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
        trail = GetComponent<TrailRenderer>();
    }

    bool correctingPosition = false;
    Vector3 positionAfterTeleport = Vector3.zero;

    public void NextFramePositionCorrection() 
    {
        lastVelocity = rb.velocity * Time.deltaTime;
        positionAfterTeleport = transform.position + lastVelocity;
        correctingPosition = true;
        Debug.LogError("Need correction");
    }

    bool teleportedLastFrame = false;

    public void TelerportFrame(Portal p, Portal n, bool isFront)
    {
        isFrontPortalToIgnoreRenter = !isFront;
        timeRenter = Time.time + 0.2f;
        portalToIgnoreRenter = p;
        teleportedLastFrame = true;
        SetClone(p, n);
        Debug.LogError("Got teleported");
    }

    bool showThisFrame = false;
    bool hideThisFrame = false;
    int currentUpdateIndex = 0;

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
        //Debug.DrawRay(transform.position, lastVelocity, Color.blue);
        //Debug.DrawRay(transform.position, rb.velocity, Color.blue);

        if (portalToExitNextStep == null)
        {
            Vector3 v;
            bool intersected = LinePlaneIntersection(out v, transform.position, lastVelocity.normalized, p.transform.up, p.transform.position);
            float dist = Vector3.Distance(transform.position, v) + 0.05f;

            if (cldr.bounds.Intersects(p.Collider.bounds))
            {
                showThisFrame = true;
                SetPos(p.transform.position, Vector3.Dot(transform.position - p.transform.position, p.transform.up) > 0 ? p.transform.up : -p.transform.up);
                SetClone(p, n);
            }
            else
            {
                hideThisFrame = true;
            }
            //Debug.LogError((dist < lastVelocity.magnitude) + " " + v +" " + p.name);

            if (lastVelocity.magnitude > dist)
            {
                portalToExitNextStep = p;
                if (teleportedLastFrame)
                    NextFramePositionCorrection();
                Debug.LogError("Next frame Exit " + teleportedLastFrame + "   " + lastVelocity.magnitude + "   " + dist);
            }

            if (teleportedLastFrame)
                teleportedLastFrame = false;
        }
        else if(portalToExitNextStep == p)
        {
            portalToExitNextStep.ExitedObject(this);
            portalToExitNextStep = null;
            Debug.LogError("Exit");
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
                    SetSlice(false);
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
        //Debug.DrawRay(transform.position, lastVelocity, Color.red);
        //Debug.DrawRay(transform.position, rb.velocity, Color.blue);

        if (portalToTpNextStep == null)
        {
            Vector3 v;
            bool intersected = LinePlaneIntersection(out v, transform.position, lastVelocity.normalized, p.transform.up, p.transform.position);
            float dist = Vector3.Distance(transform.position, v);

            //Debug.LogError((dist < lastVelocity.magnitude) + " " + v + " " + p.name);
            if(cldr.bounds.Intersects(p.Collider.bounds))
            {
                showThisFrame = true;
                SetPos(p.transform.position, Vector3.Dot(transform.position - p.transform.position, p.transform.up) > 0 ? p.transform.up : -p.transform.up);
                SetClone(p, n);
            }
            else
            {
                hideThisFrame = true;
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
                            //Debug.LogError(isFront + "   " + Vector3.Dot(rb.velocity.normalized, p.transform.up));
                            //Debug.DrawRay(transform.position, p.transform.up, Color.cyan);
                            //Debug.DrawLine(transform.position, rb.velocity.normalized, Color.yellow);
                            Debug.LogError(isFront);
                            portalToTpNextStep = p;
                            isFrontPortalToTpNextStep = isFront;
                            Debug.LogError("Next frame TP");
                        }   
                    }
                }
            }
        }
        else if(portalToTpNextStep == p)
        {
            Debug.LogError("TP");
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
                    SetSlice(false);
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

        //calculate the distance between the linePoint and the line-plane intersection point
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
