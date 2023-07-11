using Hangfire;
using RequestProcessor.Models;
using ResponseProcessor;
using System.Collections;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Utility.Extensions;
using Utility.HttpRequest.Core;

namespace RequestProcessor
{
    public class JobDataLogic
    {
        readonly IFreeSql _freeSql;

        public JobDataLogic(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        public List<JobTableViewModel> GetTableView()
        {
            var jobStoreList = _freeSql.Select<JobStore>().ToList();
            return jobStoreList.Select(x => new JobTableViewModel()
            {
                Name = x.Name,
                Remark = x.Remark,
                Cron = x.Cron,
                JobDetailId = x.JobDetailId,
                JobId = x.JobId,
                JobStoreId = x.Id,
                Status = x.Status
            }).ToList();
        }

        public List<HttpRequestUnit> GetJobDetailList()
        {
            return _freeSql.Select<JobDetail>().ToList().Select(x => new HttpRequestUnit
            {
                Id = x.Id,
                Name = x.Name,
                Method = x.Method,
                Url = x.Url,
                Steps = x.GetSteps(),
                HttpRequestBody = x.HttpRequestBody,
                HttpRequestHeader = x.GetHeaderObj()
            }).ToList();
        }

        public void AddOrUpdateJob(JobTableEditoModel model)
        {
            var obj = new JobStore()
            {
                Id = model.JobStoreId ?? 0,
                Name = model.Name,
                JobId = model.JobId,
                Cron = model.Cron,
                Remark = model.Remark,
                JobDetailId = model.JobDetailId,
                Status = 1
            };
            if (obj.Id > 0)
                _freeSql.Update<JobStore>().SetSource(obj).Where(x => x.Id == obj.Id).IgnoreColumns(x => x.JobId).ExecuteAffrows();
            else
                obj.Id = (int)_freeSql.Insert<JobStore>(obj).IgnoreColumns(x => x.Id).ExecuteIdentity();

            var oldJobId = _freeSql.Select<JobStore>().Where(x => x.Id == obj.Id).ToOne().JobId;
            RecurringJob.AddOrUpdate(oldJobId, () => DoJob(obj.Id), obj.Cron, TimeZoneInfo.Local);
        }

        public void DeleteJob(int jobStoreId)
        {
            var model = GetJobStore(jobStoreId);
            if (model != null)
            {
                RecurringJob.RemoveIfExists(model.JobId);
                DeleteJobStorage(model.Id);
            }
        }

        private JobStore GetJobStore(int jobStoreId)
        {
            return _freeSql.Select<JobStore>().Where<JobStore>(x => x.Id == jobStoreId).ToOne();
        }

        private void DeleteJobStorage(int jobStoreId)
        {
            _freeSql.Delete<JobStore>(jobStoreId).ExecuteAffrows();
        }

        public void JobOff(int jobStoreId)
        {
            _freeSql.Update<JobStore>(jobStoreId).Set(x => x.Status, 0).ExecuteAffrows();
        }

        public void JobOn(int jobStoreId)
        {
            _freeSql.Update<JobStore>(jobStoreId).Set(x => x.Status, 1).ExecuteAffrows();
        }



        public async Task<object?> DoJob(int id)
        {
            var jobStore = _freeSql.Select<JobStore>().Where(x => x.Id == id).ToOne();
            if (jobStore != null && jobStore.Status == 1)
            {
                return await DoJobDetail(jobStore.JobDetailId);
            }
            return null;
        }

        public async Task<object?> DoJobDetail(int id)
        {
            var jobDetail = _freeSql.Select<JobDetail>().Where(x => x.Id == id).ToOne();
            if (jobDetail == null)
            {
                Console.Write("no job");
                DeleteJob(id);
                return null;
            }
            else
            {
                return await DoJobByDetail(jobDetail);
            }
        }

        public void AddOrUpdateJobDetail(HttpRequestUnit dto)
        {
            var jobDetail = new JobDetail
            {
                Id = dto.Id,
                Name = dto.Name,
                Method = dto.Method,
                Url = dto.Url,
                StepJson = JsonSerializer.Serialize(dto.Steps),
                HttpRequestHeader = JsonSerializer.Serialize(dto.HttpRequestHeader),
                HttpRequestBody = dto.HttpRequestBody
            };
            if (jobDetail.Id > 0)
                _freeSql.Update<JobDetail>().SetSource(jobDetail).Where(x => x.Id == jobDetail.Id).ExecuteAffrows();
            else
                _freeSql.Insert<JobDetail>(jobDetail).IgnoreColumns(x => x.Id).ExecuteAffrows();
        }

        public void DeleteJobDetail(int id)
        {
            _freeSql.Delete<JobDetail>(id).ExecuteAffrows();
        }

        public async Task<object?> DoJobByDetail(JobDetail jobDetail)
        {
            var content = await CustomeWebRequest.RequestDocumentContent(jobDetail.Url, new RequestObjViewModel
            {
                HeaderStr = jobDetail.HttpRequestHeader,
                PostObjStr = jobDetail.HttpRequestBody,
                Method = jobDetail.Method
            });

            object? lastResult = content;
            var allSteps = jobDetail.GetSteps();
            if (allSteps.Count > 0)
            {
                for (int i = 0; i < allSteps.Count; i++)
                {
                    lastResult = allSteps[i].Run(lastResult);
                }
            }

            if (lastResult != null)
            {
                if (lastResult is string)
                {
                    await _freeSql.Insert(new JobResult(jobDetail.Id, lastResult?.ToString())).IgnoreColumns(x => x.Id).ExecuteAffrowsAsync();
                }
                else
                {
                    var datas = (IEnumerable)lastResult;
                    var list = new List<JobResult>();
                    foreach (var item in datas)
                    {
                        list.Add(new JobResult(jobDetail.Id, item.ToString()));
                    }
                    await _freeSql.Insert(list).IgnoreColumns(x => x.Id).ExecuteAffrowsAsync();
                }

            }
            return lastResult;
        }

        public List<JobResult> GetJobResultList(int jobDetailId)
        {
            return _freeSql.Select<JobResult>().Where(x => x.JobDetailId == jobDetailId).OrderByDescending(x => x.CreationTime)
                .ToList(x => new JobResult { Result = x.Result, CreationTime = x.CreationTime });
        }

        public List<ActionOptionViewModel> GetActionOptionList()
        {
            return OptionCollection.Default.Select(x => new ActionOptionViewModel
            {
                Id = x.Id,
                Name = x.DisplayName,
                FromType = x.FromType.ToString(),
                ReturnType = x.ReturnType.ToString(),
                Params = x.Params
            }).ToList();
        }
    }

    public class RequestObjViewModel
    {
        /// <summary>
        /// "POST" or "GET"
        /// </summary>
        public string? Method { get; set; }
        /// <summary>
        /// object => json
        /// </summary>
        public string? PostObjStr { get; set; }
        /// <summary>
        /// <see cref="List{SpiderHeaderDto}"/> => json
        /// </summary>
        public string? HeaderStr { get; set; }
        public HttpRequestHeaderItem[] GetHeaders()
        {
            return JsonSerializer.Deserialize<HttpRequestHeaderItem[]>(HeaderStr ?? "[]") ?? new HttpRequestHeaderItem[] { };
        }

        public HttpContent? GetPostContent()
        {
            if (Method == "GET")
                return null;

            if (string.IsNullOrEmpty(PostObjStr))
                return null;

            var headers = GetHeaders();

            var contentTypeItem = headers.FirstOrDefault(x => x.IsContentType());
            if (contentTypeItem != null && contentTypeItem.Value.StartsWith("application/x-www-form-urlencoded"))
                return new FormUrlEncodedContent(JsonConverter.ConvertJsonToKeyValuePairs(PostObjStr));

            return JsonContent.Create(JsonSerializer.Deserialize<object>(PostObjStr));
        }
    }

    public class CustomeWebRequest
    {
        public static async Task<string> RequestDocumentContent(string url, RequestObjViewModel spiderConfig, CancellationToken cancellationToken = default)
        {
            var requestConfig = new HttpRequestMessage();
            requestConfig.RequestUri = new Uri(url);
            if (!string.IsNullOrEmpty(spiderConfig.Method))
                requestConfig.Method = new HttpMethod(spiderConfig.Method);

            var postModel = spiderConfig.GetPostContent();
            if (postModel != null)
                requestConfig.Content = postModel;

            var headerObj = spiderConfig.GetHeaders();
            foreach (var item in headerObj)
            {
                requestConfig.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }

            using (HttpClient httpClient = new HttpClient())
            {
                var res = await httpClient.HttpSendCore(requestConfig, cancellationToken);
                var responseStream = await res.Content.ReadAsStreamAsync(cancellationToken);
                return responseStream.DecodeData(res.Content.Headers.ContentType?.CharSet);
            }

        }
    }
}
