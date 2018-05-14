using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHowToPlay : MonoBehaviour {

	bool isOpen = false;
	[SerializeField] Animator howToPlayCanvasAnim;
	[SerializeField] Canvas settingButtonCanvas;
	[SerializeField] Animator howToPlayButtonCanvasAnim;
	public Sprite[] sprites;
	[SerializeField] Image image;
	public string[] str;
	[SerializeField] Text text;
	int num;
	[SerializeField] Button nextButton;
	[SerializeField] Button prevButton;

	void Start () {
		
	}
	
	void Update () {
		switch(num) {
			case 0:
				prevButton.interactable = false;
				nextButton.interactable = true;
				break;
			case 1:
			case 2:
				prevButton.interactable = true;
				nextButton.interactable = true;
				break;
			case 3:
				prevButton.interactable = true;
				nextButton.interactable = false;
				break;
		}
		image.sprite = sprites[num];
		text.text = str[num];

		if (isOpen) {
			settingButtonCanvas.enabled = false;
		} else {
			settingButtonCanvas.enabled = true;
		}
	}

	public void NextButton() {
		AudioSourceManager.PlaySE(2);
		num++;
	}

	public void PrevButton() {
		AudioSourceManager.PlaySE(2);
		num--;
	}

	public void SettingButton() {
		AudioSourceManager.PlaySE(2);
		if (isOpen) {
			howToPlayCanvasAnim.SetTrigger("Close");
			howToPlayButtonCanvasAnim.SetTrigger("Close");
		} else {
			howToPlayCanvasAnim.SetTrigger("Open");
			howToPlayButtonCanvasAnim.SetTrigger("Open");
		}
	}

	void IsOpenChange() {
		isOpen = !isOpen;
		num = 0;
	}
}
