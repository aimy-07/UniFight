using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineBattleStart : MonoBehaviour {

	[SerializeField] GameObject mainCanvas;
	[SerializeField] AudioSource audio_voice;
	public AudioClip[] startVoices;
	[SerializeField] AudioSource bgm;

	void Start() {
		//GetComponent<AudioSource>().volume = 1.0f;  //BGM
		//audio_voice.volume = 1.0f;  //VOICE
	}

	void BattleStart() {
		mainCanvas.SetActive(true);
		audio_voice.clip = startVoices[3];
		audio_voice.Play();
		bgm.Play();
		OfflineManager.isPlaying = true;
		GameObject.FindWithTag("myPlayer").GetComponent<OfflinePlayerController>().joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
	}

	void LeftVoicePlay() {
		audio_voice.clip = startVoices[OfflineCharaSet.GetCharaNum(GameObject.FindWithTag("myPlayer").GetComponent<OfflinePlayerController>().chara)];
		audio_voice.Play();
	}

	void RightVoicePlay() {
		audio_voice.clip = startVoices[OfflineCharaSet.GetCharaNum(GameObject.FindWithTag("enemyPlayer").GetComponent<OfflineEnemyController>().chara)];
		audio_voice.Play();
	}
}
