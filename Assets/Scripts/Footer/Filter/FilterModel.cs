using UniRx;

[System.Serializable]
public enum Filter {
	All,
	Active,
	Completed
}

[System.Serializable]
public class FilterModel {
	public Filter filter;
}
