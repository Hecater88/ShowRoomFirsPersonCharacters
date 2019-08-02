using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RougueFlame : WeaponShoot {
	public float velocityAntigravityUpUlt;
	public float maxYantigravity;
	public float timeUlt;
	public bool activeLanza;
	public float impactRadiusUlt;
	public GameObject lanzaPrefab;
	public GameObject lanzaAnimation;
	public GameObject rifle;
	public Transform lanzaRespawn;
	public float ultDelayDamage = 0.5f;
	public int damageUlt;
	public bool drawGizmo;
	public ParticleSystem fire;



	public void CallShoot(){
		if(areInUlti){
			return;
		}
		// Si no ha pasado el tiempo de delay, salgo de la función.
		if (Time.time < nextTimeToShoot) {
			return;
		}

		// Calculamos el siguiente tiempo de delay al final del método.
		nextTimeToShoot = Time.time + shootDelay;

		if (magazine <= 0) {
			// Reproducimos el sonido de que no quedan balas.
			audiosource.PlayOneShot (noAmmoSound);
			return;
		}

		// En cuanto se inicia el disparo aplico el retroceso
		player.AddRecoil (recoil);
		//animator.SetTrigger ("Shoot");
		// Efecto de partículas del arma.
		gunShoot.Play();

		// Efecto de sonido.
		audiosource.Play();


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
			if (hitInfo.collider.tag == "Enemy") {
				hitInfo.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damage);
				player.GetComponent<FPSControler> ().TakeAmountForUlt (amountFoUlt);

				
			}

		}

		// Tras el disparo, reducimos en 1 la munición. 
		magazine--;
		// Actualizo la munición en el HUD.
		UpdateAmmoDisplay ();
		//GenerateSound ();
		// Generamos el ruido.
		//if (hitInfo.transform.GetComponent<EnemyBehaviour> ().stun == false) {
		//GenerateSound ();
		//}
	}

	/// <summary>
	/// Activa la ultimate
	/// </summary>
	public void Ulti(){
		//activamos la booleana que controla si estamos en estado ultimate
		areInUlti = true;
		//activamos la animacion de ulti
		animator.SetTrigger ("CastUlti");
	}

	/// <summary>
	/// Casteo de la ulti
	/// </summary>
	public void CastUlti(){
		
		StartCoroutine(UltimateRogueFlame ());
	}

	/// <summary>
	/// Activa la antigravedad para que pueda volar el jugador
	/// </summary>
	/// <returns>The ult.</returns>
	IEnumerator AntigravityUlt(){
		//activamos la animacion
		animator.SetBool ("Ulti", true);
			//desactivamos la gravedad
			player.GetComponent<FPSControler>().ultiWithoutGravity = true;
			//mientras el player no llegue hasta el max
			while (player.transform.position.y < maxYantigravity) {
				//subimos al player en y
				Vector3 destination = new Vector3(player.transform.position.x, player.transform.position.y +(velocityAntigravityUpUlt*Time.deltaTime),player.transform.position.z);
				player.transform.position = destination;
				yield return null;
			}
		//activamos la animacion de la lanza
		animator.SetTrigger ("Lanza");
		//Activamos el boton derecho para poder lanzar la lanza.
		player.GetComponent<FPSControler> ().haveSpecial = true;
			
	}
	/// <summary>
	/// Gestiona la ultima de RogueFlame
	/// </summary>
	/// <returns>The rogue flame.</returns>
	IEnumerator UltimateRogueFlame(){
		//activamos la particula de sistem
			fire.gameObject.SetActive (true);
		//activamos la antigravedad
			StartCoroutine(AntigravityUlt ());
		//activamos el area de daño
			StartCoroutine(AreaFireRogueFlame ());
		//despues de que pase el timepo
			yield return new WaitForSeconds (timeUlt);
			
		//lo desactivamos todo
			fire.gameObject.SetActive (false);
			lanzaAnimation.SetActive (false);
			animator.SetBool ("Ulti", false);
			yield return new WaitForSeconds (0.5f);
			player.GetComponent<FPSControler> ().haveSpecial = false;
				//activamos el rifle, que anteriormente se desactivó con una animacion
			rifle.SetActive (true);


			player.GetComponent<FPSControler>().ultiWithoutGravity = false;
			areInUlti = false;

	}

	/// <summary>
	/// Activa el area de daño de la ultimate
	/// </summary>
	/// <returns>The fire rogue flame.</returns>
	IEnumerator AreaFireRogueFlame(){
		while(areInUlti){

			Collider[] colls = Physics.OverlapSphere(player.transform.position, impactRadiusUlt);

			foreach (Collider col in colls) {

				EnemyBehaviour enemyBehaviour = col.GetComponent<EnemyBehaviour> ();
				//verificamos que el objeto impactado no sea el mismo, para que no se autostunee
				if (enemyBehaviour != null) {
					//indicamos a todas las instancias del jugador impactado, que pase a estar stuneado
					col.transform.GetComponent<EnemyBehaviour> ().TakeDamage ( damageUlt);
					yield return new WaitForSeconds (ultDelayDamage);
				}
			}
			yield return null;
		}
	}

	/// <summary>
	/// Activa la lanza de ultimate
	/// </summary>
	public void LanzaRougueFlame (){
		//activamos la animacion de lanzar la lanza
		animator.SetTrigger ("ThrowLanza");
		//intancimamos la lanza
		GameObject lanza = Instantiate (lanzaPrefab,
							lanzaRespawn.position,
							Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z));
		
		Destroy (lanza, 3f);
	}

	/// <summary>
	///Metodo que activamo el lanzamiento de la lanza con el boton derecho	
	/// </summary>
	public void Hability(){
		LanzaRougueFlame ();
		player.GetComponent<FPSControler> ().haveSpecial = false;
	}

	/// <summary>
	/// Activa el modo lanza
	/// </summary>
	public void ActiveLanzaAnimation(){
		lanzaAnimation.SetActive (true);

	}

	/// <summary>
	/// Desactiva el modo lanza
	/// </summary>
	public void DesactiveLanzaAnimation(){
		lanzaAnimation.SetActive (false);

	}
	/// <summary>
	/// Desactivos the rifle.
	/// </summary>
	public void DesactivoRifle(){
		rifle.SetActive (false);
	}
	/// <summary>
	/// Activos the rifle.
	/// </summary>
	public void ActivoRifle(){
		rifle.SetActive (true);

	}

	void OnDrawGizmos(){
		//para mostrar el área de efecto de la habilidad
		if(drawGizmo){
		Gizmos.DrawSphere (player.transform.position, impactRadiusUlt);
		}
	}
}


