using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

using UnityEngine.UI;

public class PlayerHealthOnline : NetworkBehaviour {

	// vida máxima del personaje
	public float maxHealth = 100f;

	// referencia al componente player
	Player player;

	// vida actual
	public float health;

	//texto en pantalla donde mostrará la vida actual
	public Text healthText;

	//imagen utilizada para representr el daño
	public Image damageImage;
	//color al que cambiara la imagen cuando se reciba daño
	public Color damageColor = new Color (1f, 0f, 0f, 0.1f);
	//booleana para controlar si se ha recibido daño
	public bool damaged;

	//tiempo que está stuneado jugador
	public float stunned;
	//referencia al texto que activeraemos cuando el jugador se encuentre stuneado
	public Text stunnedText;
	//referencia al fpscontrolleronline
	private FPSControllerOnline fpsController;

	//referencia al networkAnimator
	Animator anim;

	// Use this for initialization
	void Start () {
		player = GetComponent<Player> ();
		anim = GetComponent<Animator> ();
		fpsController = GetComponent<FPSControllerOnline> ();
	}

	void Update(){
		//si se trata del jugador local
		if (isLocalPlayer) {
			//y está stuneado
			if (stunned > 0) {
				//modificamos el texto a mostrar en pantalla con el tiempo restante de stun
				//quitamos la precion decimal y luego lo convierto en texto
				stunnedText.text = "Stunned " + ((int)stunned).ToString ();
				//desactivamos el control del jugador mientras dure el stun
				fpsController.inMenu = true;
				//reducimos el tiempo de stun
				stunned -= Time.deltaTime;
			} else {
				stunnedText.text = "";
				//devuelvo el control al jugador
				fpsController.inMenu = false;
			}
		}

		//si se ha recibido daño
		if (damaged) {
			//hacemos que el color de la imaén sea el definido para el daño
			damageImage.color = damageColor;
		} else {
			//si no se está recibiendo daño, lerpeamos el color hacia un color limpio
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, Time.deltaTime);
		}
		//tras la verificación de si se ha dañado y mostrar el color, reseteo la bool de daño
		damaged = false;

	}

	void OnEnable() {
		// en el momento de reactivar al jugador, lo activo con la vida máxima
		health = maxHealth;
		//inicializamos el mostrado de la vida
		healthText.text = health.ToString();
		//reseteamos el color de la imagen de daño, para que no sea visible al hacer respawn
		damageImage.color = Color.clear;
		//si aun no hemos recuperado la referencia al animator
		if(anim != null){
		//en cuanto se deshabilita el enemigo, lo ponemos en pie
			anim.SetTrigger ("Alive");
		}
	}


	// el siguiente código será solo ejecutado en el servidor
	[Server]
	/// <summary>
	/// Aplica el daño recibido como parámetro, devuelve true si el jugador a muerto a causa del impacto
	/// </summary>
	/// <returns><c>true</c>, if damage was taken, <c>false</c> otherwise.</returns>
	/// <param name="damage">Damage.</param>
	public bool TakeDamage (float damage){
		// consideramos inicialmente que el daño no mata al jugador
		bool died = false;

		// verificamos la vida en la instancia del servidor, si esta es menor o igual a 0 antes de aplicar el daño, significa que ya está muerto y salimos del metodo devolviendo false
		if (health <= 0) {
			return died;
		}

		// aplico el daño recibido
		health -= damage;

		// verifico si ha muerto a causa del impacto
		died = health <= 0;

		if (died) {
			//incrementamos el número de muertes del jugador
			GetComponent<Score> ().deaths++;
		}

		// si ha muerto por el impacto, informo a todas las instancias del jugador, para que mueran
		//y si no muere, actualiza su vida
		//Transmite los datos a los demas
		RpcTakeDamage (died, health);

		// devuelvo si el jugador ha muerto tras el impacto
		return died;
	}

	[ClientRpc]
	/// <summary>
	/// Informa a todas las instancias si el jugador ha muerto
	/// </summary>
	/// <param name="died">If set to <c>true</c> died.</param>
	void RpcTakeDamage (bool died, float actualHealth){
		if (died) {
			anim.SetTrigger ("Dead");
			// ejecute las acciones de muerte
			player.Die ();
		} else {
			//si el jugador no ha muerto con el impacto
			//el servidor indicará la vida que le queda
			health = actualHealth;
			//actualiza la vida mostrada
			healthText.text = health.ToString ();
			//indicamos que ha recibido daño
			damaged = true;
		}
	}

	[ClientRpc]
	/// <summary>
	/// Aplica el efecto del stun a todas las instancias de los clientes
	/// </summary>
	/// <param name="stunTime">Stun time.</param>
	public void RpcTakeStun(float stunTime){
		//asigno el tiempo de stun
		stunned = stunTime;
	}
}
