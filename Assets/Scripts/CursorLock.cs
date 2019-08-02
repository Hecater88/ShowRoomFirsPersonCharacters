using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Bloqueamos el cursor
		Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
		// Si se pulsa el botón escape.
		if (Input.GetButtonDown("Cancel")) {
			// Verificamos si el cursor está bloqueado.
			if (Cursor.lockState == CursorLockMode.Locked) {
				// Si está bloqueado lo desbloqueamos.
				Cursor.lockState = CursorLockMode.None;
			} else {
				// Si no está bloqueado, lo bloqueamos.
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

// Para no incluir este código en el ejecutable.
//#if UNITY_EDITOR
		// Para trabajar cómodamente en el inspector, haremos que se vuelva a bloquear el ratón 
		// cuando pulsemos con el ratón en la pestaña de game.
		if (Input.GetKeyUp (KeyCode.Mouse0)) {
			Cursor.lockState = CursorLockMode.Locked;
		}
//#endif
	}
}
