using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineBattleStart : MonoBehaviour {

	[SerializeField] GameObject mainCanvas;
	public AudioClip startBGM;
	public AudioClip battleBGM;
	public AudioClip[] startVoices;


	void BattleStart() {
		mainCanvas.SetActive(true);
		AudioSourceManager.PlayVOICE(startVoices[3]);
		AudioSourceManager.PlayBGM(battleBGM);
		OfflineManager.isPlaying = true;
		GameObject.FindWithTag("myPlayer").GetComponent<OfflinePlayerController>().joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
	}

	void LeftVoicePlay() {
		AudioClip clip = startVoices[OfflineCharaSet.GetCharaNum(GameObject.FindWithTag("myPlayer").GetComponent<OfflinePlayerController>().chara)];
		AudioSourceManager.PlayVOICE(clip);
	}

	void RightVoicePlay() {
		AudioClip clip = startVoices[OfflineCharaSet.GetCharaNum(GameObject.FindWithTag("enemyPlayer").GetComponent<OfflineEnemyController>().chara)];
		AudioSourceManager.PlayVOICE(clip);
	}

	void StartBGMPlay() {
		AudioSourceManager.PlayBGM(startBGM);
	}
}
