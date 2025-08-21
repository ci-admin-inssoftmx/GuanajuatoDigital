using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace GuanajuatoAdminUsuarios.Utils
{
    public static class HtmlExtension
    {
        //public static HtmlString IsReadonly(this HtmlString htmlString, bool @readonly)
        //{
        //    string rawstring = htmlString.ToString();
        //    if (@readonly)
        //    {
        //        rawstring = rawstring.Insert(rawstring.Length - 2, "readonly=\"readonly\"");
        //    }
        //    return new HtmlString(rawstring);
        //}

        public static IHtmlContent CustomLabelFor<TModel, TProperty>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            string result;

            TagBuilder div = new TagBuilder("div");
            div.MergeAttribute("class", "form-group");
            var label = helper.LabelFor(expression, new { @class = "control-label col-lg-1" });
            div.InnerHtml.AppendHtml(label);

            using (var sw = new System.IO.StringWriter())
            {
                div.WriteTo(sw, System.Text.Encodings.Web.HtmlEncoder.Default);
                result = sw.ToString();
            }

            return new HtmlString(result);
        }

        public static IHtmlContent CustomTextBoxFor<TModel, TProperty>(this IHtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string placeholderval, bool @readonlyVal)
        {
            string result;

            TagBuilder span = new TagBuilder("span");
            span.MergeAttribute("class", "k-widget k-textbox");
            span.MergeAttribute("style", "width:100%;");
            IHtmlContent textbox;
            if (@readonlyVal)
            {
                textbox = helper.TextBoxFor(expression, new
                {
                    placeholder = placeholderval,
                    @class = "k-input k-state-disabled",
                    style = "width:100%;",
                    @readonly = "readonly",
                    @autocomplete = "off"
                });
            }
            else
            {
                textbox = helper.TextBoxFor(expression, new { placeholder = placeholderval, @class = "k-input", style = "width:100%;" });
            }

            span.InnerHtml.AppendHtml(textbox);
            using (var sw = new System.IO.StringWriter())
            {
                span.WriteTo(sw, System.Text.Encodings.Web.HtmlEncoder.Default);
                result = sw.ToString();
            }
            //return new HtmlString(rawstring);

            return new HtmlString(result);
        }




    }
}
