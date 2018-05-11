using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NCMB;
using System;
using System.Text.RegularExpressions;

public class RecordManager : MonoBehaviour {

	[SerializeField] Text nameText;
	[SerializeField] Text countText;
	[SerializeField] Text percentText;
	[SerializeField] Image percentGraoh;

	string id;
	string playerName;
	int winCount;
	int loseCount;
	int playCount;
	float winPercent;

	[SerializeField] Canvas inputCanvas;
	[SerializeField] InputField inputField;
	[SerializeField] Button finishEditButton;
	[SerializeField] GameObject errorText;

	public GameObject[] rankItems;
	public Sprite[] rankItemSprites;
 	[SerializeField] Button refreshButton;
	[SerializeField] GameObject nowLoading;
	[SerializeField] GameObject failedLoading;

	AudioSource[] audio_systemSE = new AudioSource[6];
	[SerializeField] AudioSource bgm;
	bool buttonPressed = false;


	void Start() {
		// 端末内保存情報からユーザー情報を抽出
		id = PlayerPrefs.GetString("ID");
		playerName = PlayerPrefs.GetString("PlayerName");
		winCount = PlayerPrefs.GetInt("WinCount");
		loseCount = PlayerPrefs.GetInt("LoseCount");
		playCount = winCount + loseCount;
		if (playCount == 0) {
			winPercent = 0;
		} else {
			winPercent = (float)winCount / playCount;
		}
		// UI表示
		nameText.text = playerName;
		countText.text = winCount.ToString().PadLeft(3, '0') + "勝 / " + loseCount.ToString().PadLeft(3, '0') + "敗";
		percentGraoh.fillAmount = winPercent;
		percentText.text = (winPercent * 100).ToString("f1") + "%";

		inputCanvas.enabled = false;

		audio_systemSE = GameObject.Find("audio").GetComponents<AudioSource>();
		// for (int i = 0; i < audio_systemSE.Length; i++) {
		// 	audio_systemSE[i].volume = 1.0f;
		// }
		//bgm.volume = 1.0f;  //BGM

		GetRanking();


		// ニフティクラウド関係 ----------------------------------------------

		// データの新規追加（初回ユーザー登録）
		// name = "1P";
		// nowOnlinePlayCount = 5;
		// nowOnlineWinCount = 3;
		// nowOnlineWinPercent = (double) nowOnlineWinCount / nowOnlinePlayCount;
		// NCMBObject database = new NCMBObject("DataBase");
		// database["id"] = Guid.NewGuid().ToString();
		// database["name"] = name;
		// database["playCount"] = nowOnlinePlayCount;
		// database["winCount"] = nowOnlineWinCount;
		// database["loseCount"] = nowOnlinePlayCount - nowOnlineWinCount;
		// database["winPercent"] = nowOnlineWinPercent;
		// database.SaveAsync();

		// データの更新
		// NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("DataBase"); // NCMB上のScoreRankingクラスを取得
		// query.WhereEqualTo ("id", "3b2f3e9f-5080-45ac-855c-ab2881349f78"); // idでデータを絞る
        // query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {
        //     if (e == null) { // データの検索が成功したら、
		// 		objList[0]["winPercent"] = nowOnlineWinPercent; // 新しいスコアとしてデータを更新
        //         objList[0].SaveAsync(); // セーブ
        //     }
        // });

		// ------------------------------------------------------------------
	}

	void Update() {
		if (CheckName(inputField.text)) {
			finishEditButton.interactable = true;
		} else {
			finishEditButton.interactable = false;
		}
		if (buttonPressed) {
			bgm.volume -= Time.deltaTime * 1.2f;
		}
	}

	public void EditNameButton() {
		audio_systemSE[0].Play();
		inputCanvas.enabled = true;
		inputField.text = playerName;
	}

	public void FinisyEditNameButton() {
		errorText.SetActive(false);
		NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("DataBase");
		query.WhereEqualTo ("id", id);
        query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {
            if (e == null) { // データの検索が成功したら、
				objList[0]["name"] = inputField.text;
				objList[0].SaveAsync((NCMBException e2) => {
            		if (e2 == null) {
						GetRanking();
					}
				});
				playerName = inputField.text;
				nameText.text = playerName;
				PlayerPrefs.SetString("PlayerName", playerName);
				inputCanvas.enabled = false;
				audio_systemSE[2].Play();
		    } else {
				errorText.SetActive(true);
				audio_systemSE[4].Play();
			}
        });
		
	}

	public void CancelEditNameButton() {
		inputCanvas.enabled = false;
		audio_systemSE[5].Play();
	}

	bool CheckName(string name) {
		if (name.Length <= 0 || name.Length > 15) {
			return false;
		}
		if (!Regex.IsMatch(name, @"[^a-zA-z0-9-_]")) {
			return true;
		}
		return false;
	}

	public void GetRanking() {
		nowLoading.SetActive(true);
		failedLoading.SetActive(false);
		refreshButton.interactable = false;
		NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("DataBase");
        query.OrderByDescending ("winCount");  // 勝った回数を降順（多い順）に並び替える
		query.AddAscendingOrder ("loseCount");  //さらに負けた回数を昇順（少ない順）で並べ替え
        query.Limit = 10; // 上位10件のみ取得
        query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {
            if(e == null){ //検索成功したら
				List<string> idList = new List<string>(); // idのリスト
                List<string> nameList = new List<string>(); // 名前のリスト
                List<int> winCountList = new List<int>(); // 勝った回数のリスト
				List<int> loseCountList = new List<int>(); // 負けた回数のリスト
                for(int i = 0; i < objList.Count; i++){
					string _id = System.Convert.ToString(objList[i]["id"]); // idを取得
                    string n = System.Convert.ToString(objList[i]["name"]); // 名前を取得
                    int w = System.Convert.ToInt32(objList[i]["winCount"]); // 勝った回数を取得
					int l = System.Convert.ToInt32(objList[i]["loseCount"]); // 　負けた回数を取得
					idList.Add(_id);
                    nameList.Add(n); // リストに突っ込む
                    winCountList.Add(w);
					loseCountList.Add(l);
                }
				int rank = 1;
				for (int i = 0; i < rankItems.Length; i++) {
					if (i < nameList.Count) {
						if (idList[i] == id) {
							rankItems[i].GetComponent<Image>().sprite = rankItemSprites[1];
						} else {
							rankItems[i].GetComponent<Image>().sprite = rankItemSprites[0];
						}
						rankItems[i].GetComponent<Image>().color = Color.white;
						rankItems[i].transform.Find("RankText").gameObject.GetComponent<Text>().text = rank + "位";
						rankItems[i].transform.Find("NameText").gameObject.GetComponent<Text>().text = nameList[i];
						rankItems[i].transform.Find("CountText").gameObject.GetComponent<Text>().text = winCountList[i] + "勝 / " + loseCountList[i] + "敗";
						if (i < nameList.Count - 1 && winCountList[i + 1] == winCountList[i] && loseCountList[i + 1] == loseCountList[i]) {
							// 次が同率順位のスコア
						} else {
							rank++;
						}
					} else {
						// rankItems[i].SetActive(false);
						rankItems[i].GetComponent<Image>().sprite = rankItemSprites[0];
						rankItems[i].GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f);
						rankItems[i].transform.Find("RankText").gameObject.GetComponent<Text>().text = rank + "位";
						rankItems[i].transform.Find("NameText").gameObject.GetComponent<Text>().text = "No Data";
						rankItems[i].transform.Find("CountText").gameObject.GetComponent<Text>().text = "--勝 / --敗";
						rank++;
					}
	 			}
				nowLoading.SetActive(false);
				refreshButton.interactable = true;
				audio_systemSE[3].Play();
            } else {  //ネットワークに接続していないなどでデータが取れなかった時
				nowLoading.SetActive(false);
				failedLoading.SetActive(true);
				refreshButton.interactable = true;
				audio_systemSE[4].Play();
			}
        });
	}

	public void BackButton() {
		Invoke("toTitle", 0.2f);
		audio_systemSE[5].Play();
		buttonPressed = true;
	}

	void toTitle() {
		SceneManager.LoadScene("Title");
	}
}
