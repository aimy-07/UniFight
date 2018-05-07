using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour {

	void Awake()　{
		// PC向けビルドだったらサイズ変更
		if (Application.platform == RuntimePlatform.WindowsPlayer ||　Application.platform == RuntimePlatform.OSXPlayer)　{
			Screen.SetResolution(1280, 720, false);
		}
	}
	
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
		Invoke("toSelectCostume", 0.5f);
		GameObject.Find("SelectCostumeButtonFrame").GetComponent<Animation>().Play();
	}

	public void toMatchingButton() {
		Invoke("toMatching", 0.5f);
		GameObject.Find("StartButtonFrame").GetComponent<Animation>().Play();
	}

	public void toOfflinePlayButton() {
		Invoke("toOfflinePlay", 0.5f);
		GameObject.Find("OfflineStartButtonFrame").GetComponent<Animation>().Play();
	}

	public void toRecordButton() {
		Invoke("toRecord", 0.5f);
		GameObject.Find("RankingButtonFrame").GetComponent<Animation>().Play();
	}
}
