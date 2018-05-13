using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour {

	public static AudioSource audioBGM;
	static AudioSource[] audioSE;
	static AudioSource audioVOICE;

	public static float bgmVolume;
	public static float seVolume;
	public static float voiceVolume;
	public static float ownCharaVolume;
	public static float enemyCharaVolume;

	
	void Start () {
		bgmVolume = PlayerPrefs.GetFloat("BGM", 0.8f);
		seVolume = PlayerPrefs.GetFloat("SE", 0.8f);
		voiceVolume = PlayerPrefs.GetFloat("VOICE", 0.8f);
		ownCharaVolume = PlayerPrefs.GetFloat("OwnCharaSE", 0.8f);
		enemyCharaVolume = PlayerPrefs.GetFloat("EnemyCharaSE", 0.8f);
		audioBGM = transform.Find("audioBGM").gameObject.GetComponent<AudioSource>();
		audioBGM.volume = bgmVolume * 0.4f;
		audioSE = transform.Find("audioSE").gameObject.GetComponents<AudioSource>();
		for (int i = 0; i < audioSE.Length; i++) {
			audioSE[i].volume = seVolume;
		}
		audioVOICE = transform.Find("audioVOICE").gameObject.GetComponent<AudioSource>();
		audioVOICE.volume = voiceVolume;
	}
	
	void Update () {

	}

	public static void PlaySE(int num) {
		audioSE[num].Play();
	}

	// 0 : decision1
	// 1 : decision2
	// 2 : decision3
	// 3 : cancel
	// 4 : attention1
	// 5 : attention2
	// 6 : loadingFinish
	// 7 : movingGrapf

	public static void PlayBGM(AudioClip clip) {
		audioBGM.Stop();
		audioBGM.clip = clip;
		audioBGM.Play();
	}

	public static void PlayVOICE(AudioClip clip) {
		audioVOICE.clip = clip;
		audioVOICE.Play();
	}

	public static void ChangeVolume() {
		audioBGM.volume = bgmVolume * 0.4f;
		for (int i = 0; i < audioSE.Length; i++) {
			audioSE[i].volume = seVolume;
		}
		audioVOICE.volume = voiceVolume;
	}
}
