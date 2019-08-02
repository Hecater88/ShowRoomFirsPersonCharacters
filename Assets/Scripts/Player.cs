using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// para metodos y propiedades de networking
using UnityEngine.Networking;

// para uso de eventos
using UnityEngine.Events;

// heredará de networkBehaviour en ludar de monoBehaviour
public class Player : NetworkBehaviour {

	// evento donde activaremos / desactivaremos componentes compartidos para los estados local y remoto al activar el objeto
	[SerializeField]
	UnityEvent onSharedEnabled;

	// evento donde activaremos / desactivaremos componentes para los estados locales al activar el objeto
	[SerializeField]
	UnityEvent onLocalEnabled;

	// evento donde activaremos / desactivaremos componentes para los estados remotos al activar el objeto
	[SerializeField]
	UnityEvent onRemoteEnabled;

	// referencia al maincamera
	GameObject mainCamera;

	// evento donde activaremos / desactivaremos componentes compartidos para los estados local y remoto al desactivar el objeto
	[SerializeField]
	UnityEvent onSharedDisable;

	// evento donde activaremos / desactivaremos componentes para los estados locales al desactivar el objeto
	[SerializeField]
	UnityEvent onLocalDisabled;

	// evento donde activaremos / desactivaremos componentes para los estados remotos al desactivar el objeto
	[SerializeField]
	UnityEvent onRemoteDisabled;

	// tiempo que tardará el jugador en respawnear
	public float respawnTime = 5f;

	// Use this for initialization
	void Start () {
		
		if (isLocalPlayer) {
			// recupero la referencia al maincamera
			mainCamera = Camera.main.gameObject;
		}
		//realizamos la activación del player
		EnablePlayer ();

	}
	
	/// <summary>
	/// Activación del player
	/// </summary>
	void EnablePlayer () {
		
		// invocamos el evento compartido
		onSharedEnabled.Invoke ();

		if (isLocalPlayer) {
			// desactivamos la cámara inicial de la escena, mientras el jugador está activo
			mainCamera.SetActive (false);
			// invocamos el evento local
			onLocalEnabled.Invoke ();
		} else {
			// invocamos el evento remoto
			onRemoteEnabled.Invoke ();
		}
	}

	void OnDisable(){
		DisablePlayer ();
	}

	/// <summary>
	/// Método que llevará acabo la desactivación del jugador
	/// </summary>
	void DisablePlayer () {
		// llama al evento que realiza las acciones que configuremos para local y remoto
		onSharedDisable.Invoke ();

		if (isLocalPlayer) {
			// activamos la cámara general de la escena mientras el jugador está muertp
			mainCamera.SetActive (true);
			// llamamos al evento que realiza las acciones para la desactivación local
			onLocalDisabled.Invoke ();
		} else {
			// llamamos al evento que realiza las acciones para la desactivación remota
			onRemoteDisabled.Invoke ();
		}
	}

	/// <summary>
	/// Método que llamaremos cuando el jugador muera
	/// </summary>
	public void Die (){
		// llamamos al método que desactiva al jugador
		DisablePlayer ();
		// invocamos el método de respawn, con un delay
		Invoke ("Respawn", respawnTime);
	}

	/// <summary>
	/// Método que realiza la Reactivación del jugador
	/// </summary>
	void Respawn(){
		// si el jugador es local
		if (isLocalPlayer) {
			// pedimos al networkmanager un nuevo punto de respawn
			Transform spawm = NetworkManager.singleton.GetStartPosition ();
			// movemos al player a la posición de spawn obtenida
			transform.position = spawm.position;
			// hacemos que mire en la misma dirección que el punto de spawn
			transform.rotation = spawm.rotation;
		}
		// lamamos al método de activación del jugador
		EnablePlayer ();
	}

}
