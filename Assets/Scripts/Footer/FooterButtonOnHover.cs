using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FooterButtonOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {

	Text text;

	void Start () {
		text = GetComponentInChildren<Text> ();
	}

	public void OnPointerEnter (PointerEventData e) {
		text.fontStyle = FontStyle.Bold;
	}

	public void OnPointerExit (PointerEventData e) {
		text.fontStyle = FontStyle.Normal;
	}

	public void OnPointerDown (PointerEventData e) {
		text.fontStyle = FontStyle.Normal;
	}
}
