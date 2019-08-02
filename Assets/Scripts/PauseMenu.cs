using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
public class PauseMenu : MonoBehaviour {
	//Booleana para controlar si el juego está pausado
	public static bool GameIsPaused = false;
	//Canvas del pauseMenu
	public GameObject pauseMenuUI;
	// Update is called once per frame
	void Update () {
		//Si se pulsa escape
		if (Input.GetKeyDown (KeyCode.Escape)) {
			//si el juego ya está en pausa
			if (GameIsPaused) {
				//podemos reanudar la partida
				Resume ();
			} else {
				//pausamos la partida
				Pause();
			}
		}
	}

	/// <summary>
	/// Reanuda el juego
	/// </summary>
	public void Resume(){
		//desactivamos el canvas del pausemenu
		pauseMenuUI.SetActive (false);
		//hacemos que el tiempo sea normal
		Time.timeScale = 1f;
		GameIsPaused = false;
	}
	/// <summary>
	/// Pausa el juego
	/// </summary>
	void Pause(){
		//activamos el canvas del pausemenu
		pauseMenuUI.SetActive (true);
		//hacemos que el tiempo se pare
		Time.timeScale = 0f;
		GameIsPaused = true;
	}
	/// <summary>
	/// Loads the menu.
	/// </summary>
	public  void LoadMenu(){
		////hacemos que el tiempo sea normal
		Time.timeScale = 1f;
		//y cargamos la escena MainMenu
		SceneManager.LoadScene ("MainMenu2");
	}
	/// <summary>
	/// Quits the game.
	/// </summary>
	public void QuitGame(){
		Application.Quit ();
	}

	/// <summary>
	/// Cambia a la escena solicitada
	/// </summary>
	/// <param name="name">Name.</param>
	public void SceneLoad (string name){
		////hacemos que el tiempo sea normal
		Time.timeScale = 1f;
		SceneManager.LoadScene (name);
	}
}

