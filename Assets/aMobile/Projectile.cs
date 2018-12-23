﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 mousePosition;    // target transform
    //public GameObject clickLight;
    public float launchAngle;
    float launchPower;

    Rigidbody rb;
    public Transform shooter;
    public Transform deathBall;
    public ParticleSystem _psystem;
    public PlayerSelect playerSelect;
    public LayerMask throwMask;


    private void Start()  // Basically Launch
    {
        playerSelect = shooter.parent.GetComponent<PlayerSelect>();
        //playerSelect = FindObjectOfType<PlayerSelect>();
        GetMousePosLook();
        GetLaunchAngle();
        AudioManager.instance.Play("Throw");
        Launch();
        //GameObject light = (GameObject)Instantiate(clickLight, new Vector3(mousePosition.x, 5+ mousePosition.y, mousePosition.z), Quaternion.Euler(90f,0f,0f));
        //Destroy(light, 1.5f);
    }
    public void GetLaunchAngle()
    {
        launchAngle = playerSelect.launchAngle;
    }

    private void GetMousePosLook() // THIS IS CORRECT!!!
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, throwMask))
            {
                mousePosition = hit.point;
                transform.LookAt(mousePosition);
            }
        }
        for (int i = 0; i < Input.touchCount; i++)
        {
            Ray touchRay = Camera.main.ScreenPointToRay(Input.touches[i].position);
            RaycastHit touchHit;
            if (Physics.Raycast(touchRay, out touchHit, Mathf.Infinity, throwMask))
            {
                mousePosition = touchHit.point;
                transform.LookAt(mousePosition);
            }
        }
    }
    private void Launch()
    {
        rb = GetComponent<Rigidbody>();
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.
        Vector3 projectileXZPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 targetXZPos = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        //print("this is R  " + R);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(launchAngle * Mathf.Deg2Rad);
        float H = mousePosition.y - transform.position.y;
        //print("Launch Angle at " + LaunchAngle);

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rb.velocity = globalVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        ContactPoint contact = collision.contacts[0];
        if (contact.otherCollider.GetComponent<Collider>().tag == "SoftGround")
        {
            AudioManager.instance.Play("Ground");
        }
        if (contact.otherCollider.GetComponent<Collider>().tag == "Enemy")
        {
            AudioManager.instance.Play("PlayerHit");
        }
        else
        {
            AudioManager.instance.Play("HardSurface");
        }
        Transform newDeathBall = Instantiate(deathBall, transform.position, transform.rotation, contact.otherCollider.transform) as Transform;
        ParticleSystem newBallDeathParticle = Instantiate(_psystem, transform.position, transform.rotation) as ParticleSystem;
    }
}