using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShoot : MonoBehaviour {
	//boleana que controla si eres GarNesha
	public bool isGarNesha = false;
	//float para aumentar el fiilamount de la imagen  de la Ulti
	public float amountFoUlt = 0.2F;
	//booleana que controla si estas durante un ultimate
	public bool areInUlti = false;
	//daño del arma;
	public int damage = 60;
	public float range = 100f;			// Alcance del arma.
	public float rangeHook = 100f;
	public float impactForce = 30f;		// Fuerza aplicada con el impacto del arma.
	public ParticleSystem gunShoot;		// Partícula activada al disparar. 
	public GameObject impactParticle;	// Prefab de partícula en el punto de impacto.
	public GameObject impactDecal;		// Decal a instanciar en el punto de impacto.

	public LayerMask shootableLayer;	// Listado de layers disparables.

	public float decalLifetime = 20f;	// Variable para el tiempo que se tarda en destruir el decal.

	public float shootDelay = 0.5f;		// Retardo entre disparos.
	public float nextTimeToShoot = 0f;	// Tiempo en el que podremos realizar un siguiente disparo.

	public int magazineSize = 15;		// Tamaño del cargador.
	public int magazine;				// Cargador actual.
	public AudioClip noAmmoSound;		// Sonido para cuando ha terminado la munición.

	public Text ammoText;				// Texto para mostrar la munición disponible

	public Camera fpsCamera;			// Cámara usada para el raycast.
	public Animator animator;			// Animator del arma.
	public AudioSource audiosource;	// Para el sonido del disparo.

	public float soundRange = 10f;		// Rango de sonido generado por el arma.

	public FPSControler player;		// 
	public float recoil = 4f;			//recoil del arma

	//Prefab del hook
	public GameObject hook;
	//distancia entre el hook y la pared
	public float distanceHitForHook;
	//distancia entre el player y el hook
	public float distancePlayerHook;
	//destino del player despues de tirar el hook
	public Vector3 destinyForPlayer;
	//contador del cooldows del Hook
	public float nextTimeToHook;
	//cooldown del hook
	public float hookDelay = 6f;


	// Use this for initialization
	void Start () {
		// Recuperamos los componentes.
		//fpsCamera = GetComponentInParent<Camera> ();
		if(animator == null){
			animator = GetComponent<Animator> ();
		}

		
		audiosource = GetComponent<AudioSource> ();

		// Recarga la munición.
		Reload();


		// Recupero la cabeza.
		player = GetComponentInParent<FPSControler> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(isGarNesha){
			ammoText.text =  "Infinity";
		}
	}
		

	/// <summary>
	/// Método que será llamado desde broadcast para iniciar el proceso de recarga.
	/// </summary>
	public void CallReload(){
		if(areInUlti){
			return;
		}
		
		// Si el cargador actual es menor que el tamaño del cargador, permitimos la recarga.
		if (magazine < magazineSize) {
			
			//si no hay animacion
			if(animator == null){
				//recargamos el arma
				magazine = magazineSize;
				UpdateAmmoDisplay ();
				return;
			}
			// Ejecutamos la animación de recarga.
			animator.SetTrigger ("Reload");
		}
	}

	/// <summary>
	/// Recarga el cargador.
	/// </summary>
	public void Reload(){
		//si estas en ulti no hace falta recargar
		if(areInUlti){
			return;
		}
		//si eres gar nesha no tienes balas
		if(isGarNesha){
			return;
		}
		//actualizamos el cargador
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

	/// <summary>
	/// Calls the hook.
	/// </summary>
	public void CallHook(){
		//cooldown del hook
		if(Time.time< nextTimeToHook){
			return;
		}
		//actualizamos el cooldown
		nextTimeToHook = Time.time + hookDelay;
		//si estas durante un ultimate no se puede utilizar el hook
		if(areInUlti){
			return;
		}
		// Para almacenar la información de si el raycast ha golpeado algún objeto.
		bool impact = false;


		// Variable raycasthit para almacenar la información del impacto.
		RaycastHit hitInfo;

		//booleana para el raycast
		impact = Physics.Raycast (fpsCamera.transform.position, 
			fpsCamera.transform.forward,
			out hitInfo,
			rangeHook,
			shootableLayer);
		//si choca
		if (impact) {
			Debug.Log (hitInfo.transform.name+"hook");
			//instanciamos el prefab del hook
			GameObject hookShot = Instantiate (hook, transform.position, Quaternion.identity);
			//calculamos la posicion donde va a ir el hook
			Vector3 hookPoint = hitInfo.point + hitInfo.normal * distanceHitForHook;
			//calculamos el destino para el player
			destinyForPlayer = hookPoint + hitInfo.normal * distancePlayerHook;
			//guardamos la variable en destinyHookForPlayer
			player.GetComponent<FPSControler> ().destinyHookForPlayer = destinyForPlayer;
			//hacemos que se mueva el hook
			hookShot.GetComponent<HookShot> ().destiny = hookPoint;
			Debug.Log(hitInfo.point + " " + hitInfo.normal);
			Destroy (hookShot, 2f);

		}

	}

}
