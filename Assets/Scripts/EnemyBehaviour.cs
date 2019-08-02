using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour {
	public Animator anim;
	public int lifeEnemy = 100;
	public enum States {Patrol, Noise,	Attack, Stunned}// Para definir cada uno de los posibles estados de nuestra IA.
														// Patrol --> Modo patrulla, de waypoint a waypoint.
														// Noise --> Modo seguimiento de un sonido.
														// Attack --> Modo de avistamiento del jugador.
														//Stunned --> Modo estuneado

	public States state;								// Estado actual en el que se encuentra la IA.
	public TextMesh stateText;							// Texto que muestra el estado.

	[Header("Patrulla")]
	public Transform[] waypoints;						// Waypoints de la patrulla.
	private int actualWaypoint;							// Waypoint actual en la ruta de la pantalla.

	public float minDistanceChangeWaypoint = 0.2f;		// Distancia mínima para cambio de waypoint.
	private NavMeshAgent nav;							// Referencia al navmesh agent.

	[Header("Ataque")]
	public float reach = 10f;							// Alcance de la visión para detección de objetivos.
	public float shootingRange = 5f;					// Distancia a la que disparará el enemigo.
	public Transform eye;								// Transform utilizado como origen del raycast.
	private Transform target;							// Objetivo del seguimiento.
	public LayerMask raycastLayer;						// Layer de visibilidad de la IA.
	public float fielOfView = 90f;						// Campo de visión del enemigo.
	private int viewDirection = 1;						// Dirección de giro.
	public float turnSpeed = 3f;						// Velocidad de giro del ojo.

	public bool targetAtView = false;					// Booleana para controlar si el objetivo está a la vista.

	public float timeToDisengage = 15f;					// Tiempo que debe pasar para que el enemigo abandone la persecución.
	public float disengageCounter = 0f;					// Contador interno de tiempo que lleva sin ver al jugador.
	public float dispersionRange;						// precisión de disparo de la IA
	public float timeMaxStunned;
	public float timeStunned;
	public bool stun;
	public Slider healthSlider;
	public CapsuleCollider capsuleCollider;

	[Header("Oido")]
	public float hearDistance = 10f;					// Distancia a la que será capaz de escuchar.
	private Vector3 lastSoundPosition;					// La posición del último sonido escuchado.

	// Use this for initialization
	void Start () {
		// Recuperamos la referencia al navmeshagent.
		nav = GetComponent<NavMeshAgent> ();
		capsuleCollider = GetComponent<CapsuleCollider> ();
		// Por defecto iniciamos la IA en modo patrulla.
		state = States.Patrol;

		timeStunned = timeMaxStunned;

		// Inicia el chequeo de estados posibles.
		StartCoroutine (StateCheck ());
	}
	
	// Update is called once per frame
	void Update () {
		if (lifeEnemy < 0) {
			nav.isStopped = true;
			anim.SetTrigger ("Morir");

			healthSlider.gameObject.SetActive (false);
			stateText.gameObject.SetActive (false);
			Destroy (this);
			Destroy(this.gameObject, 3f);

		} else if (lifeEnemy > 100) {
			lifeEnemy = 100;
		}
		if(state == States.Stunned){
			if (timeStunned > 0) {
				timeStunned -= Time.deltaTime;
			} else {
				stun = false;
				timeStunned = timeMaxStunned;
				state = States.Attack;
			}
		}

		// Si estamos en modo ataque
		if (state == States.Attack) {
			// Y el contador de tiempo es mayor que 0
			if (disengageCounter > 0) {
				// Descuento el tiempo que falta para abandonar el objetivo
				disengageCounter -= Time.deltaTime;
			} else {
				// Si el tiempo se ha acabado, paso a modo de patrulla.
				state = States.Patrol;
			}

		} 

	}

	void FixedUpdate (){

		CheckView ();

		Attacking ();
	}

	/// <summary>
	/// Cambia el texto que indica el estado del enemigo.
	/// </summary>
	void StateIndicator(){
		healthSlider.transform.LookAt (Camera.main.transform.position);
		// Hacemos que mire hacia la cámara.
		stateText.transform.LookAt (Camera.main.transform.position);

		if (state == States.Patrol) {
			stateText.text = "";
		} else if (state == States.Stunned) {
			stateText.color = Color.white;
			stateText.text = "STUNNED";
		} else if (state == States.Attack) {
			stateText.color = Color.red;
			stateText.text = "!";
		} else if (state == States.Noise) {
			stateText.color = Color.yellow;
			stateText.text = "?";
		}
	}

	/// <summary>
	/// Corrutina con un bucle inferior al del update, para gestionar los cambios en la maquina de estados de la IA.
	/// No es necesario tanto ajuste y por tanto ahorramos recursos.
	/// </summary>
	/// <returns>The check.</returns>
	IEnumerator StateCheck(){
		while (true) {
			// Verifico el estado a mostrar sobre el enemigo.
			StateIndicator();

			if (state == States.Patrol) {
				Patroll ();
			} else if (state == States.Attack) {
				Attack ();
			} else if (state == States.Noise) {
				FollowNoise ();
			} else if (state == States.Stunned) {
				Stunned (timeStunned);
			}
			// 10 veces por segundo.
			yield return new WaitForSeconds (0.1f);
		}
	}

	/// <summary>
	/// Acciones para el estado de patrulla.
	/// </summary>
	void Patroll (){
		anim.SetBool ("Correr",true);
		if (stun) {
			return;
		}
		// siempre en patrol nos aseguramos de que el navmeshAgent enté en movimiento
		nav.isStopped = false;
		// Verificamos si la distancia restante al waypoint es inferior a la indicada.
		if (nav.remainingDistance < minDistanceChangeWaypoint) {
			actualWaypoint = Random.Range (0, waypoints.Length);

			// Si llegamos al final de los waypoints disponibles volvemos al primero.
			if (actualWaypoint >= waypoints.Length) {
				actualWaypoint = 0;
			}
		}
		// Fijamos la ruta hacia el waypoint correspondiente.
		nav.SetDestination (waypoints [actualWaypoint].position);
	}

	/// <summary>
	/// Verifica la visión de la IA.
	/// </summary>
	void CheckView(){
		if (stun) {
			return;
		}
		// Si el viewDirection es positivo, significa que estamos incrementando el ángulo de visión hacia la derecha.
		if (viewDirection > 0) {
			// Giramos el ojo hacia la derecha a la velocidad indicada.
			eye.localEulerAngles = new Vector3 (eye.localEulerAngles.x, 
				eye.localEulerAngles.y + turnSpeed * Time.deltaTime,
				eye.localEulerAngles.z);
			// Si superamos el fov especificado, invertimos la dirección de giro.
			if (eye.localEulerAngles.y > (180 + (fielOfView / 2))) {
				// Invierto la dirección.
				viewDirection = -1;
			}

		} else {
			// Giramos el ojo hacia la izquierda a la velocidad indicada.
			eye.localEulerAngles = new Vector3 (eye.localEulerAngles.x, 
				eye.localEulerAngles.y - turnSpeed * Time.deltaTime,
				eye.localEulerAngles.z);
			// Si superamos el fov especificado, invertimos la dirección de giro.
			if (eye.localEulerAngles.y < (180 - (fielOfView / 2))) {
				// Invierto la dirección.
				viewDirection = 1;
			}

		}




		// Calculamos el alcance máximo de la visión.
		// Invertimos el ojo y ponemos en negativo para controlar más facilmente la oscilación y controlar el giro.
		Vector3 targetView = (-eye.forward * reach + eye.position);

		RaycastHit hit = new RaycastHit ();

		if (Physics.Linecast(eye.position, targetView, out hit, raycastLayer)) {

			// Si el linecast impacta al jugador y el estado no es de ataque.
			if (hit.collider.tag == "Player" && state != States.Attack) {
				Debug.Log ("Jugador a la vista");
				state = States.Attack;
				// Si ha visto al jugador, el objetivo de su movimiento
				// será el jugador.
				target = hit.collider.gameObject.transform;
				targetAtView = true;
				disengageCounter = timeToDisengage;
			}
			if (hit.collider.tag == "Player") {
				targetAtView = true;
			} else {
				targetAtView = false;
			}

			// Si el linecast impacta al jugador y estamos en modo ataque, reseteo el contador de tiempo para abandonar el objetivo.
			if (hit.collider.tag == "Player" && state == States.Attack) {
				disengageCounter = timeToDisengage;
			}

			// Debug line si hay algo a la vista.
			Debug.DrawLine (eye.position, hit.point, Color.green);
		} else {
			// Debug line si no hay nada a la vista.
			Debug.DrawLine(eye.position, targetView, Color.red);
		}
	}

	/// <summary>
	/// Acciones para el modo de ataque.
	/// </summary>
	void Attack(){
		if (stun) {
			return;
		}
		// Actualiza la posición del objetivo.
		nav.SetDestination (target.position);


	}

	/// <summary>
	/// Acciones de ataque hacia el jugador.
	/// </summary>
	void Attacking () {
		if (stun) {
			return;
		}
		if (state == States.Attack) {
			// si el enemigo se encuentra dentro de su rango de ataque
			if (nav.remainingDistance <= shootingRange) {
				// paramos el movimiento
				nav.isStopped = true;
				// para despreciar la posición en y del enemigo creo un vector 3 temporal que recupere x z del objetivo y mantenga
				Vector3 tempLook = new Vector3 (target.position.x, transform.position.y, target.position.z);

				transform.LookAt (tempLook);

				if (targetAtView) {
					// almaceno la rotación del ojo antes del disparo
					Quaternion originalRotation = eye.rotation;
					// roto aleatoriamente el ojo en fucnición de la dispersión indicada
					eye.Rotate(Random.Range(-dispersionRange,dispersionRange),Random.Range(-dispersionRange,dispersionRange), 0f);

					// disparamos
					BroadcastMessage ("CallShoot");
					// recupero la posición original del ojo antes del disparo
					eye.rotation = originalRotation;
				}
			} else {
				nav.isStopped = false;
			}
		}
	}
	/// <summary>
	/// Almacena la posición del último sonido e inicia el estado de seguimiento de sonido.
	/// </summary>
	public void HearSound(Vector3 position){
		if (stun) {
			return;
		}
		// Si la distancia a la que se encuentra el sonido es mayor que la distancia a la que es capaz de escuchar
		// salimos del método.
		if (Vector3.Distance (transform.position, position) > hearDistance) {
			return;
		}

		// Si el estado actual es distinto al estado de ataque, cambio a estado noise.
		if (state != States.Attack) {
			state = States.Noise;
		}

		// Indicamos al enemigo el origen del sonido.
		lastSoundPosition = position;
	}

	/// <summary>
	/// Follows the noise.
	/// </summary>
	void FollowNoise(){
		if (stun) {
			return;
		}
		// Fijo el destino a la última posición del sonido.
		nav.SetDestination (lastSoundPosition);

		// Si la distancia para llegar al objetivo es menor que la definida, pasamos a modo patrulla.
		if (nav.remainingDistance < minDistanceChangeWaypoint) {
			state = States.Patrol;
		}
	}

	/// <summary>
	/// Stunea al enemigo
	/// </summary>
	/// <param name="time">Time.</param>
	void Stunned(float time){
		timeStunned = time;
		if (state == States.Stunned ) {
			stun = true;
			nav.isStopped = true;
		} 
	}

	/// <summary>
	/// Takes the damage.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void TakeDamage(int amount){
		anim.SetTrigger ("Daño");
		lifeEnemy -= amount;
		UpdateLifeEnemy ();
	}
	/// <summary>
	/// Updates the life enemy.
	/// </summary>
	public void UpdateLifeEnemy(){
		healthSlider.value = (float)lifeEnemy;
	}
	/// <summary>
	/// Desactives the capsule collider.
	/// </summary>
	public void DesactiveCapsuleCollider(){
		transform.GetComponent<CapsuleCollider> ().enabled = false;
	}

}
