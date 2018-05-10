using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NCMB;
using System;
using System.Text.RegularExpressions;


public class InputNameManager : MonoBehaviour {

	[SerializeField] InputField inputField;
	[SerializeField] Button okButton;
	[SerializeField] GameObject errorText;
	AudioSource[] audio_systemSE = new AudioSource[2];

	void Start () {
		errorText.SetActive(false);
		audio_systemSE = GameObject.Find("audio").GetComponents<AudioSource>();
		// for (int i = 0; i < audio_systemSE.Length; i++) {
		// 	audio_systemSE[i].volume = 1.0f;
		// }
	}

	void Update() {
		if (CheckName(inputField.text)) {
			okButton.interactable = true;
		} else {
			okButton.interactable = false;
		}
	}

	public void OkButton() {
		errorText.SetActive(false);
		NCMBObject database = new NCMBObject("DataBase");
		string id = Guid.NewGuid().ToString();
		database["id"] = id;
		database["name"] = inputField.text;
		database["winCount"] = 0;
		database["loseCount"] = 0;
		database.SaveAsync((NCMBException e) => {
            if (e == null) {
				PlayerPrefs.SetString("ID", id);
				PlayerPrefs.SetString("PlayerName", inputField.text);
				PlayerPrefs.SetInt("WinCount", 0);
				PlayerPrefs.SetInt("LoseCount", 0);
				PlayerPrefs.SetInt("FirstOpen", 1);
				Invoke("toTitle", 0.5f);
				audio_systemSE[0].Play();
			} else {
				errorText.SetActive(true);
				audio_systemSE[1].Play();
			}
		});
		GameObject.Find("OkButton").GetComponent<Animation>().Play();
	}

	bool CheckName(string name) {
		if (name.Length <= 0 || name.Length > 15) {
			return false;
		}
		if (!Regex.IsMatch(name, @"[^a-zA-z0-9-_]")) {
			return true;
		}
		return false;
	}

	void toTitle() {
		SceneManager.LoadScene("Title");
	}
}
