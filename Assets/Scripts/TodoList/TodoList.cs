using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace TodoList {
	public class TodoList {
		public ReactiveCollection<Todo.TodoModel> todoList;

		public TodoList () {
			todoList = new ReactiveCollection<Todo.TodoModel> ();
		}
	}
}