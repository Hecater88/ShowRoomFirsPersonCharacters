using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	//referencia al game menu
	public GameObject gameMenu;

	//almacenamos el nombre elegido por el jugador
	public string playerName{get;set;}

	//text que mostrara la puntuacion de la partida
	public Text scoreText;

	//texto que será mostrado en el scoreText;
	private string score;

	//instancia estática al GameManager
	public static GameManager GM;



	void Awake(){
		if (GM == null) {
			GM = GetComponent <GameManager> ();
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Activo o desactivo el game menu
	/// </summary>
	/// <param name="show">If set to <c>true</c> show.</param>
	public void GameMenu(bool show){
		gameMenu.SetActive (show);

		//si muestro el menú, actualizo la puntuación a mostrar
		if (show) {
			UpdateScore ();
		}
	}

	/// <summary>
	/// Actualizamos la puntuación revisando los valores de los jugadores
	/// </summary>
	public void UpdateScore(){
		//formateo la cabecera de la puntuacion
		// \t --> tabulacion
		// \n --> nueva linea
		score = "Player \t\t Kills \t Deaths\n";

		//recorro todos los jugadores de la partida, recuperando su información
		//y concatenándola a la variable score
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")){
			score += player.GetComponent<Score>().playerName + " \t\t " +
				player.GetComponent<Score>().kills + " \t " +
				player.GetComponent<Score>().deaths + "\n";
		}

		//actualizo la puntuación mostrada
		scoreText.text = score;
	}
}
