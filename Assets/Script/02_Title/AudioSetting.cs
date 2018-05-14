using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour {

	bool isOpen = false;
	[SerializeField] Animator settingCanvasAnim;
	[SerializeField] Canvas howToPlayButtonCanvas;
	[SerializeField] Animator settingButtonCanvasAnim;

	[SerializeField] Slider bgmSlider;
	[SerializeField] Slider seSlider;
	[SerializeField] Slider voiceSlider;
	[SerializeField] Slider ownCharaSlider;
	[SerializeField] Slider enemyCharaSlider;


	void Start () {
		bgmSlider.value = AudioSourceManager.bgmVolume;
		seSlider.value = AudioSourceManager.seVolume;
		voiceSlider.value = AudioSourceManager.voiceVolume;
		ownCharaSlider.value = AudioSourceManager.ownCharaVolume;
		enemyCharaSlider.value = AudioSourceManager.enemyCharaVolume;
	}
	
	void Update () {
		if (isOpen) {
			AudioSourceManager.bgmVolume = bgmSlider.value;
			AudioSourceManager.seVolume = seSlider.value;
			AudioSourceManager.voiceVolume = voiceSlider.value;
			AudioSourceManager.ownCharaVolume = ownCharaSlider.value;
			AudioSourceManager.enemyCharaVolume = enemyCharaSlider.value;
			AudioSourceManager.ChangeVolume();
			PlayerPrefs.SetFloat("BGM", bgmSlider.value);
			PlayerPrefs.SetFloat("SE", bgmSlider.value);
			PlayerPrefs.SetFloat("VOICE", bgmSlider.value);
			PlayerPrefs.SetFloat("OwnCharaSE", bgmSlider.value);
			PlayerPrefs.SetFloat("EnemyCharaSE", bgmSlider.value);

			howToPlayButtonCanvas.enabled = false;
		} else {
			howToPlayButtonCanvas.enabled = true;
		}
	}

	public void SettingButton() {
		AudioSourceManager.PlaySE(2);
		if (isOpen) {
			settingCanvasAnim.SetTrigger("Close");
			settingButtonCanvasAnim.SetTrigger("Close");
		} else {
			settingCanvasAnim.SetTrigger("Open");
			settingButtonCanvasAnim.SetTrigger("Open");
		}
	}

	void IsOpenChange() {
		isOpen = !isOpen;
	}
}
