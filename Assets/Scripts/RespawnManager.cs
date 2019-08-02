using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour {
	static public RespawnManager RM;
	public GameObject LightTrace, RogueFlame, RevGo, ChuckAll,LaDiyMET, ThunderBold, GarNesha, JumperMod, EdgeinKami, SmashHer,XF20;
	public static int numberPj = 0;
	public Transform respawn;
	// Use this for initialization
	void Awake(){
		if(RM == null){
			RM = GetComponent<RespawnManager>();
		}
	}
	void Start () {
		if(numberPj == 0){
			 Instantiate(LightTrace, respawn.position, Quaternion.identity);
		}else if(numberPj == 1){
			Instantiate(RogueFlame, respawn.position, Quaternion.identity);
		}
		else if(numberPj == 2){
			Instantiate(RevGo, respawn.position, Quaternion.identity);
		}
		else if(numberPj == 3){
			Instantiate(ChuckAll, respawn.position, Quaternion.identity);
		}
		else if(numberPj == 4){
			Instantiate(LaDiyMET, respawn.position, Quaternion.identity);
		}
		else if(numberPj == 5){
			Instantiate(ThunderBold, respawn.position, Quaternion.identity);
		}
		else if(numberPj == 6){
			Instantiate(GarNesha, respawn.position, Quaternion.identity);
		}
		else if(numberPj == 7){
			Instantiate(JumperMod, respawn.position, Quaternion.identity);
		}
		else if(numberPj == 8){
			Instantiate(EdgeinKami, respawn.position, Quaternion.identity);
		}
		else if(numberPj == 9){
			Instantiate(SmashHer, respawn.position, Quaternion.identity);
		}
		else{
			Instantiate(XF20, respawn.position, Quaternion.identity);
		}
	}
	
	
}
