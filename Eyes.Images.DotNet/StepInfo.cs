namespace Applitools
{
    public class StepInfo
    {
        public string Name { get; set; }
        public bool IsDifferent { get; set; }
        public bool HasBaselineImage { get; set; }
        public bool HasCurrentImage { get; set; }
        public bool HasCheckpointImage { get; set; }

        public ApiUrls ApiUrls { get; set; }
        public AppUrls AppUrls { get; set; }
    }

    public class AppUrls
    {
        public string Step { get; set; }
        public string StepEditor { get; set; }
    }

    public class ApiUrls
    {
        public string BaselineImage { get; set; }
        public string CurrentImage { get; set; }
        public string DiffImage { get; set; }
        public string CheckpointImage { get; set; }
        public string CheckpointImageThumbnail { get; set; }
    }
}