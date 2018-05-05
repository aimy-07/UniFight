using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PhotonManager : Photon.MonoBehaviour {

	public enum PHASE{
		isConnecting, isReady, isStarting, isPlaying, isEnded, other
	}
	public static PHASE phase;
	bool roomIn;
	
	[SerializeField] Text statusText;
	[SerializeField] Button reconnectPhotonButton;
	[SerializeField] Image networkImg;
	public Sprite[] networkImgSprites;
	[SerializeField] Button backButton;
	[SerializeField] GameObject loadingImg;
	[SerializeField] GameObject matchingCanvas;
	[SerializeField] Text statusText1;
	[SerializeField] Text statusText2;
	[SerializeField] GameObject stopCanvas;
	[SerializeField] GameObject mainCanvas;
	[SerializeField] Animator battleStartPanelAnim;

	public static float MAXHP = 20;
	RectTransform myHpbarRect;
	RectTransform enemyHpbarRect;
	PlayerController myPlayerPlayerController;
	PlayerController enemyPlayerPlayerController;

	public static float smallAttackDamage = 3;
	public static float bigAttackDamage = 5;
	public static float SkillDamage = 10;
	
	[SerializeField] GameObject resultCanvas;
	[SerializeField] Text resultText;
	GameObject winPlayer;
	GameObject losePlayer;
	string winPlayerTag;
	string losePlayerTag;



	void Start() {
		phase = PHASE.isConnecting;

		matchingCanvas.SetActive(true);
		statusText.text = "";
		backButton.interactable = true;
		statusText1.text = "";
		statusText2.text = "";
		stopCanvas.SetActive(false);
		mainCanvas.SetActive(false);
		resultCanvas.SetActive(false);
		ResultManager.result = "";

		ConnectPhoton();
    }
	
	void Update () {
		if (phase == PHASE.isReady) {
			BattleReady();
			phase = PHASE.isStarting;
		}
		if (phase == PHASE.isPlaying) {
			myHpbarRect.localScale = new Vector3(myPlayerPlayerController.hp / MAXHP, 1, 1);
			enemyHpbarRect.localScale = new Vector3(enemyPlayerPlayerController.hp / MAXHP, 1, 1);
		}
	}

	void BattleReady() {
		Destroy(matchingCanvas);
		battleStartPanelAnim.SetTrigger("START");
		Invoke("BattleStart", 3);
	}

	void BattleStart() {
		mainCanvas.SetActive(true);
		battleStartPanelAnim.SetTrigger("DELETE");
		myPlayerPlayerController = GameObject.FindWithTag("myPlayer").GetComponent<PlayerController>();
		enemyPlayerPlayerController = GameObject.FindWithTag("enemyPlayer").GetComponent<PlayerController>();
		if (GameObject.FindWithTag("myPlayer").transform.position.x < 0) {
			myHpbarRect = GameObject.Find("1PHPbar").GetComponent<RectTransform>();
			enemyHpbarRect = GameObject.Find("2PHPbar").GetComponent<RectTransform>();
		} else  {
			myHpbarRect = GameObject.Find("2PHPbar").GetComponent<RectTransform>();
			enemyHpbarRect = GameObject.Find("1PHPbar").GetComponent<RectTransform>();
		}
		phase = PHASE.isPlaying;
	}

	void BattleEnd(string losePlayerTag) {
		phase = PHASE.isEnded;
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
		} else {
			if (losePlayer.transform.rotation.y > 0) {
				cam_x = losePlayer.transform.transform.position.x - 0.4f;
			} else {
				cam_x = losePlayer.transform.transform.position.x + 0.4f;
			}
		}
		cam.transform.position = new Vector3(cam_x, 1.5f, -10);
		cam.transform.eulerAngles = new Vector3(6, 0, 0);
		cam.GetComponent<Camera>().orthographicSize = 0.6f;
		Time.timeScale = 0.5f;
		mainCanvas.SetActive(false);
		Invoke("ShowResult", 1);
	}

	void ShowResult() {
		Time.timeScale = 1f;
		GameObject winPlayer = GameObject.FindWithTag(winPlayerTag);
		winPlayer.transform.Find("UTC_Default").gameObject.GetComponent<Animator>().SetTrigger("Win");
		winPlayer.transform.eulerAngles = new Vector3(0, 180, 0);
		resultCanvas.SetActive(true);
		if (winPlayerTag == "myPlayer") {
			resultText.text = "You Win!";
			ResultManager.result = "WIN";
		} else {
			resultText.text = "You Lose...";
			ResultManager.result = "LOSE";
		}
	}

	//ボタンを押したらルームを出てリザルトシーンに遷移
	public void GotoResult() {
		PhotonNetwork.LeaveRoom();
		PhotonNetwork.LeaveLobby(); 
		PhotonNetwork.Disconnect();
		Invoke("toResult", 0.5f);
		GameObject.Find("NextButton").GetComponent<Animation>().Play();
	}

	void toResult() {
		SceneManager.LoadScene("Result");
	}

	public void GotoTitle() {
		if (roomIn) {
			phase = PHASE.other;
			PhotonNetwork.room.IsVisible = false;
			PhotonNetwork.room.IsOpen = false;
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.LeaveLobby();
			PhotonNetwork.Disconnect();
		}
		SceneManager.LoadScene("Title");
	}

	// Photon関係 -----------------------------------------------------

	// Photonに接続するメソッド
	void ConnectPhoton() {
		PhotonNetwork.ConnectUsingSettings("v1.0");
    	PhotonNetwork.logLevel = PhotonLogLevel.Full;
    	PhotonNetwork.sendRate = 60;
    	PhotonNetwork.sendRateOnSerialize = 60;
		PhotonNetwork.BackgroundTimeout = 0;
		reconnectPhotonButton.interactable = false;
		networkImg.sprite = networkImgSprites[0];
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
		PhotonNetwork.JoinRandomRoom();
	}

	// ルーム入室した時に呼ばれるコールバックメソッド
	// ここでキャラクターなどのプレイヤー間で共有するGameObjectを作成する
	void OnJoinedRoom() {
    	Debug.Log ("log : ルームに入りました");
		roomIn = true;
		if (PhotonNetwork.room.PlayerCount == 1) {
			PhotonNetwork.Instantiate("Player", new Vector3(-1, 0.01f, 0), Quaternion.identity, 0);
			statusText.text = "対戦相手を待っています...";
		} else if (PhotonNetwork.room.PlayerCount == 2) {
			PhotonNetwork.room.IsVisible = false;
			PhotonNetwork.room.IsOpen = false;
			statusText.text = "";
			backButton.interactable = false;
			PhotonNetwork.Instantiate("Player", new Vector3(1, 0.01f, 0), Quaternion.identity, 0);
		}
	}

	void OnPhotonJoinRoomFailed() {
		Debug.Log ("log : ルームの入室に失敗しました");
	}

	// リモートプレイヤーがルームに入室した時によばれるメソッド
	// 自分が先に入り、相手の入室を待機している時、相手が入室したらゲームが始まる
	void OnPhotonPlayerConnected() {
		Debug.Log ("log : リモートプレイヤーが入室しました");
		if (PhotonNetwork.room.PlayerCount == 2) {
			PhotonNetwork.room.IsVisible = false;
			PhotonNetwork.room.IsOpen = false;
			statusText.text = "";
			backButton.interactable = false;
		}
	}

	public void ReConnectButton() {
		ConnectPhoton();
		GameObject.Find("ReConnectPhotonButton").GetComponent<Animation>().Play();
	}

	// Photonとの接続が切断された時に呼ばれるメソッド
	void OnDisconnectedFromPhoton(){
		Debug.Log ("log : 通信が切断されました");
		switch(phase) {
			case PHASE.isConnecting:
				statusText.text = "通信エラーが発生しました\n通信の安定した場所で再度接続してください";
				reconnectPhotonButton.interactable = true;
				networkImg.sprite = networkImgSprites[1];
				loadingImg.SetActive(false);
				break;
			case PHASE.isReady:
			case PHASE.isStarting:
			case PHASE.isPlaying:
				stopCanvas.SetActive(true);
				statusText1.text = "通信エラーが発生しました";
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
			case PHASE.isReady:
			case PHASE.isStarting:
			case PHASE.isPlaying:
				stopCanvas.SetActive(true);
				statusText1.text = "通信エラーが発生しました";
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
			case PHASE.isConnecting:
			case PHASE.isReady:
			case PHASE.isStarting:
			case PHASE.isPlaying:
				stopCanvas.SetActive(true);
				statusText1.text = "現在同時接続人数が上限を超えています\n時間を置いて再度プレイしてください";
				BackTitleCountdown3();
			break;
			case PHASE.isEnded:
				Debug.Log ("log : すでに勝敗がついています");
			break;
		}	
		statusText.text = "同時接続人数が上限を超えました\n時間を置いて再度プレイしてください";
		Debug.Log ("log : 同時接続人数が20人を超えました");
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
		SceneManager.LoadScene("Title");
	}
}
