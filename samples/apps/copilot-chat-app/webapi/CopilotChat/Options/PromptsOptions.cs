// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SemanticKernel.Service.Options;

namespace SemanticKernel.Service.CopilotChat.Options;

/// <summary>
/// Configuration options for the chat
/// </summary>
public class PromptsOptions
{
    public const string PropertyName = "Prompts";

    /// <summary>
    /// Token limit of the chat model.
    /// </summary>
    /// <remarks>https://platform.openai.com/docs/models/overview for token limits.</remarks>
    [Required, Range(0, int.MaxValue)] public int CompletionTokenLimit { get; set; }

    /// <summary>
    /// The token count left for the model to generate text after the prompt.
    /// </summary>
    [Required, Range(0, int.MaxValue)] public int ResponseTokenLimit { get; set; }

    /// <summary>
    /// Weight of memories in the contextual part of the final prompt.
    /// Contextual prompt excludes all the system commands and user intent.
    /// </summary>
    internal double MemoriesResponseContextWeight { get; } = 0.3;

    /// <summary>
    /// Weight of documents in the contextual part of the final prompt.
    /// Contextual prompt excludes all the system commands and user intent.
    /// </summary>
    internal double DocumentContextWeight { get; } = 0.3;

    /// <summary>
    /// Weight of information returned from planner (i.e., responses from OpenAPI skills).
    /// Contextual prompt excludes all the system commands and user intent.
    /// </summary>
    internal double ExternalInformationContextWeight { get; } = 0.3;

    /// <summary>
    /// Minimum relevance of a semantic memory to be included in the final prompt.
    /// The higher the value, the answer will be more relevant to the user intent.
    /// </summary>
    internal double SemanticMemoryMinRelevance { get; } = 0.8;

    /// <summary>
    /// Minimum relevance of a document memory to be included in the final prompt.
    /// The higher the value, the answer will be more relevant to the user intent.
    /// </summary>
    internal double DocumentMemoryMinRelevance { get; } = 0.8;

    // System
    [Required, NotEmptyOrWhitespace] public string KnowledgeCutoffDate { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string InitialBotMessage { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string SystemDescription { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string SystemResponse { get; set; } = string.Empty;

    internal string[] SystemAudiencePromptComponents => new string[]
    {
        this.SystemAudience,
        "{{ChatSkill.ExtractChatHistory}}",
        this.SystemAudienceContinuation
    };

    internal string SystemAudienceExtraction => string.Join("\n", this.SystemAudiencePromptComponents);

    internal string[] SystemIntentPromptComponents => new string[]
    {
        this.SystemDescription,
        this.SystemIntent,
        "{{ChatSkill.ExtractChatHistory}}",
        this.SystemIntentContinuation,
        this.SystemIntentFormat
    };

    internal string SystemIntentExtraction => string.Join("\n", this.SystemIntentPromptComponents);

    // Intent extraction
    [Required, NotEmptyOrWhitespace] public string SystemIntent { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string SystemIntentContinuation { get; set; } = string.Empty;

    //sck add 2023-06-21
    [Required, NotEmptyOrWhitespace] public string SystemIntentFormat { get; set; } = string.Empty;
    //End

    // Audience extraction
    [Required, NotEmptyOrWhitespace] public string SystemAudience { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string SystemAudienceContinuation { get; set; } = string.Empty;

    // Memory extraction
    [Required, NotEmptyOrWhitespace] public string SystemCognitive { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string MemoryFormat { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string MemoryAntiHallucination { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string MemoryContinuation { get; set; } = string.Empty;

    // Long-term memory
    [Required, NotEmptyOrWhitespace] public string LongTermMemoryName { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string LongTermMemoryExtraction { get; set; } = string.Empty;

    internal string[] LongTermMemoryPromptComponents => new string[]
    {
        this.SystemCognitive,
        $"{this.LongTermMemoryName} Description:\n{this.LongTermMemoryExtraction}",
        this.MemoryAntiHallucination,
        $"Chat Description:\n{this.SystemDescription}",
        "{{ChatSkill.ExtractChatHistory}}",
        this.MemoryContinuation
    };

    internal string LongTermMemory => string.Join("\n", this.LongTermMemoryPromptComponents);

    // Working memory
    [Required, NotEmptyOrWhitespace] public string WorkingMemoryName { get; set; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string WorkingMemoryExtraction { get; set; } = string.Empty;

    internal string[] WorkingMemoryPromptComponents => new string[]
    {
        this.SystemCognitive,
        $"{this.WorkingMemoryName} Description:\n{this.WorkingMemoryExtraction}",
        this.MemoryAntiHallucination,
        $"Chat Description:\n{this.SystemDescription}",
        "{{ChatSkill.ExtractChatHistory}}",
        this.MemoryContinuation
    };

    internal string WorkingMemory => string.Join("\n", this.WorkingMemoryPromptComponents);

    // Memory map
    internal IDictionary<string, string> MemoryMap => new Dictionary<string, string>()
    {
        { this.LongTermMemoryName, this.LongTermMemory },
        { this.WorkingMemoryName, this.WorkingMemory }
    };

    // Chat commands 这个是聊天命令，表示后面开始输出内容
    internal string SystemChatContinuation = "SINGLE RESPONSE FROM Xiaoan TO USER:\n[{{TimeSkill.Now}} {{timeSkill.Second}}] Xiaoan:";

    internal string[] SystemChatPromptComponents => new string[]
    {
        this.SystemDescription,
        this.SystemResponse,
        "{{$audience}}",
        "{{$userIntent}}",
        "{{$chatContext}}",
        this.SystemChatContinuation
    };

    internal string SystemChatPrompt => string.Join("\n\n", this.SystemChatPromptComponents);

    internal double ResponseTemperature { get; } = 0.7;
    internal double ResponseTopP { get; } = 1;
    internal double ResponsePresencePenalty { get; } = 0.5;
    internal double ResponseFrequencyPenalty { get; } = 0.5;

    internal double IntentTemperature { get; } = 0.7;
    internal double IntentTopP { get; } = 1;
    internal double IntentPresencePenalty { get; } = 0.5;
    internal double IntentFrequencyPenalty { get; } = 0.5;


    //sck add
    //[Required, NotEmptyOrWhitespace] public string SystemIntentExamples { get; set; } = string.Empty;
}
