using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Networking;

public class SkillAreaStunOnline : NetworkBehaviour {

	//tiempo que estunea la habilidad
	public float stunTime = 10f;
	// tiempo de enfriamiento de la habilidad
	public float coolDown = 30f;
	//temporizador interno para control del cooldown
	private float coolDownTimer;
	//radio de impacto de la habilidad
	public float impactRadius = 4f;
	//para mostrar u ocultar el dibujado del área de acción
	public bool drawGizmo;
	//referencia a la imagen de cooldown
	public Image coolDownImage;
	//referencia al sistema de partículas para el efecto de la habilidad
	public ParticleSystem skillEffect;

	private PlayerHealthOnline playerHealth;

	// Use this for initialization
	void Start () {
		playerHealth = GetComponent<PlayerHealthOnline> ();
	}
	
	// Update is called once per frame
	void Update () {
		//si no es el player local, no hacemos nada
		if (!isLocalPlayer) {
			return;
		}

		//si se pulsa el botón que activa la habilidad
		//el cooldown ha terminado
		//y el jugador no se encuentra stuneado
		if (Input.GetButtonDown ("Fire3") && coolDownTimer <= 0 && playerHealth.stunned <= 0) {
			//comando que ejecuta la habilidad en el servidor
			CmdUseSkill ();
		}
		//si el contador es mayor que 0 significa que está en cooldown
		if (coolDownTimer > 0) {
			coolDownTimer -= Time.deltaTime;
			//cambiamos el relleno de la cooldownimage en funcion del tiempo restante de cooldown
			coolDownImage.fillAmount = coolDownTimer / coolDown;
		} else {
			coolDownImage.fillAmount = 0f;
		}
	}

	[Command]
	/// <summary>
	/// Comando ejecutado en el servidor, que se encargará de verificar los jugadores stuneados
	/// </summary>
	public void CmdUseSkill (){
		//localizamos los objetos impactados por la habilidad
		Collider[] colls = Physics.OverlapSphere(transform.position, impactRadius);
		foreach (Collider col in colls) {
			//intento recuperar el componente playerhealthonline de la colision
			PlayerHealthOnline enemy = col.GetComponent<PlayerHealthOnline> ();

			//verificamos que el objeto impactado no sea el mismo, para que no se autostunee
			if (enemy != null && col.gameObject != gameObject) {
				//indicamos a todas las instancias del jugador impactado, que pase a estar stuneado
				enemy.RpcTakeStun (stunTime);
			}
		}

		RpcUseSkill ();
	}

	[ClientRpc]
	/// <summary>
	/// Actualización de efectos y cmd en todas las instancias
	/// </summary>
	public void RpcUseSkill(){
		//iniciamos el cooldown
		coolDownTimer = coolDown;
		SkillEffects ();
	}

	/// <summary>
	/// Realiza la ejecución de todos los efectos visuales y sonoros de la habilidad
	/// </summary>
	public void SkillEffects (){
		//mostramos el efecto de partículas
		skillEffect.Play ();
	}

	void OnDrawGizmos(){
		//para mostrar el área de efecto de la habilidad
		if (drawGizmo) {
			Gizmos.DrawSphere (transform.position, impactRadius);
		}
	}
}
