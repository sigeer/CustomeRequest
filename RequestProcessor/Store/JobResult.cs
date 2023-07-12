namespace RequestProcessor
{
    public class JobResult
    {
        public int Id { get; set; }
        public int JobDetailId { get; set; }
        public DateTime CreationTime { get; set; }
        public string? Result { get; set; }

        public JobResult() { }

        public JobResult(int jobDetailId, string? result)
        {
            JobDetailId = jobDetailId;
            CreationTime = DateTime.Now;
            Result = result;
        }
    }
}
