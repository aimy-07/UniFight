using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaIdleScript : MonoBehaviour {

	[SerializeField] Animator[] animators;
	[SerializeField] AudioSource audio_voice;
	public AudioClip[] unityVoice;
	public AudioClip[] misakiVoice;
	public AudioClip[] yukoVoice;
	[SerializeField] Button voiceButton;
	float t;

	void Start () {
		
	}
	
	void Update () {
		if (t > 0) {
			t -= Time.deltaTime;
			voiceButton.interactable = false;
		} else {
			voiceButton.interactable = true;
		}
	}

	public void VoiceButton() {
		int rand = Random.Range(0, 6);
		int chara = PlayerPrefs.GetInt("chara", 0);
		for (int i = 0; i < 7; i++) {
			animators[i].SetTrigger(rand.ToString());
		}
		switch(OfflineCharaSet.GetCharaNum(chara)) {
			case 0:
				audio_voice.clip = unityVoice[rand];
				break;
			case 1:
				audio_voice.clip = misakiVoice[rand];
				break;
			case 2:
				audio_voice.clip = yukoVoice[rand];
				break;
		}
		audio_voice.Play();
		t = 1;
	}
}
