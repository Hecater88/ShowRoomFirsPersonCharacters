using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkCustom : NetworkManager {
	//personaje elegido del array
	public int chosenCharacter = 0;
	//array con los posibles personajes a elegir
	public GameObject[] characters;

	//clase utilizada para transmitir al servidor el personaje elegido
	public class NetworkMessage : MessageBase {
		public int chosenClass;
	}

	//evento disparado cuando un jugador se conecta el servidor
	public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader){

		//recuperar el mensaje enviado con la solicitud de creación de personaje
		NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage> ();
		//recuperamos el id del personaje seleccionado
		int selectedClass = message.chosenClass;
		Debug.Log ("Se ha seleccionado el personaje" + selectedClass);

		//creamos una variable para almacenar el gameobject del player que será instanciado
		GameObject player;
		//recuperamos el transform de un punto de spawn aleatorio
		Transform startPos = GetStartPosition ();

		//si recuperamos una posición de spawn
		if (startPositions != null) {
			player = Instantiate (characters [selectedClass], startPos.position, startPos.rotation);
		} else {
			//si no hay punto de spawn recuperado, lo posicionamos en el centro del mundo
			player = Instantiate (characters [selectedClass], Vector3.zero, Quaternion.identity);
			Debug.LogWarning ("No hay puntos de espawn en este mapa!!!");
		}

		//finalmente hacemos el añadido del player a la conexion
		NetworkServer.AddPlayerForConnection (conn, player, playerControllerId);
	}

	//evento disparado cuando el cliente se conecta a un servidor
	public override void OnClientConnect(NetworkConnection conn){
		//creamos un mensaje con el personaje seleccionado, para transmitirselo al servidor al conectar
		NetworkMessage temp = new NetworkMessage ();
		temp.chosenClass = chosenCharacter;

		//envia unmensaje al realizar la conexion del player, con el personaje seleccionado
		ClientScene.AddPlayer (conn, 0, temp);
	}
}
