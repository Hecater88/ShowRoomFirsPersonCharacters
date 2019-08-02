using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMove : MonoBehaviour {

	public float amount = 0.02f;			// Multiplicador del desplazamiento del ratón aplicado.
	public float max = 0.06f;				// Desplazamiento máximo permitido por update, para limitar el desplazamiento del arma en giros rápidos.
	public float smooth = 6f;				// Para el suavizado del lerp.
	private Vector3 initialPosition;		// Variable para almacenar la posición inicial del arma.

	// Use this for initialization
	void Start () {
		// Guardamos la posición local inicial del arma.
		initialPosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		// El movimiento en el eje x e y será el opuesto al movimiento del ratón y reducido. 
		// mediante un multiplicador.
		float movementx = -Input.GetAxis ("Mouse X") * amount;
		float movementY = -Input.GetAxis ("Mouse Y") * amount;

		// Hacemos un clamp del movimiento, para evitar que supere los límites impuestos.
		movementx = Mathf.Clamp (movementx, -max, max);
		movementY = Mathf.Clamp (movementY, -max, max);

		// Calculamos cual será la posición final del arma.
		Vector3 finalPosition = new Vector3 (movementx, movementY, 0);

		// Realizamos el movimiento desde el origen hasta el destino, 
		transform.localPosition = Vector3.Lerp (transform.localPosition, 
			finalPosition + initialPosition,
			smooth * Time.deltaTime);
	}
}
