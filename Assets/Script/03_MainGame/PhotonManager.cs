using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PhotonManager : Photon.MonoBehaviour {

	public enum PHASE{
		yetJoinedRoom, isWaiting, isConnected, isReady, isPlaying, isEnded, other
	}
	public static PHASE phase = PHASE.other;
	float t;
	
	[SerializeField] GameObject matchingCanvas;
	[SerializeField] Text statusText;
	[SerializeField] Image networkImg;
	[SerializeField] GameObject loadingImg;
	[SerializeField] GameObject backButton;
	[SerializeField] GameObject stopCanvas;
	[SerializeField] Text statusText1;
	[SerializeField] Text statusText2;
	[SerializeField] GameObject mainCanvas;
	[SerializeField] Animation battleStartPanelAnim;
	[SerializeField] Animation battleStartCanvasAnim;

	PlayerController myPlayerPlayerController;
	PlayerController enemyPlayerPlayerController;
	public static int MAXHP = 40;
	[SerializeField] Image myHpbar;
	[SerializeField] Text myHpbarText;
	[SerializeField] Image enemyHpbar;
	[SerializeField] Text enemyHpbarText;
	public static int smallAttackDamage = 3;
	public static int bigAttackDamage = 5;
	public static int SkillDamage = 4;
	public static int SkillOwnDamage = 5;

	public static int MAXAP = 25;
	[SerializeField] Image myApbar;
	[SerializeField] Text myApbarText;
	[SerializeField] Image enemyApbar;
	[SerializeField] Text enemyApbarText;
	public static float AUTE_AP_INTERVAL = 3f;
	public static int SmallAttackAP = 3;
	public static int BigAttackAP = 5;
	public static int AvoidAP = 1;

	[SerializeField] GameObject resultCanvas;
	[SerializeField] Text resultText;
	GameObject winPlayer;
	GameObject losePlayer;
	string winPlayerTag;
	string losePlayerTag;

	public AudioClip matchingBGM;
	public AudioClip startBGM;
	public AudioClip battleBGM;
	public AudioClip[] endBGMs;
	public AudioClip[] startVoices;
	public AudioClip[] attackVoice;
	public AudioClip[] damageVoice;
	public AudioClip[] winVoice;



	void Start() {
		phase = PHASE.yetJoinedRoom;
		OfflineManager.isPlaying = false;

		matchingCanvas.SetActive(true);
		statusText.text = "";
		stopCanvas.SetActive(false);
		statusText1.text = "";
		statusText2.text = "";
		mainCanvas.SetActive(false);
		resultCanvas.SetActive(false);
		ResultManager.result = "";
		ResultManager.enemyName = "";

		ConnectPhoton();

		AudioSourceManager.PlayBGM(matchingBGM);
    }
	
	void Update () {
		switch(phase) {
			case PHASE.isConnected:
				// この３秒の間に相手のみため情報を取得する
				if (t < 3) {
					t += Time.deltaTime;
				} else {
					BattleReady();
				}
				break;
			case PHASE.isPlaying:
				myHpbar.fillAmount = (float)myPlayerPlayerController.hp / MAXHP;
				enemyHpbar.fillAmount = (float)enemyPlayerPlayerController.hp / MAXHP;
				myHpbarText.text = "HP " + myPlayerPlayerController.hp + " / " + PhotonManager.MAXHP;
				enemyHpbarText.text = "HP " + enemyPlayerPlayerController.hp + " / " + PhotonManager.MAXHP;
				myApbar.fillAmount = (float)myPlayerPlayerController.ap / MAXAP;
				enemyApbar.fillAmount = (float)enemyPlayerPlayerController.ap / MAXAP;
				myApbarText.text = "AP " + myPlayerPlayerController.ap + " / " + PhotonManager.MAXAP;
				enemyApbarText.text = "AP " + enemyPlayerPlayerController.ap + " / " + PhotonManager.MAXAP;
				break;
		}
	}

	void BattleReady() {
		phase = PHASE.isReady;

		myPlayerPlayerController = GameObject.FindWithTag("myPlayer").GetComponent<PlayerController>();
		enemyPlayerPlayerController = GameObject.FindWithTag("enemyPlayer").GetComponent<PlayerController>();
		enemyPlayerPlayerController.SetChara("enemyPlayer");
		ResultManager.enemyName = enemyPlayerPlayerController.playerName;

		Destroy(matchingCanvas);
		battleStartPanelAnim.Play();
		battleStartCanvasAnim.Play();
		AudioSourceManager.PlayBGM(startBGM);
	}

	public void LeftVoicePlay() {
		if (GameObject.FindWithTag("myPlayer").transform.position.x < 0) {
			AudioClip clip = startVoices[OfflineCharaSet.GetCharaNum(myPlayerPlayerController.chara)];
			AudioSourceManager.PlayVOICE(clip);
		} else {
			AudioClip clip = startVoices[OfflineCharaSet.GetCharaNum(enemyPlayerPlayerController.chara)];
			AudioSourceManager.PlayVOICE(clip);
		}
	}

	public void RightVoicePlay() {
		if (GameObject.FindWithTag("myPlayer").transform.position.x > 0) {
			AudioClip clip = startVoices[OfflineCharaSet.GetCharaNum(myPlayerPlayerController.chara)];
			AudioSourceManager.PlayVOICE(clip);
		} else {
			AudioClip clip = startVoices[OfflineCharaSet.GetCharaNum(enemyPlayerPlayerController.chara)];
			AudioSourceManager.PlayVOICE(clip);
		}
	}

	public void BattleStart() {
		phase = PHASE.isPlaying;
		AudioSourceManager.PlayBGM(battleBGM);
		AudioSourceManager.PlayVOICE(startVoices[3]);

		mainCanvas.SetActive(true);
		myPlayerPlayerController.joystick = GameObject.Find("Joystick").GetComponent<Joystick>();

		if (GameObject.FindWithTag("myPlayer").transform.position.x < 0) {
			myHpbar = GameObject.Find("1PHPbar").GetComponent<Image>();
			enemyHpbar = GameObject.Find("2PHPbar").GetComponent<Image>();
			myHpbarText = GameObject.Find("1PHPtext").GetComponent<Text>();
			enemyHpbarText = GameObject.Find("2PHPtext").GetComponent<Text>();
			myApbar = GameObject.Find("1PAPbar").GetComponent<Image>();
			enemyApbar = GameObject.Find("2PAPbar").GetComponent<Image>();
			myApbarText = GameObject.Find("1PAPtext").GetComponent<Text>();
			enemyApbarText = GameObject.Find("2PAPtext").GetComponent<Text>();
			GameObject.Find("1Pname").GetComponent<Text>().text = myPlayerPlayerController.playerName;
			GameObject.Find("2Pname").GetComponent<Text>().text = enemyPlayerPlayerController.playerName;
		} else  {
			myHpbar = GameObject.Find("2PHPbar").GetComponent<Image>();
			enemyHpbar = GameObject.Find("1PHPbar").GetComponent<Image>();
			myHpbarText = GameObject.Find("2PHPtext").GetComponent<Text>();
			enemyHpbarText = GameObject.Find("1PHPtext").GetComponent<Text>();
			myApbar = GameObject.Find("2PAPbar").GetComponent<Image>();
			enemyApbar = GameObject.Find("1PAPbar").GetComponent<Image>();
			myApbarText = GameObject.Find("2PAPtext").GetComponent<Text>();
			enemyApbarText = GameObject.Find("1PAPtext").GetComponent<Text>();
			GameObject.Find("2Pname").GetComponent<Text>().text = myPlayerPlayerController.playerName;
			GameObject.Find("1Pname").GetComponent<Text>().text = enemyPlayerPlayerController.playerName;
		}
	}

	// どちらかのHPが0になったら呼ばれる
	void BattleEnd(string losePlayerTag) {
		phase = PHASE.isEnded;
		AudioSourceManager.audioBGM.Stop();
		if (losePlayerTag == "myPlayer") {
			winPlayerTag = "enemyPlayer";
		} else {
			winPlayerTag = "myPlayer";
		}
		GameObject cam = GameObject.Find("Main Camera");
		GameObject myPlayer = GameObject.FindWithTag("myPlayer");
		float cam_x;
		if (winPlayerTag == "myPlayer") {
			// 自分が勝った
			cam_x = myPlayer.transform.position.x;
			AudioClip clip = attackVoice[OfflineCharaSet.GetCharaNum(myPlayerPlayerController.chara)];
			AudioSourceManager.PlayVOICE(clip);
		} else {
			// 自分が負けた
			if (myPlayer.transform.rotation.y > 0) {
				cam_x = myPlayer.transform.position.x - 0.4f;
			} else {
				cam_x = myPlayer.transform.position.x + 0.4f;
			}
			AudioClip clip = damageVoice[OfflineCharaSet.GetCharaNum(myPlayerPlayerController.chara)];
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
		// 勝った方のキャラのWinモーション再生
		GameObject winPlayer = GameObject.FindWithTag(winPlayerTag);
		winPlayer.transform.Find("UTC_Default").gameObject.GetComponent<Animator>().SetTrigger("Win");
		winPlayer.transform.eulerAngles = new Vector3(0, 180, 0);
		resultCanvas.SetActive(true);
		if (winPlayerTag == "myPlayer") {
			// 自分が勝った
			resultText.text = "You Win!";
			ResultManager.result = "WIN";
			AudioClip clip = winVoice[OfflineCharaSet.GetCharaNum(myPlayerPlayerController.chara)];
			AudioSourceManager.PlayVOICE(clip);
			AudioSourceManager.audioBGM.loop = false;
			AudioSourceManager.PlayBGM(endBGMs[0]);
		} else {
			// 相手が勝った
			resultText.text = "You Lose...";
			ResultManager.result = "LOSE";
			AudioSourceManager.audioBGM.loop = false;
			AudioSourceManager.PlayBGM(endBGMs[1]);
		}
	}

	//ボタンを押したらルームを出てリザルトシーンに遷移
	public void GotoResult() {
		PhotonNetwork.LeaveRoom();
		PhotonNetwork.LeaveLobby(); 
		PhotonNetwork.Disconnect();
		Invoke("toResult", 0.25f);
		AudioSourceManager.PlaySE(2);
		GameObject.Find("NextButton").GetComponent<Animation>().Play();
		AudioSourceManager.audioBGM.loop = true;
	}

	void toResult() {
		SceneManager.LoadScene("Result");
	}

	//ボタンを押したらタイトルシーンに遷移
	public void GotoTitle() {
		phase = PHASE.other;
		PhotonNetwork.room.IsVisible = false;
		PhotonNetwork.room.IsOpen = false;
		AudioSourceManager.PlaySE(3);
		PhotonNetwork.LeaveRoom();
		BackTitle();
	}

	public void StopGame() {
		phase = PHASE.other;
		PhotonNetwork.LeaveRoom();
		stopCanvas.SetActive(true);
		AudioSourceManager.PlaySE(4);
		AudioSourceManager.audioBGM.Stop();
		statusText1.text = "オンライン対戦を中断しました";
		BackTitleCountdown3();
	}

	// Photon関係 -----------------------------------------------------

	// Photonに接続するメソッド
	void ConnectPhoton() {
		PhotonNetwork.ConnectUsingSettings("v1.0");
    	PhotonNetwork.logLevel = PhotonLogLevel.Full;
    	PhotonNetwork.sendRate = 60;
    	PhotonNetwork.sendRateOnSerialize = 60;
		PhotonNetwork.BackgroundTimeout = 0;
		statusText.text = "ネットワークに接続中...";
		loadingImg.SetActive(true);
	}

	// Photonに接続した時に呼ばれるメソッド
	void OnConnectedToPhoton() {
		Debug.Log ("log : Photonに接続しました");
		Debug.Log ("log : 自動的にロビーに入ります");
	}

	// ロビー入室時に呼ばれるメソッド
	// Photonに接続すると自動的にロビーに入る
	// 自動的にランダムにルームに入室する
	void OnJoinedLobby () {
    	Debug.Log ("log : ロビーに入りました");
		Debug.Log ("log : 自動的にルームに入ります");
		PhotonNetwork.JoinRandomRoom();
	}

	// 入室可能なルームがなく、JoinRandomRoom()が失敗した(false)時に呼ばれるメソッド
	// ルームに入れなかったので自分でルームを作る
    void OnPhotonRandomJoinFailed(){
		Debug.Log("log : 入室可能なルームがありませんでした");
		Debug.Log("log : 自動的にルームを作成します");
		// プレイヤーが退室した際に同期オブジェクトが消えないよう設定
		PhotonNetwork.autoCleanUpPlayerObjects = false;
		// ルームオプションの作成
		RoomOptions roomOptions = new RoomOptions ();
		roomOptions.IsVisible = true;
		roomOptions.IsOpen = true;
		roomOptions.MaxPlayers = 2;
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable(){{"CustomProperties", "カスタムプロパティ"} };
		roomOptions.CustomRoomPropertiesForLobby = new string[] {"CustomProperties"};
		// ルームの作成
		PhotonNetwork.JoinOrCreateRoom (Guid.NewGuid().ToString(), roomOptions, null);
    }

	// なんらかの原因でルーム作成に失敗した時に呼ばれるメソッド
	// 自動的にルーム再入室を試みる
	void OnPhotonCreateRoomFailed(){
		Debug.Log ("log : ルーム作成に失敗しました");
		Debug.Log ("log : 再度ルーム入室を試みます");
		OnDisconnectedFromPhoton();
	}

	// ルーム入室した時に呼ばれるコールバックメソッド
	// ここでキャラクターなどのプレイヤー間で共有するGameObjectを作成する
	void OnJoinedRoom() {
    	Debug.Log ("log : ルームに入りました");
		if (PhotonNetwork.room.PlayerCount == 1) {
			phase = PHASE.isWaiting;
			PhotonNetwork.Instantiate("Player", new Vector3(-1, 0.01f, 0), Quaternion.identity, 0);
			statusText.text = "対戦相手を待っています...";
		} else if (PhotonNetwork.room.PlayerCount == 2) {
			phase = PHASE.isConnected;
			PhotonNetwork.room.IsVisible = false;
			PhotonNetwork.room.IsOpen = false;
			statusText.text = "相手の情報を取得しています";
			backButton.SetActive(false);
			PhotonNetwork.Instantiate("Player", new Vector3(1, 0.01f, 0), Quaternion.identity, 0);
		}
	}

	void OnPhotonJoinRoomFailed() {
		Debug.Log ("log : ルームの入室に失敗しました");
		Debug.Log ("log : 再度ルーム入室を試みます");
		OnDisconnectedFromPhoton();
	}

	// リモートプレイヤーがルームに入室した時によばれるメソッド
	// 自分が先に入り、相手の入室を待機している時、相手が入室したらゲームが始まる
	void OnPhotonPlayerConnected() {
		Debug.Log ("log : リモートプレイヤーが入室しました");
		if (PhotonNetwork.room.PlayerCount == 2) {
			phase = PHASE.isConnected;
			PhotonNetwork.room.IsVisible = false;
			PhotonNetwork.room.IsOpen = false;
			statusText.text = "相手の情報を取得しています";
			backButton.SetActive(false);
		}
	}

	// Photonとの接続が切断された時に呼ばれるメソッド
	void OnDisconnectedFromPhoton(){
		Debug.Log ("log : ネットワークに接続していません");
		switch(phase) {
			case PHASE.yetJoinedRoom:
			case PHASE.isWaiting:
				statusText.text = "";
				networkImg.enabled = false;
				loadingImg.SetActive(false);
				backButton.SetActive(false);
				stopCanvas.SetActive(true);
				statusText1.text = "ネットワークに接続していません\n通信の安定した場所で再度プレイしてください";
				AudioSourceManager.PlaySE(4);
				AudioSourceManager.audioBGM.Stop();
				BackTitleCountdown3();
				break;
			case PHASE.isConnected:
			case PHASE.isReady:
			case PHASE.isPlaying:
				stopCanvas.SetActive(true);
				statusText1.text = "ネットワークに接続していません\n通信の安定した場所で再度プレイしてください";
				AudioSourceManager.PlaySE(4);
				AudioSourceManager.audioBGM.Stop();
				BackTitleCountdown3();
				break;
			case PHASE.isEnded:
				Debug.Log ("log : すでに勝敗がついています");
				break;
			case PHASE.other:
				Debug.Log ("log : タイトルに戻る時の接続遮断");
				break;
		}
	}

	// リモートプレイヤーが退室した時に呼ばれるメソッド
	void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer){
		Debug.Log ("log : リモートプレイヤーが退室しました");
		switch(phase) {
			case PHASE.yetJoinedRoom:
			case PHASE.isWaiting:
				// まだ相手が接続していない段階なのでスルー
				break;
			case PHASE.isConnected:
			case PHASE.isReady:
			case PHASE.isPlaying:
				stopCanvas.SetActive(true);
				statusText1.text = "対戦相手との通信が切断されました";
				AudioSourceManager.PlaySE(4);
				AudioSourceManager.audioBGM.Stop();
				BackTitleCountdown3();
			break;
			case PHASE.isEnded:
				Debug.Log ("log : すでに勝敗がついています");
			break;
		}	
	}

	// CCU上限を超えた時に呼ばれるメソッド
	void OnPhotonMaxCccuReached(){
		Debug.Log ("log : 同時接続人数が20人を超えました");
		switch(phase) {
			case PHASE.yetJoinedRoom:
			case PHASE.isWaiting:
				statusText.text = "";
				networkImg.enabled = false;
				loadingImg.SetActive(false);
				backButton.SetActive(false);
				stopCanvas.SetActive(true);
				statusText1.text = "ネットワークに接続していません\n通信の安定した場所で再度プレイしてください";
				AudioSourceManager.PlaySE(4);
				AudioSourceManager.audioBGM.Stop();
				BackTitleCountdown3();
				break;
			case PHASE.isConnected:
			case PHASE.isReady:
			case PHASE.isPlaying:
				stopCanvas.SetActive(true);
				statusText1.text = "現在同時接続人数が上限を超えています\n時間を置いて再度プレイしてください";
				AudioSourceManager.PlaySE(4);
				AudioSourceManager.audioBGM.Stop();
				BackTitleCountdown3();
			break;
			case PHASE.isEnded:
				Debug.Log ("log : すでに勝敗がついています");
			break;
		}	
	}

	void BackTitleCountdown3() {
		statusText2.text = "3秒後にタイトルにもどります";
		Invoke("BackTitleCountdown2", 1);
		Invoke("BackTitleCountdown1", 2);
		Invoke("BackTitle", 3);
	}

	void BackTitleCountdown2() {
		statusText2.text = "2秒後にタイトルにもどります";
	}

	void BackTitleCountdown1() {
		statusText2.text = "1秒後にタイトルにもどります";
	}

	void BackTitle() {
		PhotonNetwork.Disconnect();
		SceneManager.LoadScene("Title");
	}
}
