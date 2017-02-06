using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

namespace Todo {
	public class TodoPresenter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

		[SerializeField] Text title;
		[SerializeField] Toggle toggle;
		[SerializeField] Button removeButton;
		[SerializeField] InputField inputField;

		FooterPresenter footerPresenter;

		public TodoModel model;

		public void Init (TodoModel _model, FooterPresenter _footerPresenter) {
			model = _model;
			footerPresenter = _footerPresenter;

			model.presenter = this;

			model.title.SubscribeToText (title);

			IObservable<bool> onCompletedChange = model.completed.ObserveEveryValueChanged (value => model.completed.Value);

			onCompletedChange
				.Subscribe (OnToggle)
				.AddTo (this);

			onCompletedChange
				.Subscribe (OnCompletedChange)
				.AddTo (this);

			toggle.OnValueChangedAsObservable ()
				.Subscribe (OnToggle)
				.AddTo (this);
			
			removeButton.OnClickAsObservable ()
				.Subscribe (_ => OnClick ())
				.AddTo (this);

			footerPresenter.model.filter.ObserveEveryValueChanged (filter => filter.Value)
				.Subscribe (OnFilter)
				.AddTo (this);

			onCompletedChange
				.Select(_ => footerPresenter.model.filter.Value)
				.Subscribe (OnFilter)
				.AddTo (this);

			ObservablePointerDownTrigger titlePointerDownTrigger = title.gameObject.AddComponent<ObservablePointerDownTrigger> ();
			titlePointerDownTrigger.OnPointerDownAsObservable ()
				.Buffer (titlePointerDownTrigger.OnPointerDownAsObservable ().Throttle (System.TimeSpan.FromMilliseconds (250)))
				.Where (buffer => buffer.Count >= 2)
				.Subscribe (_ => model.editing.Value = true);

			model.editing.ObserveEveryValueChanged (editing => editing.Value)
				.StartWith (false)
				.Subscribe (editing => {
					inputField.gameObject.SetActive (editing);

					if (editing) {
						inputField.text = model.title.Value;
						EventSystem.current.SetSelectedGameObject(inputField.gameObject);
					}
				})
				.AddTo (this);

			inputField.OnEndEditAsObservable ()
				.Subscribe (text => {
					model.title.Value = text;
					model.editing.Value = false;
				})
				.AddTo (this);

			model.editing
				.Select (editing => !editing)
				.SubscribeToInteractable (toggle);
		}

		void OnClick () {
			MessageBroker.Default.Publish<RemoveTodoMsg> (new RemoveTodoMsg { presenter = this });
		}

		void OnCompletedChange (bool _completed) {
			MessageBroker.Default.Publish<CompletedChangeMsg> (new CompletedChangeMsg { completed = _completed });
		}

		void OnToggle (bool value) {
			toggle.isOn = value;
			model.completed.Value = value;

			float alpha = model.completed.Value ? .2f : 1;
			MainThreadDispatcher.StartUpdateMicroCoroutine (TitleFadeCoroutine (alpha));
		}

		public void OnPointerEnter (PointerEventData pointer) {
			removeButton.gameObject.SetActive (true);
		}

		public void OnPointerExit (PointerEventData pointer) {
			removeButton.gameObject.SetActive (false);
		}

		IEnumerator TitleFadeCoroutine (float alpha) {
			if (alpha < title.color.a) {
				while (alpha < title.color.a && title != null) {
					title.color = new Color (title.color.r, title.color.g, title.color.b, title.color.a - Time.deltaTime);
					yield return null;
				}
			} else {
				while (alpha > title.color.a && title != null) {
					title.color = new Color (title.color.r, title.color.g, title.color.b, title.color.a + Time.deltaTime);
					yield return null;
				}
			}
		}

		void OnFilter (Filter _filter) {
			bool show = false;

			switch (_filter) {
			case Filter.All:
				show = true;
				break;
			case Filter.Active:
				show = !model.completed.Value;
				break;
			case Filter.Completed:
				show = model.completed.Value;
				break;
			}

			gameObject.SetActive (show);
			removeButton.gameObject.SetActive (false);
		}
	}
}