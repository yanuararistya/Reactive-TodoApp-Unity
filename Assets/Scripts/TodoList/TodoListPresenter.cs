using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Todo;

namespace TodoList {
	public class TodoListPresenter : MonoBehaviour {

		[SerializeField] Button selectAllButton;
		[SerializeField] FooterPresenter footer;
		[SerializeField] TodoPresenter todoPrefab;

		public TodoList model;

		void Start () {
			model = new TodoList ();

			model.todoList.ObserveAdd ()
				.Select (e => e.Value)
				.Subscribe (OnAdd)
				.AddTo (this);

			IObservable<TodoPresenter> removeStream = MessageBroker.Default.Receive<RemoveTodoMsg> ()
				.Select (msg => msg.presenter);

			removeStream	
				.Subscribe (OnRemove)
				.AddTo (this);

			removeStream
				.Select (todo => todo.model.completed.Value)
				.Where (completed => completed == false)
				.Subscribe (completed => footer.model.itemsLeft.Value--);

			IObservable<int> countChangedStream = model.todoList.ObserveCountChanged ();

			countChangedStream
				.Select (count => count > 0)
				.StartWith (false)
				.SubscribeToInteractable (selectAllButton)
				.AddTo (this);

			countChangedStream
				.Select (count => count > 0)
				.StartWith (false)
				.Subscribe (hasItem => footer.gameObject.SetActive (hasItem))
				.AddTo (this);
			
			selectAllButton.OnClickAsObservable ()
				.Subscribe (_ => OnSelectAllButtonClicked ())
				.AddTo (this);

			MessageBroker.Default.Receive<ClearCompletedMsg> ()
				.Subscribe (_ => ClearCompleted ())
				.AddTo (this);
		}

		void OnAdd (TodoModel todo) {
			TodoPresenter presenter = Instantiate<TodoPresenter> (todoPrefab, transform);
			presenter.Init (todo, footer);
			presenter.transform.localScale = Vector3.one;

			footer.transform.SetAsLastSibling ();
		}

		void OnRemove (TodoPresenter presenter) {
			model.todoList.Remove(presenter.model);
			Destroy(presenter.gameObject);
		}

		void ClearCompleted () {
			for (int i = model.todoList.Count - 1; i >= 0; i--) {
				TodoModel todo = model.todoList [i];

				if (todo.completed.Value) {
					OnRemove (todo.presenter);
				}
			}
		}

		void SelectAll (bool completed) {
			for (int i = 0; i < model.todoList.Count; i++) {
				TodoModel todo = model.todoList [i];
				todo.completed.Value = completed;
			}
		}

		void OnSelectAllButtonClicked () {
			SelectAll (!IsAllCompleted ());
		}

		bool IsAllCompleted () {
			bool isAllCompleted = true;

			for (int i = 0; i < model.todoList.Count && isAllCompleted; i++) {
				isAllCompleted = model.todoList [i].completed.Value;
			}

			return isAllCompleted;
		}
	}
}