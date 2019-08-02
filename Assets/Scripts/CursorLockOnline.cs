using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class CursorLockOnline : NetworkBehaviour {
	//referencia al controller, para activarlo/desactivarlo
	private FPSControllerOnline controller;
	//booleana para controlar cuando está bloqueado el cursor
	private bool isLocked = true;

	// Use this for initialization
	void Start () {
		//si no soy local player, no hago nada
		if (!isLocalPlayer){
			return;
		}

		controller = GetComponent<FPSControllerOnline>();
		//hacemos el ocultado del cursor
		Cursor.visible = false;
		//bloquemoas el cursor
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		//si no es el player local
		if (!isLocalPlayer) {
			return;
		}
		if (Input.GetButtonDown ("Cancel")) {
			//alternamos la booleana de bloqueo
			isLocked = !isLocked;

			//si el cursor está bloqueado
			if (isLocked) {
				//iniciamos la corrutina que bloqueará el cursor al final del frame
				StartCoroutine (DisableCursor ());
				//permito mover al player
				controller.inMenu = false;
				//oculto el menu
				GameManager.GM.GameMenu (false);
			//si el cursor no está bloqueado
			} else {
				//iniciamos la corrutina que liberará el cursor al final del frame
				StartCoroutine (EnableCursor());
				//impido que el player se pueda mover
				controller.inMenu = true;
				//muestro el menú
				GameManager.GM.GameMenu (true);
			}
		}

		#if UNITY_EDITOR
		// Para trabajar cómodamente en el inspector, haremos que se vuelva a bloquear el ratón 
		// cuando pulsemos con el ratón en la pestaña de game.
		if (Input.GetKeyUp (KeyCode.Mouse0)) {
			Cursor.lockState = CursorLockMode.Locked;
		}
		#endif
	}

	/// <summary>
	/// Corrutina para desactivar el cursor al final del frame
	/// </summary>
	/// <returns>The cursor.</returns>
	IEnumerator DisableCursor(){
		//para evitar solapamiento con otras instrucciones que puedan evitar el bloqueo dle raton,
		//esperaremos para llevarlo a cabo, justo al final del dibujado del frame
		yield return new WaitForEndOfFrame ();
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	/// <summary>
	/// Corrutina para activar el cursor al final del frame
	/// </summary>
	/// <returns>The cursor.</returns>
	IEnumerator EnableCursor(){
		yield return new WaitForEndOfFrame ();
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
}
