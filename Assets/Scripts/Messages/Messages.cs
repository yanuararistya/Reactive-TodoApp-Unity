using Todo;

public class RemoveTodoMsg {
	public TodoPresenter presenter;
}

public class CompletedChangeMsg {
	public bool completed;
}

public class ClearCompletedMsg {}

public class FilterChangedMsg {
	public Filter filter;
}