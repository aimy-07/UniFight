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
	[SerializeField] AudioSource bgm;
	AudioSource[] audio_systemSE = new AudioSource[4];
	
	[SerializeField] GameObject resultCanvas;
	[SerializeField] Text resultText;
	GameObject winPlayer;
	GameObject losePlayer;
	string winPlayerTag;
	string losePlayerTag;
	public AudioClip[] endBGMs;
	[SerializeField] AudioSource audio_voice;
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


		audio_systemSE = GameObject.Find("audio").GetComponents<AudioSource>();
		// for (int i = 0; i < audio_systemSE.Length; i++) {  //SE
		// 	audio_systemSE[i].volume = 1.0f;
		// }
		//bgm.volume = 1.0f;  //BGM

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
		bgm.Stop();
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
			audio_voice.clip = attackVoice[OfflineCharaSet.GetCharaNum(winPlayer.GetComponent<OfflinePlayerController>().chara)];
			audio_voice.Play();
		} else {
			if (losePlayer.transform.rotation.y > 0) {
				cam_x = losePlayer.transform.transform.position.x - 0.4f;
			} else {
				cam_x = losePlayer.transform.transform.position.x + 0.4f;
			}
			audio_voice.clip = damageVoice[OfflineCharaSet.GetCharaNum(losePlayer.GetComponent<OfflinePlayerController>().chara)];
			audio_voice.Play();
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
			audio_voice.clip = winVoice[OfflineCharaSet.GetCharaNum(winPlayer.GetComponent<OfflinePlayerController>().chara)];
			audio_voice.Play();
			bgm.clip = endBGMs[0];
			bgm.loop = false;
			bgm.Play();
		} else {
			resultText.text = "You Lose...";
			OfflineResultManager.result = "LOSE";
			bgm.clip = endBGMs[1];
			bgm.loop = false;
			bgm.Play();
		}
	}

	//ボタンを押したら一時停止
	public void StopGame() {
		isPlaying = false;
		Time.timeScale = 0;
		audio_systemSE[2].Play();
		bgm.Pause();
		stopCanvas.SetActive(true);
	}

	//ボタンを押したら再開
	public void ReStartGame() {
		isPlaying = true;
		Time.timeScale = 1;
		audio_systemSE[3].Play();
		bgm.UnPause();
		stopCanvas.SetActive(false);
	}

	//ボタンを押したらリザルトシーンに遷移
	public void GotoResult() {
		Invoke("toResult", 0.3f);
		audio_systemSE[1].Play();
		GameObject.Find("NextButton").GetComponent<Animation>().Play();
	}

	void toResult() {
		SceneManager.LoadScene("OfflineResult");
	}

	//ボタンを押したらタイトルシーンに遷移
	public void BackTitle() {
		Time.timeScale = 1;
		Invoke("toTitle", 0.2f);
		audio_systemSE[3].Play();
	}

	void toTitle() {
		SceneManager.LoadScene("Title");
	}
}
