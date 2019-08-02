using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class FPSControlerOnline : NetworkBehaviour {

	private float speed = 2f;							// Velocidad de desplazamiento.
	public float regularSpeed = 2f;						// Velocidad de desplazamiento normal.		
	public float runSpeed = 4f;							// Velocidad de desplazamiento en carrera.

	public float stepDistance = 1f;						// Distancia que deberá recorrer el player para que suene el sonido de un paso.
	public AudioClip stepSound;							// Sonido del paso.
	public AudioClip jumpSound;							// Sonido del salto.
	public AudioClip landingSound;						// Sonido para caidas.

	private float stepDistanceCounter = 0f;				// Contador de distancia, para controlar cuando se llega a la distancia de un paso.
	public float minVerticalDistanceSound = 0.5f;		// Distancia vertical que debe recorrer el player, para que se reproduzca el sonido de caida.
	private float verticalDistanceCounter = 0f;			// Contador de caida vertical.
	private AudioSource audioSource;					// Referencia al Audiosource.

	public float sensitivity = 2f;						// Velociada de rotación.

	//indica si puede o no realizar las interacciones de disparo
	public bool canShoot;

	//la referencia del player que será utilizada como origen del raycast de disparo
	public Transform firePosition;

	public GameObject head;								// Referencia del objeto cabeza.

	CharacterController player;							// Referencia al componente character controller.

	float moveHorizontal;								// Variable donde almacenaremos de forma temporal el valor de los ejes.
	float moveVertical;									// Variable donde almacenaremos de forma temporal el valor de los ejes.

	float rotationHorizontal;							// Variable donde almacenaremos de forma temporal el valor de la rotación de la cámara.
	float rotationVertical;								// Variable donde almacenaremos de forma temporal el valor de la rotación de la cámara.

	Vector3 movement = new Vector3();					// Dirección final de desplazamiento.

	public bool invertView;								// Permite invertir el movimiento vertical de la cámara.

	public float verticalV;								// Variable con la que controlaremos la velocidad vertical.

	private bool jumped;								// Saltando.
	public float jumpForce = 4f;						// Fuerza de salto.	
	public bool isPreviouslyGrounded;					// Almacenamos el estado anterior de player.grounded.

	public bool fullAuto;								// Indica si se podrá disparar de forma continua o tiro a tiro.

	public bool crouched = false;						// 
	public float regularHeight = 2f;					//
	public float crouchedHeight = 1.5f;					//

	public float pushForce = 2f;						// Fuerza de empuje a los objetos con un rigidbody.


	// Use this for initialization
	void Start () {
		// Recuperamos la referencia al componenete CharacterController.
		player = GetComponent<CharacterController> ();
		// Asignamos la velocidad normal.
		speed = regularSpeed;
		// Referencia al componente audiosource.
		audioSource = GetComponent<AudioSource> ();

		//si el jugador es el local, le permiteremos realizar las interacciones
		if (isLocalPlayer) {
			canShoot = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!canShoot) {
			return;
		}

		Movement ();
		ApplyGravity ();
		if (Input.GetButtonDown ("Jump")) {
			Jump ();
		}
		// Si se está pulsando el botón correr, el jugador está tocando el suelo y se está desplazando hacia adelante
		// permitimos que pueda correr.
		if (Input.GetButton ("Run") && player.isGrounded && moveVertical > 0){
			speed = runSpeed;
		} else {
			speed = regularSpeed;
		}

		if (Input.GetButtonDown ("Crouch")) {
			// Invierto el estado actual de la booleana.
			crouched = !crouched;
			// Asigno la altura correspondiente al valor de la booleana.
			player.height = crouched?crouchedHeight:regularHeight;
		}

		if (!fullAuto && Input.GetButtonDown ("Fire1")) {
			//le indicamos al servidor la dirección en la que disparamos
			CmdShoot(head.transform.forward);
		} 

		// Si dejamos pulsado el botón dispara continuamente despues del delay.
		if (fullAuto && Input.GetButton ("Fire1")) {
			//le indicamos al servidor la dirección en la que disparamos
			CmdShoot(head.transform.forward);
		}


		if (Input.GetButtonDown ("Reload")) {
			CmdReload ();
		}

	}

	/// <summary>
	/// Movimiento y rotación del player.
	/// </summary>
	void Movement (){
		// Recuperamos el desplazamiento del ratón para realizar la rotación del jugador
		rotationHorizontal = Input.GetAxis ("Mouse X") * sensitivity; 
		rotationVertical += Input.GetAxis ("Mouse Y") * sensitivity * (invertView?1:-1);

		// Limitamos la rotación vertical para que solo pueda tener valores entre -90 y 90.
		rotationVertical = Mathf.Clamp (rotationVertical, -90f, 90f);

		// Aplicamos la rotación horizontal al player-
		transform.Rotate (0f, rotationHorizontal, 0f);

		// Rotamos verticalmente la cabeza del jugador.
		head.transform.localEulerAngles = new Vector3 (rotationVertical, 0f, 0f);

		// Guardamos el valor del eje horizontal. GetAxisRaw realiza el movimiento sin transición al usar las teclas	
		moveHorizontal = Input.GetAxisRaw ("Horizontal");
		// Guardamos el valor del eje vertical.
		moveVertical = Input.GetAxisRaw ("Vertical");

		// Asignar el valor recuperado con los input a un vector que usaremos para indicar el desplazamiento.
		movement.Set (moveHorizontal, 0, moveVertical);

		// Orientamos el vector de movimiento aplicándole la misma rotación que el objeto.
		movement = transform.rotation * movement;

		// Normalizamos el vector de desplazamiento para que las diagonales no se desplace más rápido.
		movement = movement.normalized;

		// Le indicamos al character controller que realizaremos un desplazamiento, indicando el vector de dirección 
		// y aplicando la velocidad de desplazamiento controlada con time.deltatime. 
		player.Move (movement * speed * Time.deltaTime);

		if (isPreviouslyGrounded) {
			// Incrementamos la distancia desplazada en el contador de distancia.
			stepDistanceCounter += movement.magnitude * speed * Time.deltaTime;
		}

		// Incrementamos la distancia desplazada en el contador de distancia.
		stepDistanceCounter += movement.magnitude * speed * Time.deltaTime;
		// Si el contador de distancia ha llegado a la distancia del paso.
		if (stepDistanceCounter >= stepDistance) {
			// Reseteamos el contador.
			stepDistanceCounter = 0f;
			// Hacemos que el pitch sea aleatorio para modificar.
			audioSource.pitch = Random.Range (0.8f, 1.2f);
			// Reproducimos el sonido del paso.
			audioSource.PlayOneShot (stepSound);
		}
	}

	/// <summary>
	/// Realizamos la simulación de la gravedad.
	/// </summary>
	void ApplyGravity(){
		// Si previamente el player no estaba tocando el suelo.
		if (!isPreviouslyGrounded) {
			// Y la velocidad vertical es negativa, lo que significaria que estamos en caida.
			if (verticalV < 0) {
				verticalDistanceCounter += verticalV * Time.deltaTime;
			}
		} else {
			// Si ya estaba tocando el suelo antes, ponemos a 0 la distancia recorrida.
			verticalDistanceCounter = 0f;
		}

		// Aplicamos la fuerza vertical.
		player.Move (transform.up * verticalV * Time.deltaTime);

		// Si el jugador está tocando el suelo.
		if (player.isGrounded) {
			// Verificamos que no se haya activado el salto.
			// si se ha activado, no aplicamos la fuerza, ya que puede dificultarlo.
			if (!jumped) {
				verticalV = Physics.gravity.y;
			}

			// Si la distancia vertical recorrida es menor que la indicada para que se reproduzca el sonido significa
			// que antes de este ciclo estabamos en caida libre reproduciremos el sonido al tocar el suelo.
			if (verticalDistanceCounter <= -minVerticalDistanceSound) {
				// Reproduzco el sonido
				audioSource.PlayOneShot (landingSound);
			}
		} else {
			// Si no se encuentra tocando el suelo, la velocidad vertical se irá incrementando con el tiempo.
			verticalV += Physics.gravity.y * Time.deltaTime;

			// Limitamosel el valor del desplazamiento vertical, para que se mantenga siempre dentro de unos márgenes ( estos se pueden / deben poner 
			// como variables públicas).
			verticalV = Mathf.Clamp (verticalV, -50f, 50f);

			// Si estamos en el aire, reseteamos la variable de salto.
			jumped = false;
		}

		// Guardamos el estado del grounded de este ciclo.
		isPreviouslyGrounded = player.isGrounded;
	}

	/// <summary>
	/// Realiza las acciones de salto del jugador.
	/// </summary>
	void Jump(){
		// Cambiamos el valor del movimiento vertical, para realizar el salto.
		if (player.isGrounded) {
			verticalV = jumpForce;
			jumped = true;
			// Reproducimos el sonido del salto.
			audioSource.PlayOneShot (jumpSound);
		} 
	}

	void OnControllerColliderHit(ControllerColliderHit hit){
		// Intentamos recuperar el rigidbody asociado al objeto colisionado.
		Rigidbody body = hit.rigidbody;

		// Si no hay rigidbody o este es kinemático, salgo del método.
		if (body == null || body.isKinematic) {
			return;
		}

		// Calculamos el vector de desplazamiento del objeto colisionado, eliminando la componente vertical.
		Vector3 pushDir = new Vector3 (hit.moveDirection.x, 
			                  0f,
			                  hit.moveDirection.z);
		// Aplicamos la velocidad a la que se moverá el objeto colisionado calculándola a partir de la 
		// dirección del impacto y la fuerza de empuje configurada.
		body.velocity = pushDir * pushForce;
	}

	/// <summary>
	/// Adds the recoil.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void AddRecoil (float amount){
		rotationVertical -= amount; 
	}

	// los comandos, son peticiones de ejecución de funciones en el servidor
	// los clientes pedirán mediante comandos, que se ejcuten acciones en el servidor
	[Command]
	/// <summary>
	/// Realiza la petición de disparo en el servidor
	/// </summary>
	/// <param name="direction">Direction.</param>
	void CmdShoot(Vector3 direction){
		//pedimos al servidor que todas las instancias en los clientes, disparen
		RpcShoot(direction);
	}

	//esto hace que el método permita ser invocado en los clientes, desde el servidor
	[ClientRpc]
	/// <summary>
	/// Realiza la petición de disparo en todas las instancias de los clientes
	/// </summary>
	/// <param name="direction">Direction.</param>
	void RpcShoot (Vector3 direction){
		//pedimos a todas las instancias de los clientes, que disparen
		BroadcastMessage ("CallShoot", direction);
	}

	[Command]
	/// <summary>
	/// Realiza la petición de recarga en el servidor
	/// </summary>
	void CmdReload(){
		BroadcastMessage ("CallReload");
	}
				
}
