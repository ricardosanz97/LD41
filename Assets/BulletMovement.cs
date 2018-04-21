﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour {
    private Rigidbody rb;
    // Use this for initialization
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void bulletMovement(Vector3 direction, float force)
    {
        rb.AddForce(direction * force);
    }
    

}
