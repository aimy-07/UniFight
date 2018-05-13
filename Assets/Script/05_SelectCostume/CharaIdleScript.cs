using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaIdleScript : MonoBehaviour {

	[SerializeField] Animator[] animators;
	public AudioClip[] unityVoice;
	public AudioClip[] misakiVoice;
	public AudioClip[] yukoVoice;
	[SerializeField] Button voiceButton;
	float t;

	
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
				AudioSourceManager.PlayVOICE(unityVoice[rand]);
				break;
			case 1:
				AudioSourceManager.PlayVOICE(misakiVoice[rand]);
				break;
			case 2:
				AudioSourceManager.PlayVOICE(yukoVoice[rand]);
				break;
		}
		t = 1;
	}
}
