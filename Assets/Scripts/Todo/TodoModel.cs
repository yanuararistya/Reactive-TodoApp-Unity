using UniRx;

namespace Todo {
	public class TodoModel {

		public StringReactiveProperty title;
		public BoolReactiveProperty completed;
		public BoolReactiveProperty editing;
		public TodoPresenter presenter;

		public TodoModel (
			string _title,
			bool _completed = false,
			bool _editing = false
		) {
			title = new StringReactiveProperty (_title);
			completed = new BoolReactiveProperty (_completed);
			editing = new BoolReactiveProperty (_editing);
		}
	}
}