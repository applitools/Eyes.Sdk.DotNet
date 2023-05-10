namespace Applitools
{
	public abstract class OcrRegionBase
	{
		private string hint_;
		private string language_;
		private int? minMatch_;
		public OcrRegionBase Hint(string hint)
		{
			hint_ = hint;

			return this;
		}

		public string GetHint()
		{
			return hint_;
		}

		public OcrRegionBase Language(string language)
		{
			language_ = language;

			return this;
		}

		public string GetLanguage()
		{
			return language_;
		}

		public OcrRegionBase MinMatch(int? minMatch)
		{
			minMatch_ = minMatch;

			return this;
		}

		public int? GetMinMatch()
		{
			return minMatch_;
		}
	}
}