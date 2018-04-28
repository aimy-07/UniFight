using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonManager : Photon.MonoBehaviour {

	public static bool isPlaying;
	[SerializeField] Animator battleStartPanelAnim;

	[SerializeField] GameObject statusText;
	[SerializeField] GameObject connectPhotonButton;
	[SerializeField] GameObject joinRoomButton;
	[SerializeField] GameObject matchingCanvas;
	[SerializeField] GameObject mainCanvas;

	public static float MAXHP = 20;
	RectTransform myHpbarRect;
	RectTransform enemyHpbarRect;
	PlayerController myPlayerPlayerController;
	PlayerController enemyPlayerPlayerController;

	public static float smallAttackDamage = 3;
	public static float bigAttackDamage = 5;
	public static float SkillDamage = 10;
	
	public static bool isEnded = false;
	[SerializeField] GameObject resultCanvas;
	[SerializeField] Text resultText;
	GameObject winPlayer;
	GameObject losePlayer;
	string winPlayerTag;
	string losePlayerTag;



	void Start() {
		mainCanvas.SetActive(false);
		resultCanvas.SetActive(false);
    	isPlaying = false;
		OnlineResultScript.result = "";
    }
	
	void Update () {
		if (isPlaying) {
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
		isPlaying = true;
	}

	void BattleEnd(string losePlayerTag) {
		isPlaying = false;
		isEnded = true;
		if (losePlayerTag == "myPlayer") {
			winPlayerTag = "enemyPlayer";
		} else {
			winPlayerTag = "myPlayer";
		}
		GameObject cam = GameObject.Find("Main Camera");
		GameObject winPlayer = GameObject.FindWithTag(winPlayerTag);
		GameObject losePlayer = GameObject.FindWithTag(losePlayerTag);
		float cam_x = (losePlayer.transform.position.x + winPlayer.transform.position.x) / 2;
		cam.transform.position = new Vector3(cam_x, 1.6f, -10);
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
			OnlineResultScript.result = "WIN";
		} else {
			resultText.text = "You Lose...";
			OnlineResultScript.result = "LOSE";
		}
	}

	//ボタンを押したらルームを出てリザルトシーンに遷移
	public void GotoResult() {
		PhotonNetwork.LeaveRoom();
		PhotonNetwork.LeaveLobby();
		PhotonNetwork.Disconnect();
		SceneManager.LoadScene("OnlineResult");
	}

	// ボタンを押したらPhotonに接続する。
	public void ConnectPhoton() {
		PhotonNetwork.ConnectUsingSettings("v1.0");
    	PhotonNetwork.logLevel = PhotonLogLevel.Full;
    	PhotonNetwork.sendRate = 60;
    	PhotonNetwork.sendRateOnSerialize = 60;
	}

	// ロビー入室時に呼ばれるコールバックメソッド
	// OnJoinedLobby内でルーム一覧取ったりすると良い。
	void OnJoinedLobby () {
    	Debug.Log ("log : ロビーに入りました");
		joinRoomButton.GetComponent<Button> ().interactable = true;
	}

	// ボタンを押したらランダムにルームに入室する。
	public void JoinRoom(){
		PhotonNetwork.JoinRandomRoom();
	}

	// 入室可能なルームがなく、JoinRandomRoom()が失敗した(false)時に呼ばれる
	// ルームに入れなかったので自分でルームを作る
    void OnPhotonRandomJoinFailed(){
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
		PhotonNetwork.JoinOrCreateRoom ("CustomPropertiesRoom", roomOptions, null);
    }

	// ルーム入室した時に呼ばれるコールバックメソッド
	// ここでキャラクターなどのプレイヤー間で共有するGameObjectを作成すると良い。
	void OnJoinedRoom() {
    	Debug.Log ("log : ルームに入りました");
		if (PhotonNetwork.room.PlayerCount == 1) {
			PhotonNetwork.Instantiate("Player", new Vector3(-1, 0.01f, 0), Quaternion.identity, 0);
			statusText.SetActive(true);
			Destroy(connectPhotonButton);
			Destroy(joinRoomButton);
		} else if (PhotonNetwork.room.PlayerCount == 2) {
			PhotonNetwork.room.IsVisible = false;
			PhotonNetwork.room.IsOpen = false;
			PhotonNetwork.Instantiate("Player", new Vector3(1, 0.01f, 0), Quaternion.identity, 0);
			Invoke("BattleReady", 0.5f);
		}
	}

	// リモートプレイヤーがルームに入室した時によばれるメソッド
	// 自分が先に入り、相手の入室を待機している時、相手が入室したらゲームが始まる
	void OnPhotonPlayerConnected() {
		if (PhotonNetwork.room.PlayerCount == 2) {
			PhotonNetwork.room.IsVisible = false;
			PhotonNetwork.room.IsOpen = false;
			Invoke("BattleReady", 0.5f);
		}
	}

	// ルーム作成
	// public void CreateRoom(){
    //     string userName = "ユーザ1";
    //     string userId = "user1";
    //     PhotonNetwork.autoCleanUpPlayerObjects = false;
    //     //カスタムプロパティ
    //     ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
    //     customProp.Add ("userName", userName); //ユーザ名
    //     customProp.Add ("userId", userId); //ユーザID
    //     PhotonNetwork.SetPlayerCustomProperties(customProp);
	// 	// ルームオプションの作成
    //     RoomOptions roomOptions = new RoomOptions ();
    //     roomOptions.customRoomProperties = customProp;
    //     //ロビーで見えるルーム情報としてカスタムプロパティのuserName,userIdを使いますよという宣言
    //     roomOptions.customRoomPropertiesForLobby = new string[]{ "userName","userId"};
    //     roomOptions.maxPlayers = 2; //部屋の最大人数
    //     roomOptions.isOpen = true; //入室許可する
    //     roomOptions.isVisible = true; //ロビーから見えるようにする
    //     //userIdが名前のルームがなければ作って入室、あれば普通に入室する。
    //     PhotonNetwork.JoinOrCreateRoom (userId, roomOptions, null);
    // }	

    // ルーム一覧が取れると呼ばれるメソッド
    // void OnReceivedRoomListUpdate(){
    //     //ルーム一覧を取る
    //     RoomInfo[] rooms = PhotonNetwork.GetRoomList();
    //     if (rooms.Length == 0) {
    //         Debug.Log ("ルームが一つもありません");
    //     } else {
    //         //ルームが1件以上ある時ループでRoomInfo情報をログ出力
    //         for (int i = 0; i < rooms.Length; i++) {
    //             Debug.Log ("RoomName:"   + rooms [i].name);
    //             Debug.Log ("userName:" + rooms[i].customProperties["userName"]);
    //             Debug.Log ("userId:"   + rooms[i].customProperties["userId"]);
    //             GameObject.Find("StatusText").GetComponent<Text>().text = rooms [i].name;
    //         }
    //     }
    // }
}
