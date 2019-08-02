using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revgo : WeaponShoot {
	//booleana que controla si estas en estado de mirilla
	private bool scoped = false;
	//sprite de la mirilla
	public GameObject scopeUI;
	//camara
	public GameObject weaponCamera;
	//mirilla normal
	public GameObject crossUI;
	//objeto que cura
	public GameObject revgoUltiPrefab;
	//objeto que cura de mentira, solo es para la animacion
	public GameObject resurection;
	//position para el objeto que cura
	public Transform positionForUlt;
	//field of view de la camara para cuando este en modo sniper
	public float fovMSniper = 15f;
	//variable que guarda el field of view standard
	private float normalFov;

	/// <summary>
	///Activacion de la mirilla con el boton derecho del raton
	/// </summary>
	public void Hability(){
		Scope ();
	}

	/// <summary>
	/// Activa ultimate
	/// </summary>
	public void Ulti(){
		RevgoUlti ();
	}

	/// <summary>
	/// Metodo que controla si estas o no en estado sniper
	/// </summary>
	public void Scope(){
		scoped = !scoped;


		if (scoped) {
			//activamos la mirilla sniper
			StartCoroutine(Scoped());
		} else {
			UnScoped ();
		}
			
	}
	/// <summary>
	/// Activa la mirilla
	/// </summary>
	IEnumerator Scoped(){
		animator.SetBool("Scope",true);
		yield return new WaitForSecondsRealtime (.15f);
		scopeUI.SetActive (true);
		weaponCamera.SetActive (false);
		crossUI.SetActive (false);
		normalFov = fpsCamera.fieldOfView;
		fpsCamera.fieldOfView = fovMSniper;

	}
	/// <summary>
	/// metodo que desactiva la mirilla sniper
	/// </summary>
	void UnScoped(){
		
		animator.SetBool("Scope",false);
		fpsCamera.fieldOfView = normalFov;
		scopeUI.SetActive (false);
		weaponCamera.SetActive (true);
		crossUI.SetActive (true);
	}
	/// <summary>
	/// Realiza los cálculos para el disparo. Será llamado desde message, todas las armas deberan
	/// incluir este método.
	/// </summary>
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

		// Efecto de partículas del arma.


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
			//if (hitInfo.transform.GetComponent<EnemyBehaviour> () != null) {
			//hitInfo.transform.GetComponent<EnemyBehaviour> ().state = EnemyBehaviour.States.Stunned;
			//}

		}

		// Tras el disparo, reducimos en 1 la munición. 
		magazine--;
		// Actualizo la munición en el HUD.
		UpdateAmmoDisplay ();
		GenerateSound ();

	}

	/// <summary>
	/// Activamos la animacion de ulti
	/// </summary>
	void RevgoUlti (){
		animator.SetTrigger ("Ulti");

	}
	/// <summary>
	/// Actives el objeto de mentira.
	/// </summary>
	public void ActiveResurection(){
		resurection.SetActive (true);
	}

	/// <summary>
	/// Desactives el objeto de mentira.
	/// </summary>
	public void DesactiveResurection(){
		resurection.SetActive (false);
	}
	/// <summary>
	/// Instantiates the resurect.
	/// </summary>
	public void InstantiateResurect(){
		Instantiate (revgoUltiPrefab, positionForUlt.position, Quaternion.identity);
	}
}

