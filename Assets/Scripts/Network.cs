using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Networking;

public class Network : MonoBehaviour {

	//referencia al menú de conexión
	public GameObject startMenu;
	//referencia al texto de IP a utilizar
	public Text ip;
	//referencia al texto del puerto a utilizar
	public Text port;
	//referencia al texto que contiene el nombre del jugador
	public Text playerName;

	//para identificar si es el servidor
	private bool isHost;
	//referencia al manager de red
	private NetworkManager networkManager;

	// Use this for initialization
	void Start () {
		//recuperamos la referencia al networkmanager
		networkManager = GetComponent<NetworkManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		//si no hay una conexión activa
		if (networkManager.client == null) {
			//mostramos el menú de conexión
			startMenu.SetActive (true);
		}
	}

	/// <summary>
	/// Crea un servidor en el puerto seleccionado	/// </summary>
	public void HostGame(){
		//indicamos que seremos el servidor
		isHost = true;
		//(int)port.text();
		//indicamos cual sera el puerto en el que se realizará la escucha
		networkManager.networkPort = int.Parse (port.text);
		//ocultamos el menu de conexión
		startMenu.SetActive(false);
		//iniciamos el servidor con los prámetros indicados
		networkManager.StartHost ();

		GameManager.GM.playerName = playerName.text;
	
	}
	/// <summary>
	/// Se une al servidor con la ip y el puerto especificados
	/// </summary>
	public void JoinGame(){
		//indicamos que no seremos el servidor
		isHost = false;
		//indicamos la dirección a la que nos conectaremos
		networkManager.networkAddress = ip.text;
		//indicamos el puerto de conexión
		networkManager.networkPort = int.Parse (port.text);
		//desactivamos el menú de conexión
		startMenu.SetActive(false);
		//inicia la coneción utilizando los parámetros de ip y port configurados
		networkManager.StartClient ();

		GameManager.GM.playerName = playerName.text;

	}
	/// <summary>
	///abandona el servidor si es cliente, cierra el servido si es host 
	/// </summary>
	public void LeaveGame(){
		if (isHost) {
			//llamamos al método que realiza la parada del servidor
			networkManager.StopHost ();
		} else {
			//llamamos al método que realiza la desconexión del servidor
			networkManager.StopClient ();
		}
		//mostramos el menú de conexión
		startMenu.SetActive (true);
	}
}
