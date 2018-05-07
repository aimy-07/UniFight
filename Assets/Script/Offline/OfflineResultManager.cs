using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OfflineResultManager : MonoBehaviour {

	public static string result;
	[SerializeField] Text resultText;
	bool nextSceneOK;


	void Start() {
		if (result != "WIN" && result != "LOSE") {
			result = "WIN";
		}
		if (result == "WIN") {
			resultText.text = "You Win!";
		} else if (result == "LOSE") {
			resultText.text = "You Lose...";
		}

		// キャラのセット		
		int chara = PlayerPrefs.GetInt("chara", 0);
		int hairColor = PlayerPrefs.GetInt("hair", 1);
		int eyeColor = PlayerPrefs.GetInt("eye", 3);
		int costumeColor = PlayerPrefs.GetInt("costume", 6);
		
		GameObject charaObj = Instantiate(Resources.Load("CharaPrefabs/chara" + chara) as GameObject);
		charaObj.transform.position = new Vector3(-0.4f, 0, 0);
		charaObj.transform.eulerAngles = new Vector3(0, 180, 0);
		Destroy(charaObj.GetComponent<AnimationEventScript>());

		if (chara == 0) {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
		} else {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
		}
		switch(chara) {
			case 0:
			case 1:
			case 2:
			charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
			charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
			break;
			case 3:
			case 4:
			charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
			charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
			break;
			case 5:
			case 6:
			charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
			charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
			break;
		}

		if (result == "WIN") {
			charaObj.GetComponent<Animator>().SetTrigger("Win");
		} else if (result == "LOSE") {
			charaObj.GetComponent<Animator>().SetTrigger("Lose");
		}
	}

	void Update() {
		if (nextSceneOK) {
			if (Input.GetMouseButtonDown(0)) {
				ReturnToTitle();
			}
		}
	}

	void NextSceneOK() {
		nextSceneOK = true;
	}

	public void ReturnToTitle() {
		SceneManager.LoadScene("Title");
	}
}