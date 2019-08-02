using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPj : MonoBehaviour {

	public void SelectChamp(int number){
		RespawnManager.numberPj = number;
		SceneManager.LoadScene ("Training");
	}
	public void QuitGame(){
		Application.Quit ();
	}
	
	/// <summary>
	/// Cambia a la escena solicitada
	/// </summary>
	/// <param name="name">Name.</param>
	public void SceneLoad (string name){
		SceneManager.LoadScene (name);
	}
}
