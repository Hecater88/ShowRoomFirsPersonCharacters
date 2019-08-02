using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponShoot : MonoBehaviour {
	
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
	public bool reloading;				// booleana para controlar si está recargando

	public GameObject eye;				// Lugar desde donde se tira el raycast
	public Animator animator;			// Animator del arma.
	private AudioSource audiosource;	// Para el sonido del disparo.

	public float soundRange = 10f;		// Rango de sonido generado por el arma.

	// Use this for initialization
	void Start () {
		// Recuperamos los componentes.

		audiosource = GetComponent<AudioSource> ();

		// Recarga la munición.
		Reload();
	}

	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Realiza los cálculos para el disparo. Será llamado desde message, todas las armas deberan
	/// incluir este método.
	/// </summary>
	public void CallShoot(){
		if (magazine == 0 && !reloading) {
			reloading = true;
			Reload ();
			//animator.SetTrigger ("Reload");
		} else if (magazine > 0) {
			reloading = false;
			// Si no ha pasado el tiempo de delay, salgo de la función.
			if (Time.time < nextTimeToShoot) {
				return;
			}

			// Calculamos el siguiente tiempo de delay al final del método.
			nextTimeToShoot = Time.time + shootDelay;

			// Efecto de partículas del arma.
			gunShoot.Play ();
			// Efecto de sonido.
			audiosource.Play ();
			// Animación del disparo.
			animator.SetTrigger ("Atacar");

			// Para almacenar la información de si el raycast ha golpeado algún objeto.
			bool impact = false;

			// Variable raycasthit para almacenar la información del impacto.
			RaycastHit hitInfo;

			//
			impact = Physics.Raycast (eye.transform.position, 
				-eye.transform.forward,
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
					hitInfo.rigidbody.AddForce (-eye.transform.forward * impactForce, ForceMode.Impulse);
				}

			}

			// Tras el disparo, reducimos en 1 la munición. 
			magazine--;
			// Generamos el ruido.
			GenerateSound ();
		}
		}

	/// <summary>
	/// Recarga el cargador.
	/// </summary>
	public void Reload(){
		magazine = magazineSize;
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
