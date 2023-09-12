// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using Microsoft.SemanticKernel.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RepoUtils;
using Resources;

// ReSharper disable once InconsistentNaming
public static class Example19_Qdrant
{
    private const string _Endpoint = "http://192.168.1.25:5000/embeddings";
    private const string _Model = "text2vec-large-chinese";
    private const string MemoryCollectionName = "test-documents";

    public static async Task RunAsync()
    {
        int qdrantPort = int.Parse("6333", CultureInfo.InvariantCulture);


        QdrantMemoryStore memoryStore = new("http://39.152.142.232", qdrantPort, vectorSize: 1536, ConsoleLogger.Log);

        //HttpClient httpClient = new(new HttpClientHandler { CheckCertificateRevocationList = true });
        //httpClient.DefaultRequestHeaders.Add("api-key", "qXVneL-jjDTBll2MMqwtYZu9fntoVBIVM0m0gbRruyDqoovbo95Rlw");

        //QdrantMemoryStore memoryStore = new QdrantMemoryStore(new QdrantVectorDbClient(
        //    "https://70215708-5ff3-4e43-83a0-38f20a807119.us-east-1-0.aws.cloud.qdrant.io", vectorSize: 1536, port: qdrantPort, httpClient: httpClient, log: ConsoleLogger.Log));

        //IKernel kernel = Kernel.Builder
        //    .WithLogger(ConsoleLogger.Log)
        //    .WithOpenAITextCompletionService("text-davinci-003", Env.Var("OPENAI_API_KEY"))
        //    .WithOpenAITextEmbeddingGenerationService("text-embedding-ada-002", Env.Var("OPENAI_API_KEY"))
        //    .WithMemoryStorage(memoryStore)
        //    .Build();

        IKernel kernel = Kernel.Builder
            .WithLogger(ConsoleLogger.Log)
            .WithAzureTextCompletionService("gpt35-chat", "https://gpt3-westeurope.openai.azure.com/", Env.Var("AZURE_OPENAI_KEY"))
            //.WithAzureTextEmbeddingGenerationService("embed-002", "https://gpt3-westeurope.openai.azure.com/", Env.Var("AZURE_OPENAI_KEY"))
            .WithHuggingFaceTextEmbeddingGenerationService(_Model, _Endpoint)
            .WithMemoryStorage(memoryStore)
            .Build();

        Console.WriteLine("== Printing Collections in DB ==");
        var collections = memoryStore.GetCollectionsAsync();
        await foreach (var collection in collections)
        {
            Console.WriteLine(collection);
            ////清空所有数据库
            //await memoryStore.DeleteCollectionAsync(collection);
        }

        var jsonData = EmbeddedResource.Read("peidianxiang.txt");

        //JArray jsonArray = (JArray)JsonConvert.DeserializeObject(jsonData);

        //string sId = string.Empty;
        //for (int i = 0; i < jsonArray.Count; i++)
        //{
        //    var entry = jsonArray[i].ToString();
        //    sId = "data" + i;
        //    await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: sId, text: jsonArray[i].ToString(),description: $"Document: 配电箱json.txt");
        //    //await kernel.Memory.SaveReferenceAsync(
        //    //    collection: MemoryCollectionName,
        //    //    text: entry,
        //    //    externalId: sId,
        //    //    externalSourceName: "GistSource"
        //    //);
        //}

        //Console.WriteLine("== Retrieving Memories Through the Kernel ==");
        //MemoryQueryResult? lookup = await kernel.Memory.GetAsync(MemoryCollectionName, "cat1");
        //Console.WriteLine(lookup != null ? lookup.Metadata.Text : "ERROR: memory not found");

        //lookup = await kernel.Memory.GetAsync(MemoryCollectionName, "data1");
        //Console.WriteLine(lookup != null ? lookup.Metadata.Text : "ERROR: memory not found");

        //Console.WriteLine("== Retrieving Memories Directly From the Store ==");
        //var memory1 = await memoryStore.GetWithPointIdAsync(MemoryCollectionName, key1);
        //var memory2 = await memoryStore.GetWithPointIdAsync(MemoryCollectionName, key2);
        //var memory3 = await memoryStore.GetWithPointIdAsync(MemoryCollectionName, key3);
        //Console.WriteLine(memory1 != null ? memory1.Metadata.Text : "ERROR: memory not found");
        //Console.WriteLine(memory2 != null ? memory2.Metadata.Text : "ERROR: memory not found");
        //Console.WriteLine(memory3 != null ? memory3.Metadata.Text : "ERROR: memory not found");

        //Console.WriteLine("== Similarity Searching Memories: My favorite color is orange ==");
        //var searchResults = kernel.Memory.SearchAsync(MemoryCollectionName, "My favorite color is orange", limit: 3, minRelevanceScore: 0.8);

        //await foreach (var item in searchResults)
        //{
        //    Console.WriteLine(item.Metadata.Text + " : " + item.Relevance);
        //}

        //Console.WriteLine("== Removing Collection {0} ==", MemoryCollectionName);
        //await memoryStore.DeleteCollectionAsync(MemoryCollectionName);

        //通过HuggingFace模型进行测试
        await kernel.Memory.SaveInformationAsync(MemoryCollectionName, id: "001", text: jsonData, description: $"Document: 配电箱json.txt");
        await SearchMemoryAsync(kernel, "对于内部含有电器、电线等物体的顶棚，使用燃烧性能过低的木质板材装修，这个隐患对应的检查依据是什么");

        Console.WriteLine("== Printing Collections in DB ==");
        await foreach (var collection in collections)
        {
            Console.WriteLine(collection);
        }
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
}
