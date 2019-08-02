using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBold :WeaponShoot {
	public float stunTime;
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
				//si se ha  chocado con un collider que tenga el tag enemy
				if (hitInfo.collider.tag == "Enemy"&& !ultUp)  {
				//le quitamos vida al enemigo detectado
					hitInfo.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damage);
				//quitamos cooldown a la ultimate
					player.GetComponent<FPSControler> ().TakeAmountForUlt (amountFoUlt);
				}
				
				//si el objetivo tiene el script EnemyBehaviuour y la ulti está activa
				if (hitInfo.transform.GetComponent<EnemyBehaviour> () != null && ultUp) {
						//le quitamos vida al enemigo detectado
						hitInfo.transform.GetComponent<EnemyBehaviour> ().TakeDamage (damageUlt);
					//tiempo del stun para el enemigo
						hitInfo.transform.GetComponent<EnemyBehaviour> ().timeStunned = stunTime;
					//stuneamos al enemigo
						hitInfo.transform.GetComponent<EnemyBehaviour> ().state = EnemyBehaviour.States.Stunned;

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
		StartCoroutine(ThunderBoldUlti ());
	}

	/// <summary>
	/// Controla la ultimate del ThunderBold
	/// </summary>
	/// <returns>The bold ulti.</returns>
	IEnumerator ThunderBoldUlti (){
		//activamos la booleana que ocntrola si estamos en ultimate
		ultUp = true;
		//activamos el sistema de particulas
		ultiParticleSystem.gameObject.SetActive (true);
		//lo desactivamos todo despues de que haya pasado el tiempo
		yield return new WaitForSeconds (timeUlt);
		ultiParticleSystem.gameObject.SetActive (false);
		ultUp = false;
		yield return null;
	}
}
