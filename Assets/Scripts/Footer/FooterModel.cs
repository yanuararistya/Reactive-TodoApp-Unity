using UniRx;

public class FooterModel {

	public IntReactiveProperty itemsLeft;
	public ReactiveProperty<Filter> filter;

	public FooterModel () {
		itemsLeft = new IntReactiveProperty (0);
		filter = new ReactiveProperty<Filter> (Filter.All);
	}

}
