using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OfflineResultManager : MonoBehaviour {

	public static string result;
	[SerializeField] Text resultText;

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

	public void BackTitle() {
		UnityAdsScript.ShowAd();
		SceneManager.LoadScene("Title");
	}

	public void OfflineTweetButton() {
		string text1 = "【オンライン対戦3D格ゲー「ユニファイト」をプレイ中！】\n";
		string text2 = "";
		if (result == "WIN") {
			text2 = "オフライン練習対戦でNPCに勝った！\n";
		} else {
			text2 = "オフライン練習対戦でNPCに負けた...\n";
		}
		// string url = "https://play.google.com/store/apps/details?id=net.ARCircle.DollyRun\n";
		string url = "";
		string hashtag = "#ユニファイト #unity";
		string message = text1 + text2 + url + hashtag;
		Application.OpenURL("http://twitter.com/intent/tweet?text=" + WWW.EscapeURL(message));
	}
}