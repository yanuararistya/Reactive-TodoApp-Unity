using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using TodoList;

public class FooterPresenter : MonoBehaviour {

	[SerializeField] Text itemsLeftText;
	[SerializeField] Button clearCompletedButton;

	[SerializeField] TodoListPresenter todoListPresenter;

	public FooterModel model;

	void Start () {
		Observable.NextFrame ()
			.Subscribe (_ => Init ());
	}

	void Init () {
		model = new FooterModel ();

		model.itemsLeft.ObserveEveryValueChanged (_itemsLeft => _itemsLeft.Value)
			.Select (value => value + " items left")
			.SubscribeToText (itemsLeftText)
			.AddTo (this);

		MessageBroker.Default.Receive<CompletedChangeMsg> ()
			.Select (msg => msg.completed)
			.Subscribe (completed => model.itemsLeft.Value += completed ? -1 : 1)
			.AddTo (this);

		model.itemsLeft.ObserveEveryValueChanged (itemsLeft => itemsLeft.Value)
			.Select (value => !(value == todoListPresenter.model.todoList.Count))
			.StartWith (false)
			.SubscribeToInteractable (clearCompletedButton);

		clearCompletedButton.OnClickAsObservable ()
			.Subscribe (_ => OnClearCompletedButtonClicked ());
		
		MessageBroker.Default.Receive<FilterChangedMsg> ()
			.Select (msg => msg.filter)
			.Subscribe (filter => model.filter.Value = filter);
	}

	void OnClearCompletedButtonClicked () {
		MessageBroker.Default.Publish<ClearCompletedMsg> (new ClearCompletedMsg ());
	}
}
