// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace SemanticKernel.Service.CopilotChat.Models;

/// <summary>
/// 对用户意愿进行分类
/// </summary>
public class IntentCategory
{
    /// <summary>
    /// 分类
    /// </summary>
    [JsonPropertyName("Category")]
    public string Category { get; set; }

    [JsonPropertyName("Intent")]
    public string Intent { get; set; }


    public IntentCategory(string category, string intent)
    {
        this.Category = category;
        this.Intent = intent;
    }
}
