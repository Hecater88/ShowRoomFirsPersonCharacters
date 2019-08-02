using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarNesha : WeaponShoot {
	public float impactRadiusUlt;
	public float ultDelayDamage = 0.5f;
	public int damageUlt;
	public float maxYantigravity;
	public float velocityAntigravityUpUlt;
	public float timeUlt;
	public GameObject lanzaAnimation;
	public ParticleSystem fire;
	// Use this for initialization
	void Start () {
		isGarNesha = true;
	}


	public void CallShoot(){
		animator.SetTrigger("Shoot");
	}
	public void CallShootGarNesha(){

		// Si no ha pasado el tiempo de delay, salgo de la función.
		if (Time.time < nextTimeToShoot) {
			return;
		}

		// Calculamos el siguiente tiempo de delay al final del método.
		nextTimeToShoot = Time.time + shootDelay;



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


		// Actualizo la munición en el HUD.
		//GenerateSound ();
		// Generamos el ruido.
		//if (hitInfo.transform.GetComponent<EnemyBehaviour> ().stun == false) {
		//GenerateSound ();
		//}
	}

	void OnDrawGizmos(){
		//para mostrar el área de efecto de la habilidad

		Debug.DrawLine(fpsCamera.transform.position, fpsCamera.transform.forward*range ,Color.green);

	}

	/// <summary>
	/// Desactivamos las gravedad para que pueda volar	/// </summary>
	/// <returns>The ult gar nesha.</returns>
	IEnumerator AntigravityUltGarNesha(){
		animator.SetBool ("Ulti", true);
		player.GetComponent<FPSControler>().ultiWithoutGravity = true;

		while (player.transform.position.y < maxYantigravity) {
			Vector3 destination = new Vector3(player.transform.position.x, player.transform.position.y +(velocityAntigravityUpUlt*Time.deltaTime),player.transform.position.z);
			player.transform.position = destination;
			yield return null;
		}

		player.GetComponent<FPSControler> ().haveSpecial = true;

	}

	/// <summary>
	/// Area de daño, creado por un ovarlapsphere
	/// </summary>
	/// <returns>The fire gar nesha.</returns>
	IEnumerator AreaFireGarNesha(){
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
	/// Ultimate que controla todos los componentes de GarNesha	/// </summary>
	/// <returns>The gar nesha.</returns>
	IEnumerator UltimateGarNesha(){
		fire.gameObject.SetActive (true);
		StartCoroutine(AntigravityUltGarNesha ());
		StartCoroutine(AreaFireGarNesha ());

		yield return new WaitForSeconds (timeUlt);
		fire.gameObject.SetActive (false);
		areInUlti = false;

		animator.SetBool ("Ulti", false);
		yield return new WaitForSeconds (0.5f);
		player.GetComponent<FPSControler> ().haveSpecial = false;


		player.GetComponent<FPSControler>().ultiWithoutGravity = false;


	}

	/// <summary>
	/// Casts the ulti.
	/// </summary>
	public void CastUlti(){

		StartCoroutine(UltimateGarNesha ());
	}
	/// <summary>
	/// Activo la ulti mediante una animacion
	/// </summary>
	public void Ulti(){
		areInUlti = true;
		animator.SetTrigger ("CastUlti");
	}

	
}
