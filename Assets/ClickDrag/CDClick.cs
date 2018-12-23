﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDClick : MonoBehaviour {
    // ATTACH TO MAIN CAMERA (set tag to main camera)

    [SerializeField] private LayerMask clickablesLayer;

    private List<GameObject> selectedObjects;

    [HideInInspector] public List<GameObject> selectableObjects;
    private Vector3 mousePos1;
    private Vector3 mousePos2;

    void Awake()
    {
        selectedObjects = new List<GameObject>();
        selectableObjects = new List<GameObject>();
        ClearSelection();
    }

    void Update () {

        if (Input.GetMouseButtonUp(0))
        {
            ClearSelection();
        }

        if (Input.GetMouseButtonDown(0))
        {
            mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            RaycastHit rayHit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, clickablesLayer))
            {
                CDClickOn clickOnScript = rayHit.collider.GetComponent<CDClickOn>();
                if (Input.GetKey("left ctrl"))
                {
                    if (clickOnScript.currentlySelected == false)
                    {
                        selectedObjects.Add(rayHit.collider.gameObject);
                        clickOnScript.currentlySelected = true;
                        clickOnScript.ClickMe();
                    }
                    else
                    {
                        selectedObjects.Remove(rayHit.collider.gameObject);
                        clickOnScript.currentlySelected = false;
                        clickOnScript.ClickMe();
                    }
                }
                else
                {
                    ClearSelection();
                    selectedObjects.Add(rayHit.collider.gameObject);
                    clickOnScript.currentlySelected = true;
                    clickOnScript.ClickMe();
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && Input.GetKey("left shift"))
        {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            //todo make it harder to accidently deselect perhaps hold shift and click
            if(Vector3.Distance(mousePos1, mousePos2) > 0.02f)
            {
                SelectObjects();
            }
        }
	}

    void SelectObjects()
    {
        List<GameObject> remObjects = new List<GameObject>();

        if (Input.GetKey("left ctrl") == false)
        {
            ClearSelection();
        }

        Rect selectRect = new Rect(mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y);

        foreach (GameObject selectObject in selectableObjects)
        {
            if (selectObject != null)
            {
                 if (selectRect.Contains(Camera.main.WorldToViewportPoint(selectObject.transform.position), true))
                {
                    selectedObjects.Add(selectObject);
                    selectObject.GetComponent<CDClickOn>().currentlySelected = true;
                    selectObject.GetComponent<CDClickOn>().ClickMe();
                }
            }
            else
            {
                remObjects.Add(selectObject);

            }
        }

        if (remObjects.Count > 0)
        {
            foreach (GameObject rem in remObjects)
            {
                selectableObjects.Remove(rem);
            }

            remObjects.Clear();
        }
    }
    void ClearSelection()
    {
        if (selectedObjects.Count > 0)
        {
            foreach (GameObject obj in selectedObjects)
            {
                if (obj != null)
                {
                    //print(obj.GetComponent<CDClickOn>().currentlySelected);
                    obj.GetComponent<CDClickOn>().currentlySelected = false;
                    obj.GetComponent<CDClickOn>().ClickMe();
                }
            }
            selectedObjects.Clear();
        }
    }
}
