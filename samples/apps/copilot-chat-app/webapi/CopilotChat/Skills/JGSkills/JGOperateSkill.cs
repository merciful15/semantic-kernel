// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using SemanticKernel.Service.CopilotChat.Options;
using SemanticKernel.Service.CopilotChat.Skills.ChatSkills;
using SemanticKernel.Service.CopilotChat.Storage;

namespace SemanticKernel.Service.CopilotChat.Skills.JGSkills;

internal sealed class JGOperateSkill
{
    private readonly IKernel _kernel;

    public JGOperateSkill(
        IKernel kernel,
        ILogger logger)
    {
        this._kernel = kernel;
    }
    //[SKFunction, Description("Given an e-mail and message body, send an email")]
    //public string SendEmail(
    //    [Description("The body of the email message to send.")] string input,
    //    [Description("The email address to send email to.")] string email_address) =>

    //    $"Sent email to: {email_address}. Body: {input}";

    //[SKFunction, Description("Given a name, find email address")]
    //public string GetEmailAddress(
    //    [Description("The name of the person whose email address needs to be found.")] string input,
    //    ILogger? logger = null)
    //{
    //    logger?.LogDebug("Returning hard coded email for {0}", input);
    //    return "johndoe1234@example.com";
    //}

    //[SKFunction("输出监管版功能菜单地址")]
    //[SKFunctionInput(Description = "输入内容")]
    //[SKFunctionContextParameter(Name = "menuName", Description = "菜单名")]
    //public string GetMenuUrl(string input, SKContext context)
    //{
    //    //logger?.LogDebug("返回监管版内功能菜单 {0}", input);

    //    //string menuName = context["menuName"];

    //    string result = "https://hlq.guansafety.com:8443/";
    //    if (string.IsNullOrEmpty(input))
    //    {
    //        result = "https://hlq.guansafety.com:8443/" + input;
    //    }
    //    else if (input.Contains("企业库", System.StringComparison.Ordinal))
    //    {
    //        result = "https://hlq.guansafety.com:8443/UserCompany/List/OrgCompanyList";
    //    }

    //    return "访问地址是： " + result;
    //}

    [SKFunction("风险辨识")]
    [SKFunctionContextParameter(Name = "riskPoint", Description = "风险点")]
    public string GetRiskInfo(SKContext context)
    {
        //logger?.LogDebug("返回监管版内功能菜单 {0}", input);

        string result = context["riskPoint"];

        return "风险辨识： " + result;
    }

    [SKFunction("获取监管版功能菜单")]
    [SKFunctionInput(Description = "输入内容")]
    public string GetMenuUrl(string input)
    {
        //logger?.LogDebug("返回监管版内功能菜单 {0}", input);
        var resultContext = this._kernel.RunAsync(input, this._kernel.Func("SemanticSkills", "MenuUrl"));

        return "网址： " + resultContext.Result;
    }
}
