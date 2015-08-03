//----------------------------------------------
//      UnitZ : FPS Sandbox Starter Kit
//    Copyright © Hardworker studio 2015 
// by Rachan Neamprasert www.hardworkerstudio.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelsManager : MonoBehaviour
{
	public PanelInstance[] Pages;
	public PanelInstance currentPanel;

	void Start ()
	{
		for (int i=0; i<Pages.Length; i++) {
			Pages [i].gameObject.AddComponent<PanelInstance> ();
		}
	}
	
	void Awake ()
	{
		if (Pages.Length <= 0)
			return;
		
		OpenPanel (Pages [0]);	
	}

	public void CloseAllPanels ()
	{
		if (Pages.Length <= 0)
			return;
		
		for (int i=0; i<Pages.Length; i++) {
			Animator anim = Pages [i].GetComponent<Animator> ();
			if (anim && anim.isActiveAndEnabled) {
				anim.SetBool ("Open", false);
			}
			StartCoroutine (DisablePanelDeleyed (Pages [i]));
		}
	}

	public void CloseAllPanelsInTheScene ()
	{
		PanelsManager[] panelsManage = (PanelsManager[])GameObject.FindObjectsOfType (typeof(PanelsManager));
		if (panelsManage.Length <= 0)
			return;
		
		for (int i=0; i<panelsManage.Length; i++) {
			panelsManage [i].CloseAllPanels ();	
		}
	}
	
	public void OpenPanelByNameNoPreviousSave (string name)
	{
		PanelInstance page = null;
		for (int i=0; i<Pages.Length; i++) {
			if (Pages [i].name == name) {
				page = Pages [i];
				break;
			}
		}
		if (page == null)
			return;
		
		currentPanel = page;
		
		CloseAllPanels ();
		Animator anim = page.GetComponent<Animator> ();
		if (anim && anim.isActiveAndEnabled) {
			anim.SetBool ("Open", true);
		}
		page.gameObject.SetActive (true);
		
	}
	
	public void OpenPanelByName (string name)
	{
		PanelInstance page = null;
		for (int i=0; i<Pages.Length; i++) {
			if (Pages [i].name == name) {
				page = Pages [i];
				break;
			}
		}
		if (page == null)
			return;
		
		page.PanelBefore = currentPanel;
		currentPanel = page;
		
		CloseAllPanels ();
		Animator anim = page.GetComponent<Animator> ();
		if (anim && anim.isActiveAndEnabled) {
			anim.SetBool ("Open", true);
		}
		page.gameObject.SetActive (true);
		
	}
	
	public bool IsPanelOpened (string name)
	{
		for (int i=0; i<Pages.Length; i++) {
			if (Pages [i].name == name) {
				return Pages [i].gameObject.activeSelf;
			}
		}
		return false;
	}

	public bool TogglePanelByName (string name)
	{
		PanelInstance page = null;
		for (int i=0; i<Pages.Length; i++) {
			if (Pages [i].name == name) {
				page = Pages [i];
				break;
			}
		}
		if (page == null)
			return false;
		
		if (currentPanel == page) {
			ClosePanel (page);
			return false;
		} else {
			page.PanelBefore = currentPanel;
			currentPanel = page;
		
			CloseAllPanels ();
			Animator anim = page.GetComponent<Animator> ();
			if (anim && anim.isActiveAndEnabled) {
				anim.SetBool ("Open", true);
			}
			page.gameObject.SetActive (true);
			return true;
		}
		
	}
	
	public void ClosePanel (PanelInstance page)
	{
		if (page == null)
			return;
		
		currentPanel = null;
		Animator anim = page.GetComponent<Animator> ();
		if (anim && anim.isActiveAndEnabled) {
			anim.SetBool ("Open", false);
		}
		StartCoroutine (DisablePanelDeleyed (page));
		
	}
	
	public void OpenPanel (PanelInstance page)
	{
		if (page == null)
			return;
		
		page.PanelBefore = currentPanel;
		currentPanel = page;
		
		CloseAllPanels ();
		Animator anim = page.GetComponent<Animator> ();
		if (anim && anim.isActiveAndEnabled) {
			anim.SetBool ("Open", true);
		}
		page.gameObject.SetActive (true);

	}
	
	public void OpenPreviousPanel ()
	{
		if (currentPanel && currentPanel.PanelBefore) {

			CloseAllPanels ();
			Animator anim = currentPanel.PanelBefore.GetComponent<Animator> ();
			if (anim && anim.isActiveAndEnabled) {
				anim.SetBool ("Open", true);
			}
			currentPanel.PanelBefore.gameObject.SetActive (true);
			currentPanel = currentPanel.PanelBefore;
		}
	}
	
	IEnumerator DisablePanelDeleyed (PanelInstance page)
	{

		bool closedStateReached = false;
		bool wantToClose = true;
		Animator anim = page.GetComponent<Animator> ();
		if (anim && anim.enabled) {

			while (!closedStateReached && wantToClose) {
				if (!anim.IsInTransition (0)) {
					closedStateReached = anim.GetCurrentAnimatorStateInfo (0).IsName ("Closing");
				}
				yield return new WaitForEndOfFrame();
			}

			if (wantToClose) {
				anim.gameObject.SetActive (false);
			}
			
		} else {
			page.gameObject.SetActive (false);		
		}
		
	}
}
