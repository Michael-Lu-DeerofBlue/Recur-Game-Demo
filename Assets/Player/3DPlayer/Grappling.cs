using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class Grappling : MonoBehaviour
{
    
    [Header("References")]
    private PlayerController pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    public bool hitPoint;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    public bool grappling;
    public Flowchart flowchart;

    private void Start()
    {
        pm = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (grappling && hitPoint)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            hitPoint = true;
            grapplePoint = hit.point;
            lr.SetPosition(1, grapplePoint);
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
            lr.enabled = true;
        }

    }

    private void ExecuteGrapple()
    { 
        PlayEffectLaunch();
        Vector3 forceDirection = grapplePoint - pm.transform.position;
        pm.InAir(true);
        pm.isMagneticBootsOn = false;
        pm.UpdateUI();
        pm.GetComponent<Rigidbody>().AddForce(forceDirection*80);
        Invoke(nameof(StopGrapple), 1f);
        PlayEffectAttach();
    }

    public void StopGrapple()
    {
        grapplingCdTimer = grapplingCd;
        hitPoint = false;
        lr.enabled = false;
        PlayEffectDettach();
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    void PlayEffectLaunch()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart
            flowchart.ExecuteBlock("HookLaunch");
        }
    }

    void PlayEffectAttach()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart
            flowchart.ExecuteBlock("HookAttachDrag");
        }
    }

    void PlayEffectDettach()
    {
        // Check if the flowchart is not already executing
        if (!flowchart.HasExecutingBlocks())
        {
            // Start the Fungus flowchart
            flowchart.ExecuteBlock("HookDettach");
        }
    }
}
