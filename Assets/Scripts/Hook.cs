using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour {
	public GameObject hook;
	public Camera fpsCamera;
	public float range = 100f;
	public LayerMask shootableLayer;
	public Vector3 destinyForPlayer;
	public float speed;
	public bool blockFPSController;
	public float distanceHitForHook;
	public float distancePlayerHook;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (1)) {
			CallHook ();

		}
		if (!blockFPSController) {
			DisableFPSController ();
			MovePlayer (destinyForPlayer);
		} else {
			OnableFPSController ();
		}
	}

	public void CallHook(){
		// Para almacenar la información de si el raycast ha golpeado algún objeto.
		bool impact = false;


		// Variable raycasthit para almacenar la información del impacto.
		RaycastHit hitInfo;

		impact = Physics.Raycast (fpsCamera.transform.position, 
			fpsCamera.transform.forward,
			out hitInfo,
			range,
			shootableLayer);

		if (impact) {
			Debug.Log (hitInfo.transform.name+"hook");
			GameObject hookShot = Instantiate (hook, transform.position, Quaternion.identity);
			Vector3 hookPoint = hitInfo.point + hitInfo.normal * distanceHitForHook;
			destinyForPlayer = hookPoint + hitInfo.normal * distancePlayerHook;
			hookShot.GetComponent<HookShot> ().destiny = hookPoint;
			Debug.Log(hitInfo.point + " " + hitInfo.normal);
			Destroy (hookShot,2f);
		}

	}

	public void MovePlayer(Vector3 position){
		transform.position = Vector3.MoveTowards (transform.position, position, speed);
		if (transform.position == position) {
			blockFPSController = true;
		}

	}
	public void DisableFPSController(){
		transform.GetComponent<FPSControler> ().blockHook = false;
	}
	public void OnableFPSController(){
		transform.GetComponent<FPSControler> ().blockHook = true;
	}
}
