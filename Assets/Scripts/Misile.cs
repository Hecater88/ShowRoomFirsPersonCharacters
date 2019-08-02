using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misile : MonoBehaviour {
	public float impactRadius;
	public int damage;
	public int damageForPlayer;

	public Transform position;
	public float speed = 5f;
	private Rigidbody rb;
	public ParticleSystem fireMisile;
	public GameObject explosionMisile;
	public bool exploted = false;
	public GameObject player;
	// Use this for initialization
	void Start () {
		rb = GetComponent <Rigidbody> ();
		fireMisile.gameObject.SetActive (true);
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		MoveMisile ();
	}

	public void MoveMisile(){
		if(exploted){
			return;
		}
		rb.velocity = transform.forward * speed;
	}

	void OnTriggerEnter(Collider other){
		Explosion ();

	}
	/// <summary>
	/// metodo que simula la explosion del misil
	/// </summary>
	public void Explosion(){
		exploted = true;
		MeshRenderer[] children = GetComponentsInChildren<MeshRenderer> ();

		foreach (MeshRenderer child in children) {
			child.enabled = false;
		}

		GameObject explosion = Instantiate (explosionMisile, transform.position, Quaternion.identity);
		Collider[] colls = Physics.OverlapSphere(transform.position, impactRadius);

		foreach (Collider col in colls) {

			EnemyBehaviour enemyBehaviour = col.GetComponent<EnemyBehaviour> ();

			//verificamos que el objeto impactado no sea el mismo, para que no se autostunee
			if (enemyBehaviour != null ) {
				//indicamos a todas las instancias del jugador impactado, que pase a estar stuneado
				col.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damage);


				
			}
			FPSControler playerControler = col.GetComponent < FPSControler > ();

			if (playerControler != null ) {
				//indicamos a todas las instancias del jugador impactado, que pase a estar stuneado
				col.transform.GetComponent<FPSControler> ().TakeDamage (damageForPlayer);
			}

		}
		Destroy (explosion,1f);
		Destroy (gameObject);
	}







}
