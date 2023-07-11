namespace RequestProcessor
{
    public class JobStore
    {
        public JobStore()
        {
        }

        public JobStore(string jobId, int jobDetailId, string name, string cron, string? remark = null)
        {
            JobId = jobId;
            Remark = remark;
            Name = name;
            Cron = cron;
            JobDetailId = jobDetailId;
            Status = 1;
        }


        public int Id { get; set; }
        /// <summary>
        /// hangfire.jobid
        /// </summary>
        public string JobId { get; set; } = null!;

        public string? Remark { get; set; }
        public string Name { get; set; } = null!;
        public string Cron { get; set; } = null!;
        public int JobDetailId { get; set; }
        /// <summary>
        /// 1.on 0.off
        /// </summary>
        public int Status { get;set; }
    }
}
