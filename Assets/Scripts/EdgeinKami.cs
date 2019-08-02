using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeinKami : WeaponShoot {
	public GameObject suriken;
	//rango para la katana
	public float rangeKatana = 5f;
	//tiempo de stun
	public float stunTime;
	//tiempo que se está en estado de  ulti
	public float timeUlt;
	public bool ultUp;
	public int damageUlt;
	public ParticleSystem ultiParticleSystem;

	public void CallShoot(){
		// Si no ha pasado el tiempo de delay, salgo de la función.
		if (Time.time < nextTimeToShoot) {
			return;
		}

		// Calculamos el siguiente tiempo de delay al final del método.
		nextTimeToShoot = Time.time + shootDelay;
		
		int flipCoin = Random.Range (0,2);
		//aleatoriedad de animacion para los katanazos
		if (flipCoin == 1) {
			animator.SetTrigger ("ShootKatana1");

		} else {
			animator.SetTrigger ("ShootKatana2");

		}
	}

	/// <summary>
	/// Metodo que lanza los suriken si se pulsa boton derecho
	/// </summary>
	public void Hability(){
		animator.SetTrigger ("ShootSuriken");
	}

	public void CallShootEdgeinKami(){
		
		// Para almacenar la información de si el raycast ha golpeado algún objeto.
		bool impact = false;

		// Variable raycasthit para almacenar la información del impacto.
		RaycastHit hitInfo;

		impact = Physics.Raycast (fpsCamera.transform.position, 
			fpsCamera.transform.forward,
			out hitInfo,
			rangeKatana,
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
			if (hitInfo.collider.tag == "Enemy" && !ultUp) {
				hitInfo.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damage);
				player.GetComponent<FPSControler> ().TakeAmountForUlt (amountFoUlt);
			}
			if (hitInfo.transform.GetComponent<EnemyBehaviour> () != null && ultUp) {

				hitInfo.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damageUlt);
				hitInfo.transform.GetComponent<EnemyBehaviour> ().timeStunned = stunTime;
				hitInfo.transform.GetComponent<EnemyBehaviour> ().state = EnemyBehaviour.States.Stunned;

	

			}
		


		}



	}

	/// <summary>
	/// Calls the shoot edgein kami suriken.
	/// </summary>
	public void CallShootEdgeinKamiSuriken(){
		
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
	/// Quitamos la animacion de suriken
	/// </summary>
	/// <returns><c>true</c> if this instance cancel animation; otherwise, <c>false</c>.</returns>
	public void CancelAnimation(){
		animator.SetBool ("ShootSuriken",false);
	}

	///activamos el suriken de mentira
	public void ActiveSuriken(){
		suriken.SetActive (true);
	}
	//desactivamos el suriken de mentira
	public void DesactiveSuriken(){
		suriken.SetActive (false);
	}

	/// <summary>
	/// activamos la ultimate
	/// </summary>
	public void Ulti(){
		StartCoroutine(EdgeinKamiUlti());
	}

	/// <summary>
	/// Controla la ultimate de edgein kami
	/// </summary>
	/// <returns>The bold edgein kami.</returns>
	IEnumerator EdgeinKamiUlti (){
		ultUp = true;
		ultiParticleSystem.gameObject.SetActive (true);
		yield return new WaitForSeconds (timeUlt);
		ultiParticleSystem.gameObject.SetActive (false);
		ultUp = false;
		yield return null;
	}

}
