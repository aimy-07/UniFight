using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsScript : MonoBehaviour {

	public static void ShowAd() {
		if (Advertisement.IsReady()) {
			Advertisement.Show();
		}
	}
}
