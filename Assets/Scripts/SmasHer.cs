using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmasHer :WeaponShoot{
	//velocidad del placaje
	public float velocityTackel;
	//booleana que ocntrola si esta en estado de placaje
	public bool isTackle;

	public GameObject colliderBox;
	//rango de collision
	public float rangeCollision = 3f;
	//radio para el overlap sphere del placaje
	public float  impactRadiusUlt;
	//radio para el overlap sphere del earthquake
	public float  impactRadiusQueake;
	//daño del placaje
	public int damageTackle;
	//botones de la ulti
	public GameObject buttonsUlti;
	//booleana para controlar si esta en estado de ultimate
	public bool ultUp;
	//tiempo de la ulti
	public float timeUlt;
	//daño del terremoto
	public int damageEarthQuake;
	//tiempo de stun
	public float stunTime;
	public bool drawGizmo;
	//sistema de particulas para el terremoto
	public ParticleSystem earthQuake;




	public void CallShoot(){
		// Si no ha pasado el tiempo de delay, salgo de la función.
		if (Time.time < nextTimeToShoot) {
			return;
		}

		// Calculamos el siguiente tiempo de delay al final del método.
		nextTimeToShoot = Time.time + shootDelay;

		//aleatoriedad de animacion
		int flipCoin = Random.Range (0,2);

		if (flipCoin == 1) {
			animator.SetTrigger ("Shoot1");

		} else {
			animator.SetTrigger ("Shoot2");

		}
	}

	/// <summary>
	///	Lanzamiento de puñetazos 
	/// </summary>

	public void CallShootSmashHer(){


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
			Debug.Log (hitInfo.transform.name);

			// Instancio la particula en el punto de impacto orientada hacia la normal de 
			// la superficie impactada.
			Instantiate (impactParticle, 
				hitInfo.point, 
				// La normal va a ser siempre perpendicular al punto de impacto.
				Quaternion.LookRotation (hitInfo.normal));

			// Instanciamos el decal del impacto en la superficie separándola de la normal
			// 0.01 unidades, para asegurarnos que sea visible correctamente
			// además el quad instanciado lo invertimos para que aparezca en la dirección correcta.
			GameObject decalTemp = Instantiate (impactDecal,
				hitInfo.point + hitInfo.normal * 0.01f,
				Quaternion.LookRotation (-hitInfo.normal));

			// Para rotar el disparo.
			decalTemp.transform.Rotate (new Vector3 (0f,
				0f,
				Random.Range (0f, 360f)));

			// Emparentamos al decal, con el objeto impactado para que se mueva con el.	
			decalTemp.transform.parent = hitInfo.transform;

			// Programamos su destrucción.
			Destroy(decalTemp, decalLifetime);

			// Verificamos si el componente impactado tiene un rigidbody.
			if (hitInfo.rigidbody != null) {
				// Si tiene un rigidbody, le aplicacmos la fuerza definida en la dirección en la que se ha ejecutado el disparo.
				hitInfo.rigidbody.AddForce (fpsCamera.transform.forward * impactForce, ForceMode.Impulse);
			}
			if (hitInfo.collider.tag == "Enemy") {
				hitInfo.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damage);
				player.GetComponent<FPSControler> ().TakeAmountForUlt (amountFoUlt);
			}



		}



	}

	/// <summary>
	/// Activamos la ultimate
	/// </summary>
	public void Ulti(){
		AreinUlt();
	}

	/// <summary>
	/// Corrutina para activar el placaje
	/// </summary>
	IEnumerator Tackle(){
		//desactivamos los botones guia
		buttonsUlti.SetActive (false);
		//paramos el metodo que controla la ultimate, porque parará cuando se choque
		StopCoroutine (SmashHerdUlti ());
		player.GetComponent<FPSControler> ().areInUlt = false;
		//mientras esta en estado de placaje
			while (isTackle) {
				StartCoroutine (TackleForEnemy ());
				// Para controlar si hay un muro delante
				RaycastHit hit;
				Vector3 destination = player.transform.position + player.transform.forward * velocityTackel;
				//si hay un muro delante, para qu eno lo traspase
				if (Physics.Linecast (transform.position, player.transform.position + player.transform.forward * range, out hit)) {
				//desactivamos todo lo que tenga que ver con la ultimate
					animator.SetBool ("isTackle", false);
					animator.SetBool ("Ulti", false);
					ultUp = false;
					isTackle = false;
					areInUlti = false;
					
					//y paramos al jugador en su destino
					destination = player.transform.position + player.transform.forward * (hit.distance - 1f);

				}
				
				//si no hay un muro delante, deplazamos al jugador
				if (Physics.Raycast (destination, -Vector3.up, out hit)) {
					destination = hit.point;
					destination.y = 1f;
					player.transform.position = destination;
					

				}

			yield return new WaitForSeconds (0.005f);

			}


	}

	/// <summary>
	/// Corrrutina que controla si ha chocado un enemigo en estado de placaje	/// </summary>
	/// <returns>The for enemy.</returns>
	IEnumerator TackleForEnemy (){

			///Creamos un OverlapSphere para que detecte las colisiones
			Collider[] colls = Physics.OverlapSphere(colliderBox.transform.position, impactRadiusUlt);

			foreach (Collider col in colls) {

				EnemyBehaviour enemyBehaviour = col.GetComponent<EnemyBehaviour> ();
				//verificamos que el objeto impactado no sea el mismo, para que no se autostunee
				if (enemyBehaviour != null) {
					//indicamos a todas las instancias del jugador impactado, que pase desactive le collider y reciba el daño correspondiente
					col.transform.GetComponent<EnemyBehaviour>().DesactiveCapsuleCollider();
					col.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damageTackle);
				}
			}
			yield return null;



	}

	/// <summary>
	/// Corrrutina que controla si ha chocado un enemigo en estado de earthQuake
	/// </summary>
	/// <returns>The quake.</returns>
	IEnumerator EarthQuake(){
		//activamos el sistema de particulas
		earthQuake.Play ();
		//deasactivamos los botones guia
		buttonsUlti.SetActive (false);
		StopCoroutine (SmashHerdUlti ());
		player.GetComponent<FPSControler> ().areInUlt = false;

			Collider[] colls = Physics.OverlapSphere(colliderBox.transform.position, impactRadiusQueake);

			foreach (Collider col in colls) {

				EnemyBehaviour enemyBehaviour = col.GetComponent<EnemyBehaviour> ();
				//verificamos que el objeto impactado no sea el mismo, para que no se autostunee
				if (enemyBehaviour != null) {
					//indicamos a todas las instancias del jugador impactado, que pase a estar stuneado
					//col.transform.GetComponent<EnemyBehaviour> ().timeStunned = stunTime;
					//col.transform.GetComponent<EnemyBehaviour> ().state = EnemyBehaviour.States.Stunned;
					
					col.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damageEarthQuake);
					col.transform.GetComponent<EnemyBehaviour> ().timeStunned = stunTime;
					col.transform.GetComponent<EnemyBehaviour> ().state = EnemyBehaviour.States.Stunned;
				}
			}
		animator.SetBool ("Ulti", false);
		areInUlti = false;

		yield return null;

		

	}

	/// <summary>
	/// Metodo que controla todo el estado de ultimate
	/// </summary>
	/// <returns>The herd ulti.</returns>
	IEnumerator SmashHerdUlti (){
		float time = timeUlt;
		areInUlti = true;
		player.GetComponent<FPSControler> ().areInUlt = true;
		animator.SetBool ("Ulti", true);
		Debug.Log ("UltiREady");
		buttonsUlti.SetActive (true);
		//cuando haya pasado el timepo, lo desactivamos todo
		yield return new WaitForSeconds (time);
		areInUlti = false;
		ultUp = false;
		buttonsUlti.SetActive (false);
		Debug.Log("UltiNOTREady");
		animator.SetBool ("Ulti", false);
		yield break;
			
	}

	/// <summary>
	/// Voleana que activa la ultimate
	/// </summary>
	public void AreinUlt(){
		ultUp = true;
		StartCoroutine ( SmashHerdUlti ());
	}

	/// <summary>
	/// activamos la booleana de fpscontroler para que indicar que estamos en modo ultimate
	/// </summary>
	public void ReadyForUlt(){
		player.GetComponent<FPSControler> ().areInUlt = true;
	}
	/// <summary>
	/// para que el EarthQUake se ative con boton izquierdo
	/// </summary>
	public void Hability1(){
		animator.SetTrigger ("EarthQuake");

	}
	/// <summary>
	/// para que el placaje se ative con boton derecho
	/// </summary>
	public void Hability2(){
		animator.SetBool ("isTackle", true);
		isTackle = true;
		StartCoroutine ( Tackle());
	}
	/// <summary>
	/// ponemos la booleana que ocntrola si estamos en estado de ulti para el script WaponShooter
	/// </summary>
	public void onDisableUlt(){
		ultUp = false;
	}

	/// <summary>
	/// EActiva la Corrutina de terremoto
	/// </summary>
	public void EarthQueakeForAnimation(){
		StartCoroutine ( EarthQuake());
	}
	void OnDrawGizmos(){
		//para mostrar el área de efecto de la habilidad
		if (drawGizmo) {
			Gizmos.DrawSphere (colliderBox.transform.position, impactRadiusQueake);
		}

	}


}
