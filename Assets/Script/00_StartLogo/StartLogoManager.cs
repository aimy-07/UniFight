using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLogoManager : MonoBehaviour {

	float t = 0;

	void Awake()　{
		// PC向けビルドだったらサイズ変更
		if (Application.platform == RuntimePlatform.WindowsPlayer ||　Application.platform == RuntimePlatform.OSXPlayer)　{
			Screen.SetResolution(1280, 720, false);
		}
		if (Application.platform == RuntimePlatform.Android ||　Application.platform == RuntimePlatform.IPhonePlayer)　{
			Screen.orientation = ScreenOrientation.LandscapeLeft;
		}
	}

	void Start() {
		if (Application.platform == RuntimePlatform.Android ||　Application.platform == RuntimePlatform.IPhonePlayer)　{
			Screen.orientation = ScreenOrientation.AutoRotation;
		}
	}
	
	void Update () {
		t += Time.deltaTime;
		if (t > 2) {
			if (PlayerPrefs.GetInt("FirstOpen", 0) == 0) {
				SceneManager.LoadScene("InputName");
			} else if (PlayerPrefs.GetInt("FirstOpen", 0) == 1) {
				SceneManager.LoadScene("Title");
			}
		}
	}
}
