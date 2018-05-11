using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

	AudioSource[] audio_systemSE = new AudioSource[1];
	[SerializeField] AudioSource bgm;
	bool buttonPressed = false;


	
	void Start() {
		PhotonManager.phase = PhotonManager.PHASE.other;
		OfflineManager.isPlaying = false;

		int chara = PlayerPrefs.GetInt("chara", 0);
		int hairColor = PlayerPrefs.GetInt("hair", 1);
		int eyeColor = PlayerPrefs.GetInt("eye", 3);
		int costumeColor = PlayerPrefs.GetInt("costume", 6);
		
		GameObject charaObj = Instantiate(Resources.Load("CharaPrefabs/chara" + chara) as GameObject);
		charaObj.transform.position = new Vector3(-0.5f, 0, 0);
		charaObj.transform.eulerAngles = new Vector3(0, 140, 0);
		charaObj.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
		Destroy(charaObj.GetComponent<AnimationEventScript>());

		if (chara == 0) {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_unity" + costumeColor) as Material;
		} else {
			charaObj.transform.Find("costume").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/costume_school" + costumeColor) as Material;
		}
		switch(OfflineCharaSet.GetCharaNum(chara)) {
			case 0:
				charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + hairColor) as Material;
				charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/unity" + eyeColor) as Material;
				break;
			case 1:
				charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + hairColor) as Material;
				charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/misaki" + eyeColor) as Material;
				break;
			case 2:
				charaObj.transform.Find("hair").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + hairColor) as Material;
				charaObj.transform.Find("eye").gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load("Materials/yuko" + eyeColor) as Material;
				break;
		}

		audio_systemSE = GameObject.Find("audio").GetComponents<AudioSource>();
		// for (int i = 0; i < audio_systemSE.Length; i++) {
		// 	audio_systemSE[i].volume = 1.0f;
		// }
		//bgm.volume = 1.0f;  //BGM
	}

	void Update() {
		if (buttonPressed) {
			bgm.volume -= Time.deltaTime / 1.2f;
		}
	}

	void toSelectCostume() {
		SceneManager.LoadScene("SelectCostume");
	}

	void toMatching() {
		SceneManager.LoadScene("Main");
	}

	void toOfflinePlay() {
		SceneManager.LoadScene("OfflineMain");
	}

	void toRecord() {
		SceneManager.LoadScene("Record");
	}

	public void toSelectCostumeButton() {
		Invoke("toSelectCostume", 0.4f);
		GameObject.Find("SelectCostumeButtonFrame").GetComponent<Animation>().Play();
		audio_systemSE[0].Play();
		buttonPressed = true;
	}

	public void toMatchingButton() {
		Invoke("toMatching", 0.4f);
		GameObject.Find("StartButtonFrame").GetComponent<Animation>().Play();
		audio_systemSE[0].Play();
		buttonPressed = true;
	}

	public void toOfflinePlayButton() {
		Invoke("toOfflinePlay", 0.4f);
		GameObject.Find("OfflineStartButtonFrame").GetComponent<Animation>().Play();
		audio_systemSE[0].Play();
		buttonPressed = true;
	}

	public void toRecordButton() {
		Invoke("toRecord", 0.4f);
		GameObject.Find("RankingButtonFrame").GetComponent<Animation>().Play();
		audio_systemSE[0].Play();
		buttonPressed = true;
	}
}
