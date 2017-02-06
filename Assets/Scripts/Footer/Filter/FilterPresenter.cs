using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;

public class FilterPresenter : MonoBehaviour {

	Toggle toggle;
	public FilterModel model;

	void Start () {
		toggle = GetComponent<Toggle> ();

		toggle.OnValueChangedAsObservable ()
			.Subscribe (OnToggle);
	}

	void OnToggle (bool isOn) {
		toggle.interactable = !isOn;
		MessageBroker.Default.Publish (new FilterChangedMsg () { filter = model.filter });
	}
}