using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	Transform myPlayer;
	Transform enemyPlayer;
	bool isReady = false;
	
	void Update () {
		if (!isReady) {
			if (GameObject.FindWithTag("myPlayer") != null && GameObject.FindWithTag("enemyPlayer") != null) {
				myPlayer = GameObject.FindWithTag("myPlayer").transform;
				enemyPlayer = GameObject.FindWithTag("enemyPlayer").transform;
				isReady = true;
			}
		}
		if (PhotonManager.phase == PhotonManager.PHASE.isPlaying || OfflineManager.isPlaying) {
			float x = (myPlayer.transform.position.x + enemyPlayer.transform.position.x) / 2;
			transform.position = new Vector3(x, 1.6f, -10);
		}
	}
}
