﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererHook : MonoBehaviour {
	public LineRenderer rope;
	public GameObject player;
	// Use this for initialization
	void Start () {
		rope = GetComponent<LineRenderer> ();
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		rope.SetPosition (0,transform.position);
		rope.SetPosition (1,player.transform.position);

	}
}