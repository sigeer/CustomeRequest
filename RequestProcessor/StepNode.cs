using ResponseProcessor;
using System.Reflection;

namespace RequestProcessor
{
    public class StepNode
    {
        public int ActionOptionId { get; set; }
        public List<string> Params { get; set; } = new List<string>();
        public object? Result { get; set; }

        public object? Run(object? lastData)
        {
            var option = GetActionOption();
            var method = typeof(ActionFactory).GetMethod(option.MethodName, BindingFlags.Static | BindingFlags.Public);
            if (method == null)
                throw new ArgumentNullException(option.MethodName);

            Result = method.Invoke(null, lastData == null ? Params.ToArray() : new object[] { lastData }.Concat(Params).ToArray());
            if (Result is Task)
            {
                var resultProperty = method.ReturnType.GetProperty("Result");
                if (resultProperty != null)
                    Result = resultProperty.GetValue(Result);
            }
            return Result;
        }

        public ActionOption GetActionOption()
        {
            return OptionCollection.Default.FirstOrDefault(x => x.Id == ActionOptionId)!; ;
        }

    }
}
