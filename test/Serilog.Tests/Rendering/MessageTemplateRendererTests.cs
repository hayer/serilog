namespace Serilog.Tests.Rendering;

public class MessageTemplateRendererTests
{
    readonly MessageTemplateParser _messageTemplateParser = new();

    [Theory]
    [InlineData("{Number}", null, "16")]
    [InlineData("{Number:X8}", null, "00000010")]
    [InlineData("{Number}", "j", "16")]
    [InlineData("{Number:X8}", "j", "00000010")]
    public void PropertyTokenFormatsAreApplied(string template, string? appliedFormat, string expected)
    {
        var eventTemplate = _messageTemplateParser.Parse(template);
        var properties = new Dictionary<string, LogEventPropertyValue> { ["Number"] = new ScalarValue(16) };

        var output = new StringWriter();
        MessageTemplateRenderer.Render(eventTemplate, properties, output, appliedFormat, CultureInfo.InvariantCulture);

        Assert.Equal(expected, output.ToString());
    }

    [Theory]
    [InlineData("Hello {Username}", false, "Hello \"User1\"")]
    [InlineData("Hello {Username}", true, "Hello User1")]
    public void CanApplyGlobalIsLiteral(string template, bool isLiteral, string expected)
    {
        var eventTemplate = _messageTemplateParser.Parse(template);
        var properties = new Dictionary<string, LogEventPropertyValue> { ["Username"] = new ScalarValue("User1") };

        MessageTemplateRenderer.DefaultIsLiteral = isLiteral;

        var output = new StringWriter();
        MessageTemplateRenderer.Render(eventTemplate, properties, output);

        Assert.Equal(expected, output.ToString());
    }
}
