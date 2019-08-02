using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour {
	public float impactRadius;
	public int damage;
	public int damageForPlayer;
	public Transform position;
	public float force = 5f;
	public float forceUp;
	private Rigidbody rb;
	public GameObject explosionMisile;
	public bool exploted = false;
	private float countDown;
	public float delayForExplosion = 3f;
	public float amountFoUlt;
	public GameObject player;
	// Use this for initialization
	void Start () {
		countDown = delayForExplosion;
		rb = GetComponent <Rigidbody> ();
		rb.AddForce(transform.forward *force);
		rb.AddForce (transform.up*forceUp);
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		countDown -= Time.deltaTime;
		if(countDown < 0 && !exploted){
			Explosion ();
		}
	}

	void OnCollisionEnter(Collision other){
		if(other.collider.CompareTag("Enemy")){
			Explosion ();
		}
				
	}

	/// <summary>
	/// Explosion de la granda
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
				player.GetComponent<FPSControler> ().TakeAmountForUlt (amountFoUlt);
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

	void OnDrawGizmos(){
		//para mostrar el área de efecto de la habilidad

		Gizmos.DrawSphere (transform.position, impactRadius);

	}


}
