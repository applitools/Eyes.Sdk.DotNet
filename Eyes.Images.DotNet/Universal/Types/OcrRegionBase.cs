namespace Applitools
{
	public abstract class OcrRegionBase
	{
		private string _hint;
		private string _language;
		private int? _minMatch;
		public OcrRegionBase Hint(string hint)
		{
			_hint = hint;

			return this;
		}

		public string GetHint()
		{
			return _hint;
		}

		public OcrRegionBase Language(string language)
		{
			_language = language;

			return this;
		}

		public string GetLanguage()
		{
			return _language;
		}

		public OcrRegionBase MinMatch(int? minMatch)
		{
			_minMatch = minMatch;

			return this;
		}

		public int? GetMinMatch()
		{
			return _minMatch;
		}
	}
}