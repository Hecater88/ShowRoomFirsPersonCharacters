using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShootOnline : MonoBehaviour {

	public float range = 100f;			// Alcance del arma.
	public float impactForce = 30f;		// Fuerza aplicada con el impacto del arma.
	public ParticleSystem gunShoot;		// Partícula activada al disparar. 
	public GameObject impactParticle;	// Prefab de partícula en el punto de impacto.
	public GameObject impactDecal;		// Decal a instanciar en el punto de impacto.

	public LayerMask shootableLayer;	// Listado de layers disparables.

	public float decalLifetime = 20f;	// Variable para el tiempo que se tarda en destruir el decal.

	public float shootDelay = 0.5f;		// Retardo entre disparos.
	private float nextTimeToShoot = 0f;	// Tiempo en el que podremos realizar un siguiente disparo.

	public int magazineSize = 15;		// Tamaño del cargador.
	public int magazine;				// Cargador actual.
	public AudioClip noAmmoSound;		// Sonido para cuando ha terminado la munición.

	public Text ammoText;				// Texto para mostrar la munición disponible

	public Transform head;				// Origen del raycast
	private Animator animator;			// Animator del arma.
	private AudioSource audiosource;	// Para el sonido del disparo.

	public float soundRange = 10f;		// Rango de sonido generado por el arma.

	private FPSControllerOnline player;		
	public float recoil = 4f;

	// daño que será aplicado en cada impacto del arma
	public float damage = 10f;

	// Use this for initialization
	void Start () {
		
		animator = GetComponent<Animator> ();
		audiosource = GetComponent<AudioSource> ();

		// Recarga la munición.
		Reload();

		// Recupero la cabeza.
		player = GetComponentInParent<FPSControllerOnline> ();
	}

	// Update is called once per frame
	void Update () {

	}
	void OnEnable(){
		//respawneamos con el cargador completo
		Reload ();
	}

	/// <summary>
	/// Realiza los cálculos para el disparo. Será llamado desde message, todas las armas deberan
	/// incluir este método.
	/// </summary>
	public void CallShoot(Vector3 direction){
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
		gunShoot.Play ();
		// Efecto de sonido.
		audiosource.Play();
		// Animación del disparo.
		animator.SetTrigger ("Shoot");

		// Para almacenar la información de si el raycast ha golpeado algún objeto.
		bool impact = false;

		// Variable raycasthit para almacenar la información del impacto.
		RaycastHit hitInfo;

		// calculamos el raycast sumandole el direction, para hacer que se calcule el origen del raycast, fuera del cuerpo para evitar dispararse a si mismo
		impact = Physics.Raycast (head.position + direction, 
			direction,
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
				hitInfo.rigidbody.AddForce (direction * impactForce, ForceMode.Impulse);
			}

			PlayerHealthOnline enemy = hitInfo.transform.GetComponent<PlayerHealthOnline> ();
			if(enemy != null) {
				//si se trata de un impacto mortal
				if (enemy.TakeDamage (damage)) {
					//apuntamos la kill a este jugador
					GetComponentInParent<Score> ().kills++;
				}
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
	/// Método que será llamado desde broadcast para iniciar el proceso de recarga.
	/// </summary>
	public void CallReload(){
		// Si el cargador actual es menor que el tamaño del cargador, permitimos la recarga.
		if (magazine < magazineSize) {
			// Ejecutamos la animación de recarga.
			animator.SetTrigger ("Reload");
		}
	}

	/// <summary>
	/// Recarga el cargador.
	/// </summary>
	public void Reload(){
		magazine = magazineSize;
		UpdateAmmoDisplay ();
	}

	/// <summary>
	/// Actualiza la munición en el HUD.
	/// </summary>
	public void UpdateAmmoDisplay(){
		// Actualizo la munición en el HUD.
		ammoText.text = magazine.ToString() + " / " + magazineSize.ToString();
	}

	/// <summary>
	/// Sonido generado por el arma, que puede llamar la atención de los enemigos dentro del alcance.
	/// </summary>
	public void GenerateSound(){
		Collider[] cols = Physics.OverlapSphere (transform.position, soundRange);

		// Revisamos si existe en la lista un enemigo que pueda escuchar el sonido.
		foreach (Collider col in cols) {
			// Intentanmos recuperar el componente EnemyBehaviour.
			EnemyBehaviour enemyBehaviour = col.GetComponent<EnemyBehaviour> ();

			if (enemyBehaviour != null) {
				enemyBehaviour.HearSound (transform.position);
			}
		}

	}
}
