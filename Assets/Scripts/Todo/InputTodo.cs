using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using Todo;
using TodoList;

public class InputTodo : MonoBehaviour {

	InputField inputField;
	[SerializeField] TodoListPresenter todoListPresenter;

	void Awake () {
		inputField = GetComponent<InputField> ();
	}

	void Start () {
		inputField.OnEndEditAsObservable ()
			.Subscribe (AddNewTodo)
			.AddTo (this);
	}

	void AddNewTodo (string title) {
		if (title == "")
			return;
		
		TodoModel todo = new TodoModel (title);
		todoListPresenter.model.todoList.Add (todo);

		inputField.text = "";
	}
}
