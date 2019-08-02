using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class Score : NetworkBehaviour {

	[SyncVar]
	public int kills = 0;
	[SyncVar]
	public int deaths = 0;
	[SyncVar]
	public string playerName;

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			//indicamos al servidor cual es el nombre del jugador
			CmdChangeName (GameManager.GM.playerName);
		}
	}

	/// <summary>
	/// Indica al servidor cual será el nombre del jugador
	/// </summary>
	/// <param name="name">Name.</param>
	[Command]
	void CmdChangeName(string name){
		playerName = name;
	}


}
