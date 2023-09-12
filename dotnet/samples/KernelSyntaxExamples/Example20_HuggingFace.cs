// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using RepoUtils;
using Microsoft.SemanticKernel.Memory;
using static Grpc.Core.Metadata;
using System.Collections.Generic;

/**
 * The following example shows how to use Semantic Kernel with HuggingFace API.
 */

// ReSharper disable once InconsistentNaming
public static class Example20_HuggingFace
{
    private const string _Endpoint = "http://192.168.1.25:5000/embeddings";
    private const string _Model = "text2vec-large-chinese";
    private const string MemoryCollectionName = "SKGitHub";

    public static async Task RunAsync()
    {
        Console.WriteLine("======== HuggingFace text completion AI ========");

        IKernel kernel = new KernelBuilder()
            .WithLogger(ConsoleLogger.Log)
            //.WithHuggingFaceTextCompletionService("gpt2", apiKey: Env.Var("HF_API_KEY"))
            .WithAzureChatCompletionService("gpt35-chat", "https://gpt3-westeurope.openai.azure.com/", "01abe965c6ee4f5fa7b73f35b78ea78b")
            .WithHuggingFaceTextEmbeddingGenerationService(_Model, _Endpoint)
            .WithMemoryStorage(new VolatileMemoryStore())
            .Build();

        await RunExampleAsync(kernel);

        const string FunctionDefinition = "Question: {{$input}}; Answer:";

        var questionAnswerFunction = kernel.CreateSemanticFunction(FunctionDefinition);

        var result = await questionAnswerFunction.InvokeAsync("What is New York?");

        Console.WriteLine(result);

        foreach (var modelResult in result.ModelResults)
        {
            Console.WriteLine(modelResult.GetHuggingFaceResult().AsJson());
        }
    }

    public static async Task RunExampleAsync(IKernel kernel)
    {
        await StoreMemoryAsync(kernel);

        await SearchMemoryAsync(kernel, "How do I get started?");

        /*
        Output:

        Query: How do I get started?

        Result 1:
          URL:     : https://github.com/microsoft/semantic-kernel/blob/main/README.md
          Title    : README: Installation, getting started, and how to contribute

        Result 2:
          URL:     : https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet-jupyter-notebooks/00-getting-started.ipynb
          Title    : Jupyter notebook describing how to get started with the Semantic Kernel

        */

        await SearchMemoryAsync(kernel, "Can I build a chat with SK?");

        /*
        Output:

        Query: Can I build a chat with SK?

        Result 1:
          URL:     : https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/ChatSkill/ChatGPT
          Title    : Sample demonstrating how to create a chat skill interfacing with ChatGPT

        Result 2:
          URL:     : https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md
          Title    : README: README associated with a sample chat summary react-based webapp

        */
    }

    private static async Task SearchMemoryAsync(IKernel kernel, string query)
    {
        Console.WriteLine("\nQuery: " + query + "\n");

        var memories = kernel.Memory.SearchAsync(MemoryCollectionName, query, limit: 2, minRelevanceScore: 0.5);

        int i = 0;
        await foreach (MemoryQueryResult memory in memories)
        {
            Console.WriteLine($"Result {++i}:");
            Console.WriteLine("  URL:     : " + memory.Metadata.Id);
            Console.WriteLine("  Title    : " + memory.Metadata.Description);
            Console.WriteLine();
        }

        Console.WriteLine("----------------------");
    }

    private static async Task StoreMemoryAsync(IKernel kernel)
    {
        /* Store some data in the semantic memory.
         *
         * When using Azure Cognitive Search the data is automatically indexed on write.
         *
         * When using the combination of VolatileStore and Embedding generation, SK takes
         * care of creating and storing the index
         */

        Console.WriteLine("\nAdding some GitHub file URLs and their descriptions to the semantic memory.");
        var githubFiles = SampleData();
        var i = 0;
        foreach (var entry in githubFiles)
        {
            await kernel.Memory.SaveReferenceAsync(
                collection: MemoryCollectionName,
                description: entry.Value,
                text: entry.Value,
                externalId: entry.Key,
                externalSourceName: "GitHub"
            );
            Console.Write($" #{++i} saved.");
        }

        Console.WriteLine("\n----------------------");
    }

    private static Dictionary<string, string> SampleData()
    {
        return new Dictionary<string, string>
        {
            ["https://github.com/microsoft/semantic-kernel/blob/main/README.md"]
                = "README: Installation, getting started, and how to contribute",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/02-running-prompts-from-file.ipynb"]
                = "Jupyter notebook describing how to pass prompts from a file to a semantic skill or function",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/00-getting-started.ipynb"]
                = "Jupyter notebook describing how to get started with the Semantic Kernel",
            ["https://github.com/microsoft/semantic-kernel/tree/main/samples/skills/ChatSkill/ChatGPT"]
                = "Sample demonstrating how to create a chat skill interfacing with ChatGPT",
            ["https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/SemanticKernel/Memory/VolatileMemoryStore.cs"]
                = "C# class that defines a volatile embedding store",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/dotnet/KernelHttpServer/README.md"]
                = "README: How to set up a Semantic Kernel Service API using Azure Function Runtime v4",
            ["https://github.com/microsoft/semantic-kernel/blob/main/samples/apps/chat-summary-webapp-react/README.md"]
                = "README: README associated with a sample chat summary react-based webapp",
        };
    }
}
