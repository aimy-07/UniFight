using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OfflineManager : MonoBehaviour {

	public static bool isPlaying = false;
	float apTimer;
	public static bool apPlus;

	[SerializeField] GameObject stopCanvas;
	[SerializeField] GameObject mainCanvas;
	
	[SerializeField] GameObject resultCanvas;
	[SerializeField] Text resultText;
	GameObject winPlayer;
	GameObject losePlayer;
	string winPlayerTag;
	string losePlayerTag;


	public AudioClip[] endBGMs;
	public AudioClip[] attackVoice;
	public AudioClip[] damageVoice;
	public AudioClip[] winVoice;



	void Start() {
		isPlaying = false;
		PhotonManager.phase = PhotonManager.PHASE.other;
		apPlus = false;

		stopCanvas.SetActive(false);
		mainCanvas.SetActive(false);
		resultCanvas.SetActive(false);
		OfflineResultManager.result = "";

		// アニメーションによって自動的にバトルが開始する
    }
	
	void Update () {
		if (isPlaying) {
			if (apTimer < PhotonManager.AUTE_AP_INTERVAL) {
				apTimer += Time.deltaTime;
				apPlus = false;
			} else {
				apTimer = 0;
				apPlus = true;
			}
		}
	}

	// どちらかのHPが0になったら呼ばれる
	void BattleEnd(string losePlayerTag) {
		isPlaying = false;
		AudioSourceManager.audioBGM.Stop();
		if (losePlayerTag == "myPlayer") {
			winPlayerTag = "enemyPlayer";
		} else {
			winPlayerTag = "myPlayer";
		}
		GameObject cam = GameObject.Find("Main Camera");
		GameObject winPlayer = GameObject.FindWithTag(winPlayerTag);
		GameObject losePlayer = GameObject.FindWithTag(losePlayerTag);
		float cam_x;
		if (winPlayerTag == "myPlayer") {
			cam_x = winPlayer.transform.transform.position.x;
			AudioClip clip = attackVoice[OfflineCharaSet.GetCharaNum(winPlayer.GetComponent<OfflinePlayerController>().chara)];
			AudioSourceManager.PlayVOICE(clip);
		} else {
			if (losePlayer.transform.rotation.y > 0) {
				cam_x = losePlayer.transform.transform.position.x - 0.4f;
			} else {
				cam_x = losePlayer.transform.transform.position.x + 0.4f;
			}
			AudioClip clip = damageVoice[OfflineCharaSet.GetCharaNum(losePlayer.GetComponent<OfflinePlayerController>().chara)];
			AudioSourceManager.PlayVOICE(clip);
		}
		cam.transform.position = new Vector3(cam_x, 1.5f, -10);
		cam.transform.eulerAngles = new Vector3(6, 0, 0);
		cam.GetComponent<Camera>().orthographicSize = 0.6f;
		Time.timeScale = 0.5f;
		mainCanvas.SetActive(false);
		Invoke("ShowResult", 0.8f);
	}

	void ShowResult() {
		Time.timeScale = 1f;
		GameObject winPlayer = GameObject.FindWithTag(winPlayerTag);
		winPlayer.transform.Find("UTC_Default").gameObject.GetComponent<Animator>().SetTrigger("Win");
		winPlayer.transform.eulerAngles = new Vector3(0, 180, 0);
		resultCanvas.SetActive(true);
		if (winPlayerTag == "myPlayer") {
			resultText.text = "You Win!";
			OfflineResultManager.result = "WIN";
			AudioClip clip = winVoice[OfflineCharaSet.GetCharaNum(winPlayer.GetComponent<OfflinePlayerController>().chara)];
			AudioSourceManager.PlayVOICE(clip);
			AudioSourceManager.audioBGM.loop = false;
			AudioSourceManager.PlayBGM(endBGMs[0]);
		} else {
			resultText.text = "You Lose...";
			OfflineResultManager.result = "LOSE";
			AudioSourceManager.audioBGM.loop = false;
			AudioSourceManager.PlayBGM(endBGMs[1]);
		}
	}

	//ボタンを押したら一時停止
	public void StopGame() {
		isPlaying = false;
		Time.timeScale = 0;
		AudioSourceManager.PlaySE(4);
		AudioSourceManager.audioBGM.Pause();
		stopCanvas.SetActive(true);
	}

	//ボタンを押したらリザルトシーンに遷移
	public void GotoResult() {
		Invoke("toResult", 0.25f);
		GameObject.Find("NextButton").GetComponent<Animation>().Play();
		AudioSourceManager.PlaySE(2);
		AudioSourceManager.audioBGM.loop = true;
	}

	void toResult() {
		SceneManager.LoadScene("OfflineResult");
	}

	//ボタンを押したら再開
	public void ReStartGame() {
		Time.timeScale = 1;
		Invoke("ReStart", 0.25f);
		GameObject.Find("BackButton").GetComponent<Animation>().Play();
		AudioSourceManager.PlaySE(3);
	}

	void ReStart() {
		isPlaying = true;
		AudioSourceManager.audioBGM.UnPause();
		stopCanvas.SetActive(false);
	}

	//ボタンを押したらタイトルシーンに遷移
	public void BackTitle() {
		Time.timeScale = 1;
		Invoke("toTitle", 0.25f);
		GameObject.Find("ReturnTitleButton").GetComponent<Animation>().Play();
		AudioSourceManager.PlaySE(3);
	}

	void toTitle() {
		SceneManager.LoadScene("Title");
	}
}
