using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineBattleStart : MonoBehaviour {

	[SerializeField] GameObject mainCanvas;

	public void BattleStart() {
		mainCanvas.SetActive(true);
		OfflineManager.isPlaying = true;
		GameObject.FindWithTag("myPlayer").GetComponent<OfflinePlayerController>().joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
	}
}
