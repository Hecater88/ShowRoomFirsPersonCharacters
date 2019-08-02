using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperMod :WeaponShoot {
	public GameObject granadePrefab;
	public Transform granadeRespawn;
	public float timeUlt;
	public bool ultUp;
	public ParticleSystem ultiParticleSystem;
	public float jumpForceUlt;



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
		Instantiate (granadePrefab,
			granadeRespawn.position,
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
		gunShoot.Play();
		audiosource.Play();



	}

	public void Ulti(){
		StartCoroutine (UltiJumperMod ());
	}

	IEnumerator UltiJumperMod(){
		ultUp = true;
		float jumpSave = player.GetComponent<FPSControler> ().jumpForce;

		player.GetComponent<FPSControler> ().jumpForce = jumpForceUlt;

		ultiParticleSystem.Play();

		yield return new WaitForSeconds (timeUlt);
		ultUp = false;
		player.GetComponent<FPSControler> ().jumpForce = jumpSave ;

		yield return null;
	}
}
