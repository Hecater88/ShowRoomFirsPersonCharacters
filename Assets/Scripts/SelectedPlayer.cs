using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class SelectedPlayer : MonoBehaviour {

	/// <summary>
	/// Asigna el id del personaje seleccionado como elegido al networkmanager
	/// </summary>
	/// <param name="Player">Player.</param>
	public void PickPlayer (int Player){
		NetworkManager.singleton.GetComponent<NetworkCustom> ().chosenCharacter = Player;
	}

}
