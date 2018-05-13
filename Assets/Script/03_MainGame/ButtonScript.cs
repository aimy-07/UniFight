using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

	public static bool smallAttackButtonPressed;
	public static bool bigAttackButtonPressed;
	public static bool skillButtonPressed;
	public static bool avoidButtonPressed;

	[SerializeField] Button smallAttackButton;
	[SerializeField] Button bigAttackButton;
	[SerializeField] Button skillButton;
	[SerializeField] Button avoidButton;

	float smallAttackTimer;
	float bigAttackTimer;
	float skillTimer;
	float avoidTimer;




	void Start () {
		smallAttackButtonPressed = false;
		bigAttackButtonPressed = false;
		skillButtonPressed = false;
		avoidButtonPressed = false;
	}
	
	void Update () {
		if (smallAttackButtonPressed) {
			smallAttackTimer = 0.5f;
			smallAttackButton.interactable = false;
		}
		if (smallAttackTimer > 0) {
			smallAttackTimer -= Time.deltaTime;
		} else {
			smallAttackButton.interactable = true;
		}
		if (bigAttackButtonPressed) {
			bigAttackTimer = 0.7f;
			bigAttackButton.interactable = false;
		}
		if (bigAttackTimer > 0) {
			bigAttackTimer -= Time.deltaTime;
		} else {
			bigAttackButton.interactable = true;
		}
		if (skillButtonPressed) {
			skillTimer = 1.0f;
			skillButton.interactable = false;
		}
		if (skillTimer > 0) {
			skillTimer -= Time.deltaTime;
		} else {
			skillButton.interactable = true;
		}
		if (avoidButtonPressed) {
			avoidTimer = 1.0f;
			avoidButton.interactable = false;
		}
		if (avoidTimer > 0) {
			avoidTimer -= Time.deltaTime;
		} else {
			avoidButton.interactable = true;
		}
	}
	
	public void SmallAttackButtonDown() {
		smallAttackButtonPressed = true;
	}

	public void BigAttackButtonDown() {
		bigAttackButtonPressed = true;
	}

	public void SkillButtonDown() {
		skillButtonPressed = true;
	}

	public void AvoidButtonDown() {
		avoidButtonPressed = true;
	}
}
