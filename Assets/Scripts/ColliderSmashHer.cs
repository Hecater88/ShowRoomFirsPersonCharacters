using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSmashHer : MonoBehaviour {
	//Desactiva los colliders d elos enemigos cuando SmashHer pasa con su ultimate de placaje
	void OnTriggerEnter(Collider other){
		if(other.CompareTag("Enemy")){
			Debug.Log ("EntraTrigger");
			other.gameObject.GetComponent<CapsuleCollider> ().enabled = false;
		}
	}
}
