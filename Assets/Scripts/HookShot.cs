using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookShot : MonoBehaviour {
	public Vector3 destiny;
	public float speed;
	public GameObject fpsController;
	public bool movePlayer = true;


	// Use this for initialization
	void Start () {
		fpsController = GameObject.FindGameObjectWithTag ("Player");

	}
	void Update(){
		MoveHook ();
	}
	/// <summary>
	/// Mueve el gancho a la direccion indicada por el raycast 
	/// </summary>
	public void MoveHook(){
		transform.position = Vector3.MoveTowards (transform.position,destiny,speed);
		if (transform.position == destiny) {
			fpsController.GetComponent<FPSControler> ().blockHook = false;
			Destroy (this);

		}

	}
		
		
}
