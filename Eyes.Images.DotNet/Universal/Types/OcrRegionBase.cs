using Applitools.Utils.Geometry;

namespace Applitools
{
	public abstract class OcrRegionBase
	{
		private string hint_;
		private string language_;
		private int? minMatch_;
		private Region? region_;

		public OcrRegionBase Hint(string hint)
		{
			hint_ = hint;

			return this;
		}

		internal string GetHint()
		{
			return hint_;
		}

		public OcrRegionBase Language(string language)
		{
			language_ = language;

			return this;
		}

		internal string GetLanguage()
		{
			return language_;
		}

		public OcrRegionBase MinMatch(int? minMatch)
		{
			minMatch_ = minMatch;

			return this;
		}

		internal int? GetMinMatch()
		{
			return minMatch_;
		}

		public OcrRegionBase Region(Region? region)
		{
			region_ = region;
			return this;
		}
		
		internal Region? GetRegion()
		{
			return region_;
		}
	}
}