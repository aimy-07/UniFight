using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NCMB;

public class ResultManager : MonoBehaviour {

	public static string result;
	[SerializeField] Text resultText;
	[SerializeField] Text countText;
	[SerializeField] Text percentText;
	[SerializeField] Image percentGraoh;

	string id;
	int winCount;
	int loseCount;
	int playCount;
	float winPercent;
	int newWinCount;
	int newLoseCount;
	int newPlayCount;
	float newWinPercent;
	float showWinPercent;

	int graphMoving = 0;

	[SerializeField] GameObject nowLoadingCanvas;
	[SerializeField] GameObject finishLoadingCanvas;
	[SerializeField] GameObject failedLoadingCanvas;



	void Start() {
		if (result != "WIN" && result != "LOSE") {
			result = "WIN";
		}
		if (result == "WIN") {
			resultText.text = "You Win!";
		} else if (result == "LOSE") {
			resultText.text = "You Lose...";
		}

		// 端末内保存情報からユーザー情報を抽出
		id = PlayerPrefs.GetString("ID");
		winCount = PlayerPrefs.GetInt("WinCount");
		loseCount = PlayerPrefs.GetInt("LoseCount");
		playCount = winCount + loseCount;
		if (playCount == 0) {
			winPercent = 0;
		} else {
			winPercent = (float)winCount / playCount;
		}
		// 更新後のユーザーデータ
		if (result == "WIN") {
			newWinCount = winCount + 1;
		} else if (result == "LOSE") {
			newLoseCount = loseCount + 1;
		}
		newPlayCount = newWinCount + newLoseCount;
		newWinPercent = (float)newWinCount / newPlayCount;

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
		if (graphMoving == 1) {
			showWinPercent += (newWinPercent - winPercent) * Time.deltaTime;
			percentGraoh.fillAmount = (float)showWinPercent;
			percentText.text = (showWinPercent * 100).ToString("f1") + "%";
		} else if (graphMoving == 2) {
			if (Input.GetMouseButtonDown(0)) {
				SetScore();
				graphMoving = 3;
			}
		} else if (graphMoving == 4) {
			if (Input.GetMouseButtonDown(0)) {
				ReturnToTitle();
			}
		}
	}

	public void SetCountText() {
		countText.text = winCount.ToString().PadLeft(3, '0') + "勝 / " + loseCount.ToString().PadLeft(3, '0') + "敗";
	}

	public void ChangeCountText() {
		countText.text = newWinCount.ToString().PadLeft(3, '0') + "勝 / " + newLoseCount.ToString().PadLeft(3, '0') + "敗";
	}

	public void GraphMoveStart() {
		showWinPercent = winPercent;
		percentGraoh.fillAmount = (float)showWinPercent;
		percentText.text = (showWinPercent * 100).ToString("f1") + "%";
		graphMoving = 1;
	}

	public void GraphMoveEnd() {
		showWinPercent = newWinPercent;
		percentGraoh.fillAmount = (float)showWinPercent;
		percentText.text = (showWinPercent * 100).ToString("f1") + "%";
		graphMoving = 2;
	}

	public void SetScore() {
		nowLoadingCanvas.SetActive(true);
		NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("DataBase");
		query.WhereEqualTo ("id", id);
        query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {
            if (e == null) {
				// データベースを更新
				objList[0]["winCount"] = newWinCount;
				objList[0]["loseCount"] = newLoseCount;
                objList[0].SaveAsync();
				PlayerPrefs.SetInt("WinCount", newWinCount);
				PlayerPrefs.SetInt("LoseCount", newLoseCount);
				nowLoadingCanvas.SetActive(false);
				finishLoadingCanvas.SetActive(true);
				graphMoving = 4;
            } else {
				// ネットワークに接続していない時
				nowLoadingCanvas.SetActive(false);
				failedLoadingCanvas.SetActive(true);
			}
        });	
	}

	public void ReturnToTitle() {
		SceneManager.LoadScene("Title");
	}
}
