using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resurrector : MonoBehaviour {
	//transform para colocar el objeto
	public Transform checkPoint;
	//giro del raycast
	public float turnSpeed = 3f;
	//alcance de cura
	public float reach = 10f;
	//variable layermask para el raycast
	public LayerMask raycastLayer;	
	//contador de tiempo
	public float coolDownTimer;
	//delay de cura
	public float delay;
	//daño para el que no sea player
	public int damage = 10;
	//cantidad de cura
	public int heal = 10;
	//particula de sistemas para el objeto
	public ParticleSystem healthParticleSystem;
	// Use this for initialization

	void Start () {
		//inicializamos el contador
		coolDownTimer = delay;

	}
	
	// Update is called once per frame
	void Update () {
		//cuenta atras para el delay
		if (coolDownTimer > 0) {
			coolDownTimer -= Time.deltaTime;
		}
		//destruimos
		Destroy (this.gameObject, 10f);
	}
	void FixedUpdate(){
		CheckColliders ();
	}

	/// <summary>
	/// Area de cura
	/// </summary>
	public void CheckColliders(){
		//activamos el sistema de particulas
		healthParticleSystem.Play ();

		//giramos el transform 
		checkPoint.localEulerAngles = new Vector3 ( checkPoint.localEulerAngles.x, 
			checkPoint.localEulerAngles.y + turnSpeed * Time.deltaTime,
			checkPoint.localEulerAngles.z);
		
		//variable raycast
		RaycastHit hit = new RaycastHit ();

		//tamaño del area
		Vector3 targetView = (checkPoint.forward * reach + checkPoint.position);

		//activamos el linecat
		if (Physics.Linecast (checkPoint.position, targetView, out hit, raycastLayer)) {
			//si el contador es menor que cero
			if (coolDownTimer < 0) {
				//si el linecast ha chocado con un gameobject que tenga FPSControler
				if (hit.transform.GetComponent<FPSControler> () != null) {
					//lo curamos
					hit.transform.GetComponent<FPSControler> ().TakeLife (heal);
					//reiniciamos el contador
					coolDownTimer = delay;

				}
				//si el linecast ha chocado con el enemigo
				if (hit.collider.tag == "Enemy") {
					//le quitamos vida
					//hit.transform.GetComponent<EnemyBehaviour> ().TakeDamage(damage);
				}

			} 
				
			

		}


		Debug.DrawLine (checkPoint.position, targetView, Color.green);
	}

}
