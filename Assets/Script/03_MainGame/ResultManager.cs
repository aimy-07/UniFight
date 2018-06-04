using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NCMB;

public class ResultManager : MonoBehaviour {

	public static string result;
	public static string enemyName;
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

	public AudioClip bgm;
	public AudioClip[] winVoice;
	public AudioClip[] loseVoice;



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
			newLoseCount = loseCount;
		} else if (result == "LOSE") {
			newLoseCount = loseCount + 1;
			newWinCount = winCount;
		}
		newPlayCount = newWinCount + newLoseCount;
		newWinPercent = (float)newWinCount / newPlayCount;

		// キャラのセット		
		int chara = PlayerPrefs.GetInt("chara", 0);
		int hairColor = PlayerPrefs.GetInt("hair", 0);
		int eyeColor = PlayerPrefs.GetInt("eye", 0);
		int costumeColor = PlayerPrefs.GetInt("costume", 0);
		
		GameObject charaObj = Instantiate(Resources.Load("CharaPrefabs/chara" + chara) as GameObject);
		charaObj.transform.position = new Vector3(-0.4f, 0, 0);
		charaObj.transform.eulerAngles = new Vector3(0, 180, 0);
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

		AudioSourceManager.PlayBGM(bgm);
		if (result == "WIN") {
			charaObj.GetComponent<Animator>().SetTrigger("Win");
			AudioSourceManager.PlayVOICE(winVoice[OfflineCharaSet.GetCharaNum(chara)]);
		} else if (result == "LOSE") {
			charaObj.GetComponent<Animator>().SetTrigger("Lose");
			AudioSourceManager.PlayVOICE(loseVoice[OfflineCharaSet.GetCharaNum(chara)]);
		}
	}

	void Update() {
		if (graphMoving == 1) {
			showWinPercent += (newWinPercent - winPercent) * Time.deltaTime;
			percentGraoh.fillAmount = (float)showWinPercent;
			percentText.text = (showWinPercent * 100).ToString("f1") + "%";
		}
	}

	void SetCountText() {
		countText.text = winCount.ToString().PadLeft(3, '0') + "勝 / " + loseCount.ToString().PadLeft(3, '0') + "敗";
	}

	void ChangeCountText() {
		countText.text = newWinCount.ToString().PadLeft(3, '0') + "勝 / " + newLoseCount.ToString().PadLeft(3, '0') + "敗";
		AudioSourceManager.PlaySE(0);
	}

	void GraphMoveStart() {
		showWinPercent = winPercent;
		percentGraoh.fillAmount = (float)showWinPercent;
		percentText.text = (showWinPercent * 100).ToString("f1") + "%";
		AudioSourceManager.PlaySE(7);
		graphMoving = 1;
	}

	void GraphMoveEnd() {
		showWinPercent = newWinPercent;
		percentGraoh.fillAmount = (float)showWinPercent;
		percentText.text = (showWinPercent * 100).ToString("f1") + "%";
		graphMoving = 2;
	}

	public void SetScore() {
		nowLoadingCanvas.SetActive(true);
		AudioSourceManager.PlaySE(2);
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
				AudioSourceManager.PlaySE(6);
            } else {
				// ネットワークに接続していない時
				nowLoadingCanvas.SetActive(false);
				failedLoadingCanvas.SetActive(true);
				AudioSourceManager.PlaySE(4);
			}
        });	
	}

	public void RetryButton() {
		Invoke("RetryLoading", 0.25f);
		GameObject.Find("RetryButton").GetComponent<Animation>().Play();
	}

	void RetryLoading() {
		SetScore();
	}

	public void BackTitle() {
		Invoke("toTitle", 0.25f);
		GameObject.Find("toTitleButton").GetComponent<Animation>().Play();
		AudioSourceManager.PlaySE(3);
	}

	public void FinishedBackTitle() {
		toTitle();
	}

	void toTitle() {
		UnityAdsScript.ShowAd();
		SceneManager.LoadScene("Title");
	}

	public void OnlineTweetButton() {
		string text1 = "【オンライン対戦3D格ゲー「ユニファイト」をプレイ中！】\n";
		string text2 = "";
		if (result == "WIN") {
			text2 = "オンライン対戦で" + enemyName.ToString() + "に勝った！\n";
		} else {
			text2 = "オンライン対戦で" + enemyName.ToString() + "に負けた...\n";
		}
		string text3 = "現在の実績　" + newWinCount + "勝" + newLoseCount + "敗！\n";
		// string url = "https://play.google.com/store/apps/details?id=net.ARCircle.DollyRun\n";
		string url = "";
		string hashtag = "#ユニファイト #unity";
		string message = text1 + text2 + text3 + url + hashtag;
		Application.OpenURL("http://twitter.com/intent/tweet?text=" + WWW.EscapeURL(message));
	}
}
