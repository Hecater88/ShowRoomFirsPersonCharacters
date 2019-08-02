using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaDIYMET :  WeaponShoot {
	public GameObject misilePrefab;
	public Transform misileRespawn;
	public GameObject[] misilesRespawn;

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
		GameObject misile = Instantiate (misilePrefab,
			misileRespawn.position,
		Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z));

	
		// Tras el disparo, reducimos en 1 la munición. 
		magazine--;
		// En cuanto se inicia el disparo aplico el retroceso
		player.AddRecoil (recoil);

		// Efecto de sonido.
		// Actualizo la munición en el HUD.
		UpdateAmmoDisplay ();
		// Generamos el ruido.
		GenerateSound ();
		audiosource.Play();
		Destroy (misile,2f);

	}
	/// <summary>
	/// UActiva la ultimate
	/// </summary>
	public void Ulti(){
			StartCoroutine (misiles());

	}

	/// <summary>
	/// Instancia 20 misiles
	/// </summary>
	IEnumerator misiles(){
		areInUlti = true;
		for (int i = 0; i < 20; i++) {
			
			int flipCoin = Random.Range (0, misilesRespawn.Length);

			GameObject misile = Instantiate (misilePrefab,
				                   misilesRespawn [flipCoin].transform.position,
				                   Quaternion.Euler (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));
			
			yield return new WaitForSeconds (0.1f);
			Destroy (misile, 2f);
		}
		areInUlti = false;

	}




}
