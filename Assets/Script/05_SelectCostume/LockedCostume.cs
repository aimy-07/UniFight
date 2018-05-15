using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class LockedCostume : MonoBehaviour {

	public static int set_black_locked;
	public static int costumeColor_default_locked;

	int lockState = -1;
	[SerializeField] SelectCostumeManager selectCostumeManager;

	public GameObject[] movieButtons;
	public GameObject[] getItemButtons;
	[SerializeField] GameObject hairColorLockedImg;
	[SerializeField] GameObject eyeColorLockedImg;
	[SerializeField] GameObject costumeColorLockedImg;



	void Start () {
		set_black_locked = PlayerPrefs.GetInt("Set_black_locked", 0);
		costumeColor_default_locked = PlayerPrefs.GetInt("CostumeColor_default_locked", 0);
	}
	
	void Update () {

	}


	public void LockedHairColor(int hairColor, int eyeColor, int costumeColor) {
		bool locked = false;
		if (hairColor == 6 && set_black_locked == 0) {
			locked = true;
			hairColorLockedImg.SetActive(true);
			lockState = 0;
			MovieButtonSetActive(lockState);
		}
		if (!locked) {
			hairColorLockedImg.SetActive(false);
			CheckOtherLockedItem(hairColor, eyeColor, costumeColor);
		}
	}

	public void LockedEyeColor(int hairColor, int eyeColor, int costumeColor) {
		bool locked = false;
		if (eyeColor == 6 && set_black_locked == 0) {
			locked = true;
			eyeColorLockedImg.SetActive(true);
			lockState = 0;
			MovieButtonSetActive(lockState);
		}
		if (!locked) {
			eyeColorLockedImg.SetActive(false);
			CheckOtherLockedItem(hairColor, eyeColor, costumeColor);
		}
	}

	public void LockedCostumeColor(int hairColor, int eyeColor, int costumeColor) {
		bool locked = false;
		if (costumeColor == 5 && set_black_locked == 0) {
			locked = true;
			costumeColorLockedImg.SetActive(true);
			lockState = 0;
			MovieButtonSetActive(lockState);
		} else if (costumeColor == 6 && costumeColor_default_locked == 0) {
			locked = true;
			costumeColorLockedImg.SetActive(true);
			lockState = 1;
			MovieButtonSetActive(lockState);
		}
		if (!locked) {
			costumeColorLockedImg.SetActive(false);
			CheckOtherLockedItem(hairColor, eyeColor, costumeColor);
		}
	}

	void MovieButtonSetActive(int num) {
		for (int i = 0; i < movieButtons.Length; i++) {
			movieButtons[i].SetActive(false);
		}
		if (num != -1) {
			movieButtons[num].SetActive(true);
		}
	}

	void CheckOtherLockedItem(int hairColor, int eyeColor, int costumeColor) {
		if (((hairColor == 6 || eyeColor == 6 || costumeColor == 5)) && set_black_locked == 0) {
			lockState = 0;
		} else if (costumeColor == 6 && costumeColor_default_locked == 0) {
			lockState = 1;
		} else {
			lockState = -1;
		}
		MovieButtonSetActive(lockState);
	}
	
	public void ShowRewardedAd() {
		if (Advertisement.IsReady("rewardedVideo")){
			ShowOptions options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
	}

	void HandleShowResult(ShowResult result) {
		switch (result) {
		case ShowResult.Finished:
			//再生完了
			Debug.Log ("The ad was successfully shown.");
			switch (lockState) {
				case 0:
					set_black_locked = 1;
					PlayerPrefs.SetInt("Set_black_locked", 1);
					SelectCostumeManager.hairColor = 6;
					SelectCostumeManager.eyeColor = 6;
					SelectCostumeManager.costumeColor = 5;
					selectCostumeManager.SetHairColor();
					selectCostumeManager.SetEyeColor();
					selectCostumeManager.SetCostumeColor();
					getItemButtons[0].SetActive(true);
					break;
				case 1:
					costumeColor_default_locked = 1;
					PlayerPrefs.SetInt("CostumeColor_default_locked", 1);
					selectCostumeManager.SetCostumeColor();
					getItemButtons[1].SetActive(true);
					break;
			}
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			// スキップ
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			//　失敗
			break;
		}
	}

	public void CloseResult() {
		getItemButtons[0].SetActive(false);
		getItemButtons[1].SetActive(false);
	}
}
