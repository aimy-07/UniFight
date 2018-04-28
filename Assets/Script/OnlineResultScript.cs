using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnlineResultScript : MonoBehaviour {

	public static string result;
	[SerializeField] Text resultText;
	[SerializeField] Text countText;
	[SerializeField] Text percentText;
	[SerializeField] Image percentGraoh;


	int prevOnlinePlayCount;
	int prevOnlineWinCount;
	float prevOnlineWinPercent;
	int nowOnlinePlayCount;
	int nowOnlineWinCount;
	float nowOnlineWinPercent;
	float showOnlineWinPercent;

	int graphMoving = 0;
	float t;



	void Start() {
		//prevOnlinePlayCount = PlayerPrefs.GetInt("OnlinePlayCount", 0);
		//prevOnlineWinCount = PlayerPrefs.GetInt("OnlineWinCount", 0);
		if (result != "WIN" && result != "LOSE") {
			result = "WIN";
		}
		prevOnlinePlayCount = 3;
		prevOnlineWinCount = 1;
		prevOnlineWinPercent = (float)prevOnlineWinCount / prevOnlinePlayCount;
		nowOnlinePlayCount = prevOnlinePlayCount + 1;
		if (result == "WIN") {
			nowOnlineWinCount = prevOnlineWinCount + 1;
			resultText.text = "You Win!";
		} else if (result == "LOSE") {
			nowOnlineWinCount = prevOnlineWinCount;
			resultText.text = "You Lose...";
		}
		nowOnlineWinPercent = (float)nowOnlineWinCount / nowOnlinePlayCount;
		//PlayerPrefs.SetInt("OnlinePlayCount", nowOnlinePlayCount);
		//PlayerPrefs.SetInt("OnlineWinCount", nowOnlineWinCount);
		
		countText.text = prevOnlineWinCount.ToString().PadLeft(3, '0') + "/" + prevOnlinePlayCount.ToString().PadLeft(3, '0');
		showOnlineWinPercent = prevOnlineWinPercent;
		percentGraoh.fillAmount = showOnlineWinPercent;
		percentText.text = (showOnlineWinPercent * 100).ToString("f1") + "%";
		
		int chara = PlayerPrefs.GetInt("chara", 0);
		int hairColor = PlayerPrefs.GetInt("hair", 1);
		int eyeColor = PlayerPrefs.GetInt("eye", 3);
		int costumeColor = PlayerPrefs.GetInt("costume", 6);
		
		GameObject charaObj = Instantiate(Resources.Load("CharaPrefabs/chara" + chara) as GameObject);
		charaObj.transform.position = new Vector3(-0.4f, 0, 0);
		charaObj.transform.eulerAngles = new Vector3(0, 180, 0);

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
		if (graphMoving == 1) {
			showOnlineWinPercent += (nowOnlineWinPercent - prevOnlineWinPercent) * Time.deltaTime;
			percentGraoh.fillAmount = showOnlineWinPercent;
			percentText.text = (showOnlineWinPercent * 100).ToString("f1") + "%";
		}
		t += Time.deltaTime;
		if (t > 3.5f) {
			if (Input.GetMouseButtonDown(0)) {
				SceneManager.LoadScene("Title");
			}
		}
	}

	public void SetCountText() {
		countText.text = prevOnlineWinCount.ToString().PadLeft(3, '0') + "/" + prevOnlinePlayCount.ToString().PadLeft(3, '0');
	}

	public void ChangeCountText() {
		countText.text = nowOnlineWinCount.ToString().PadLeft(3, '0') + "/" + nowOnlinePlayCount.ToString().PadLeft(3, '0');
	}

	public void GraphMoveStart() {
		showOnlineWinPercent = prevOnlineWinPercent;
		percentGraoh.fillAmount = showOnlineWinPercent;
		percentText.text = (showOnlineWinPercent * 100).ToString("f1") + "%";
		graphMoving = 1;
	}

	public void GraphMoveEnd() {
		showOnlineWinPercent = nowOnlineWinPercent;
		percentGraoh.fillAmount = showOnlineWinPercent;
		percentText.text = (showOnlineWinPercent * 100).ToString("f1") + "%";
		graphMoving = 2;
	}
}
