using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MVC.TagHelpers;

[HtmlTargetElement("icon", TagStructure = TagStructure.WithoutEndTag)]
public class IconTagHelper : TagHelper
{
    public string Name { get; set; } = "";
    public int? Size { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "i";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("data-lucide", Name);

        if (Size != null)
        {
            output.Attributes.SetAttribute("width", Size.ToString());
            output.Attributes.SetAttribute("height", Size.ToString());
        }
    }
}