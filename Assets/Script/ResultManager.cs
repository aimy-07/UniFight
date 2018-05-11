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

	[SerializeField] AudioSource bgm;
	bool buttonPressed = false;
	AudioSource[] audio_systemSE = new AudioSource[6];
	[SerializeField] AudioSource audio_voice;
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
		// bgm.volume = 1.0f;  //BGM
		// audio_voice.volume = 1.0f;  //VOICE

		if (result == "WIN") {
			charaObj.GetComponent<Animator>().SetTrigger("Win");
			audio_voice.clip = winVoice[OfflineCharaSet.GetCharaNum(chara)];
			audio_voice.Play();
		} else if (result == "LOSE") {
			charaObj.GetComponent<Animator>().SetTrigger("Lose");
			audio_voice.clip = loseVoice[OfflineCharaSet.GetCharaNum(chara)];
			audio_voice.Play();
		}
	}

	void Update() {
		if (graphMoving == 1) {
			showWinPercent += (newWinPercent - winPercent) * Time.deltaTime;
			percentGraoh.fillAmount = (float)showWinPercent;
			percentText.text = (showWinPercent * 100).ToString("f1") + "%";
		}
 		if (buttonPressed) {
			bgm.volume -= Time.deltaTime * 1.2f;
		}
	}

	void SetCountText() {
		countText.text = winCount.ToString().PadLeft(3, '0') + "勝 / " + loseCount.ToString().PadLeft(3, '0') + "敗";
	}

	void ChangeCountText() {
		countText.text = newWinCount.ToString().PadLeft(3, '0') + "勝 / " + newLoseCount.ToString().PadLeft(3, '0') + "敗";
		audio_systemSE[0].Play();
	}

	void GraphMoveStart() {
		showWinPercent = winPercent;
		percentGraoh.fillAmount = (float)showWinPercent;
		percentText.text = (showWinPercent * 100).ToString("f1") + "%";
		audio_systemSE[1].Play();
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
		audio_systemSE[3].Play();
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
				audio_systemSE[5].Play();
            } else {
				// ネットワークに接続していない時
				nowLoadingCanvas.SetActive(false);
				failedLoadingCanvas.SetActive(true);
				audio_systemSE[2].Play();
			}
        });	
	}

	public void BackTitle() {
		Invoke("toTitle", 0.3f);
		audio_systemSE[4].Play();
		buttonPressed = true;
	}

	public void FinishedBackTitle() {
		Invoke("toTitle", 0.3f);
		buttonPressed = true;
	}

	void toTitle() {
		SceneManager.LoadScene("Title");
	}

	public void OnlineTweetButton() {
		string text1 = "【オンライン対戦3D格ゲー「ユニファイト」をプレイ中！】\n";
		string text2 = "";
		if (result == "WIN") {
			text2 = "オンライン対戦で" + enemyName.ToString() + "に勝った！\n";
		} else {
			text2 = "オフライン練習対戦で" + enemyName.ToString() + "に負けた...\n";
		}
		string text3 = "現在の実績　" + newWinCount + "勝" + newLoseCount + "敗！\n";
		// string url = "https://play.google.com/store/apps/details?id=net.ARCircle.DollyRun\n";
		string url = "";
		string hashtag = "#ユニファイト #unity";
		string message = text1 + text2 + text3 + url + hashtag;
		Application.OpenURL("http://twitter.com/intent/tweet?text=" + WWW.EscapeURL(message));
	}
}
