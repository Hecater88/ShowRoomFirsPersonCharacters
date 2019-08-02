using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChuckAll :  WeaponShoot {
	public bool drawGizmo;
	public ParticleSystem gunShoot2;
	public Transform eye;
	public float fielOfView = 90f;						// Campo de visión del enemigo.

	public float turnSpeed = 3f;
	public float reach = 10f;
	public LayerMask raycastLayer;	// Layer de visibilidad de la IA.
	public float stunTime;
	public float impactRadiusUlt;
	public ParticleSystem loud;

	/// <summary>
	/// Realiza los cálculos para el disparo. Será llamado desde message, todas las armas deberan
	/// incluir este método.
	/// </summary>
	public void CallShoot(){
		if (areInUlti) {
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




		// Efecto de sonido.
		audiosource.Play();
		// Animación del disparo.
		// Efecto de partículas del arma.
		int coinFlip = Random.Range(0,2);

		if(coinFlip == 1){
			gunShoot.Play ();
			animator.SetTrigger ("Shoot");
		}else{
			gunShoot2.Play ();
			animator.SetTrigger ("Shoot2");}
		

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

		// Tras el disparo, reducimos en 1 la munición. 
		magazine--;
		// Actualizo la munición en el HUD.
		UpdateAmmoDisplay ();
		// Generamos el ruido.
		GenerateSound ();
	}
	/// <summary>
	/// Activa la ulti
	/// </summary>
	public void Ulti(){
		StartCoroutine (ChuckUlti());
	}

	IEnumerator ChuckAllShout (){
		while(areInUlti){

			Collider[] colls = Physics.OverlapSphere(eye.transform.position, impactRadiusUlt);

			foreach (Collider col in colls) {

				EnemyBehaviour enemyBehaviour = col.GetComponent<EnemyBehaviour> ();
				//verificamos que el objeto impactado no sea el mismo, para que no se autostunee
				if (enemyBehaviour != null) {
					//indicamos a todas las instancias del jugador impactado, que pase a estar stuneado
					col.transform.GetComponent<EnemyBehaviour> ().timeStunned = stunTime;
					col.transform.GetComponent<EnemyBehaviour> ().state = EnemyBehaviour.States.Stunned;
				}
			}
			yield return null;
		}


	}

	/// <summary>
	/// COntrolla todos los componenetes que participan en la ultimate
	/// </summary>
	/// <returns>The ulti.</returns>
	IEnumerator ChuckUlti(){
		areInUlti = true;
		loud.gameObject.SetActive (true);
		StartCoroutine(ChuckAllShout ());
		yield return new WaitForSeconds (2f);
		loud.gameObject.SetActive (false);
		areInUlti = false;

	}
	void OnDrawGizmos(){
		//para mostrar el área de efecto de la habilidad
		if (drawGizmo) {
			Gizmos.DrawSphere (eye.transform.position, impactRadiusUlt);
		}
	}
}
