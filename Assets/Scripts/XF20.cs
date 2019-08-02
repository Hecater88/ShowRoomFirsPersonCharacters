using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XF20 : WeaponShoot {
	public GameObject misilePrefab;
	public Transform misileRespawn;
	public ParticleSystem gunShoot2;
	//como tiene dos armas
	public float recoil2;
	public float timeUlt;
	public bool ultUp;
	public GameObject rifle;
	public GameObject uzi;
	public GameObject bazooka;
	public float nextTimeToShoot2;
	public float shootDelay2;
	public int damage2;
	public float nextTimeToShootBazooka;
	public float shootDelayBazooka;

	/// <summary>
	/// CallShoot para la uzi
	/// </summary>
	public void CallShoot ()
	{
		if (ultUp) {
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
		animator.SetTrigger ("Shoot");
		// Efecto de partículas del arma.
		gunShoot.Play ();

		// Efecto de sonido.
		audiosource.Play ();


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
			Destroy (decalTemp, decalLifetime);

			// Verificamos si el componente impactado tiene un rigidbody.
			if (hitInfo.rigidbody != null) {
				// Si tiene un rigidbody, le aplicacmos la fuerza definida en la dirección en la que se ha ejecutado el disparo.
				hitInfo.rigidbody.AddForce (fpsCamera.transform.forward * impactForce, ForceMode.Impulse);
			}
			if (hitInfo.collider.tag == "Enemy") {
				hitInfo.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damage);
				player.GetComponent<FPSControler> ().TakeAmountForUlt (amountFoUlt);
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

	}
	/// <summary>
	/// CallShoot para la riflecon el boton derecho
	/// </summary>
	public void Hability(){
		if (ultUp) {
			return;
		}
		// Si no ha pasado el tiempo de delay, salgo de la función.
		if (Time.time < nextTimeToShoot2) {
			return;
		}

		// Calculamos el siguiente tiempo de delay al final del método.
		nextTimeToShoot2 = Time.time + shootDelay2;

		if (magazine <= 0) {
			// Reproducimos el sonido de que no quedan balas.
			audiosource.PlayOneShot (noAmmoSound);
			return;
		}

		// En cuanto se inicia el disparo aplico el retroceso
		player.AddRecoil (recoil2);
		animator.SetTrigger ("Shoot2");
		// Efecto de partículas del arma.
		gunShoot2.Play ();

		// Efecto de sonido.
		audiosource.Play ();


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
			Destroy (decalTemp, decalLifetime);

			// Verificamos si el componente impactado tiene un rigidbody.
			if (hitInfo.rigidbody != null) {
				// Si tiene un rigidbody, le aplicacmos la fuerza definida en la dirección en la que se ha ejecutado el disparo.
				hitInfo.rigidbody.AddForce (fpsCamera.transform.forward * impactForce, ForceMode.Impulse);
			}
			if (hitInfo.collider.tag == "Enemy") {
				hitInfo.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damage2);
				player.GetComponent<FPSControler> ().TakeAmountForUlt (amountFoUlt);
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
	}

	/// <summary>
	/// Utilizamos este metodo para cuando se active la ultimate, podamos llamar el disparo con el boton izquierdo
	/// </summary>
	public void Hability1(){
		Ultimate ();
	}

	/// <summary>
	/// LLama al disparo de la ulti
	/// </summary>
	public void Ultimate(){
		// Si no ha pasado el tiempo de delay, salgo de la función.
		if (Time.time < nextTimeToShootBazooka) {
			return;
		}
		animator.SetTrigger ("ShootBazooka");
		// Calculamos el siguiente tiempo de delay al final del método.
		nextTimeToShootBazooka = Time.time + shootDelayBazooka;



		GameObject misile = Instantiate (misilePrefab,
			misileRespawn.position,
			Quaternion.Euler(fpsCamera.transform.rotation.eulerAngles.x,fpsCamera.transform.rotation.eulerAngles.y,fpsCamera.transform.rotation.eulerAngles.z));



		// En cuanto se inicia el disparo aplico el retroceso
		player.AddRecoil (recoil);

		// Generamos el ruido.
		GenerateSound ();
		audiosource.Play();
		Destroy (misile,2f);
	}

	/// <summary>
	/// Controla todo los componentes que participa en la ultimate	/// </summary>
	/// <returns>The f20 ulti.</returns>
	IEnumerator XF20Ulti (){
		animator.SetBool ("Ulti",true);
		ultUp = true;
		areInUlti = true;
		player.GetComponent<FPSControler> ().areInUlt = true;

		yield return new WaitForSeconds (timeUlt);
		areInUlti = false;
		animator.SetBool ("Ulti",false);
		player.GetComponent<FPSControler> ().areInUlt = false;
		ActiveWeapons ();
		DesActiveBazooka ();
		ultUp = false;
		yield return null;
	}

	/// <summary>
	/// Actives the bazooka.
	/// </summary>
	public void ActiveBazooka(){
		bazooka.SetActive (true);
	}
	/// <summary>
	/// DESs the active bazooka.
	/// </summary>
	public void DesActiveBazooka(){
		bazooka.SetActive (false);
	}
	/// <summary>
	/// desactiva weapons.
	/// </summary>
	public void DesActiveWeapons(){
		rifle.SetActive (false);
		uzi.SetActive (false);
	}
	/// <summary>
	/// Actives the weapons.
	/// </summary>
	public void ActiveWeapons(){
		rifle.SetActive (true);
		uzi.SetActive (true);
	}
	 /// <summary>
	 /// Activamos la ulti mediante una animacion
	/// </summary>
	public void Ulti(){
		animator.SetTrigger("CastUlti");
	}
	/// <summary>
	/// Metodo para la aniamcion
	/// </summary>
	public void ActiveUlti(){
		StartCoroutine (XF20Ulti ());
	}
}
