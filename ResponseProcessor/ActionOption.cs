using AngleSharp.Dom;

namespace ResponseProcessor
{
    public class ActionOption
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = null!;
        public ReadType FromType { get; set; }
        public ReadType ReturnType { get; set; }
        public string MethodName { get; set; } = null!;
        public List<string> Params { get; set; } = new List<string>();

        public static ActionOption ReadJson()
        {
            return new ActionOption
            {
                Id = 1,
                DisplayName = "读取Json",
                MethodName = nameof(ActionFactory.ReadJsonProperty),
                Params = new List<string> { "Json属性:" },
                FromType = ReadType.String,
                ReturnType = ReadType.String
            };
        }

        public static ActionOption ReadHtmlElement()
        {
            return new ActionOption
            {
                Id = 2,
                DisplayName = "读取Html元素",
                MethodName = nameof(ActionFactory.ReadElements),
                Params = new List<string> { "选择器:" },
                FromType = ReadType.String,
                ReturnType = ReadType.Elements
            };
        }

        public static ActionOption ReadHtmlText()
        {
            return new ActionOption
            {
                Id = 3,
                DisplayName = "读取元素Text",
                MethodName = nameof(ActionFactory.ReadTextFromElement),
                FromType = ReadType.Elements,
                ReturnType = ReadType.String
            };
        }
        public static ActionOption ReadInnerHtmlFromElement()
        {
            return new ActionOption
            {
                Id = 4,
                DisplayName = "读取元素InnerHtml",
                MethodName = nameof(ActionFactory.ReadInnertHtmlFromElement),
                FromType = ReadType.Elements,
                ReturnType = ReadType.String
            };
        }
        public static ActionOption ReadAttributeFromElement()
        {
            return new ActionOption
            {
                Id = 5,
                DisplayName = "读取元素Attribute",
                MethodName = nameof(ActionFactory.ReadAttributeFromElement),
                Params = new List<string> { "Attribute:" },
                FromType = ReadType.Elements,
                ReturnType = ReadType.String
            };
        }
        public static ActionOption RegReplace()
        {
            return new ActionOption
            {
                Id = 6,
                DisplayName = "替换字符串（正则）",
                MethodName = nameof(ActionFactory.RegReplace),
                Params = new List<string> { "正则表达式:", "替换字符串:" },
                FromType = ReadType.String,
                ReturnType = ReadType.String
            };
        }

        public static ActionOption Download()
        {
            return new ActionOption
            {
                Id = 7,
                DisplayName = "下载",
                MethodName = nameof(ActionFactory.Download),
                FromType = ReadType.String,
                ReturnType = ReadType.String
            };
        }
    }
}
