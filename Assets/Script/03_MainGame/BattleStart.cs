using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStart : MonoBehaviour {

	[SerializeField] PhotonManager photonManager;

	void CallBattleStart() {
		photonManager.BattleStart();
	}

	void CallLeftVoicePlay() {
		photonManager.LeftVoicePlay();
	}

	void CallRightVoicePlay() {
		photonManager.RightVoicePlay();
	}
}