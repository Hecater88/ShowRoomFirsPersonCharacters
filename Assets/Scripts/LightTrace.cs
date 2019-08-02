using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrace : WeaponShoot {
	public float stunTime = 0.1f;
	public float distance = 10f;
	private int stackStun;
	public float nextTimeHability;
	public float habilityDelay;


	public void Hability(){
		Blink ();
	}

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
		animator.SetTrigger ("Shoot");
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
			if (hitInfo.transform.GetComponent<EnemyBehaviour> () != null) {
				stackStun++;
				if (stackStun > 4) {
					
					hitInfo.transform.GetComponent<EnemyBehaviour> ().timeStunned = stunTime;
					hitInfo.transform.GetComponent<EnemyBehaviour> ().state = EnemyBehaviour.States.Stunned;
					stackStun = 0;
				}
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
	public void Blink(){
		// Si no ha pasado el tiempo de delay, salgo de la función.
		if (Time.time < nextTimeHability) {
			return;
		}

		// Calculamos el siguiente tiempo de delay al final del método.
		nextTimeHability = Time.time + habilityDelay;
		RaycastHit hit;
		Vector3 destination = player.transform.position + player.transform.forward * distance;

		//verificamos si hay alguna pared delante
		if (Physics.Linecast (transform.position, destination, out hit)) {
			destination = player.transform.position + player.transform.forward *(hit.distance - 1f);
		}

		//movemos nuestro player
		if (Physics.Raycast (destination, -Vector3.up, out hit)) {
			destination = hit.point;
			destination.y = 0.5f;
			player.transform.position = destination;
		}


	}
}
