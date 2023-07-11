namespace ResponseProcessor
{
    public class OptionCollection
    {
        public static List<ActionOption> Default = new List<ActionOption>
        {
            ActionOption.ReadJson(),
            ActionOption.ReadHtmlElement(),
            ActionOption.ReadHtmlText(),
            ActionOption.ReadInnerHtmlFromElement(),
            ActionOption.ReadAttributeFromElement(),
            ActionOption.RegReplace(),
            ActionOption.Download()
        };
    }
}
