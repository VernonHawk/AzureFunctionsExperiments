using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;

namespace Nanoservices
{
    public static class DeletePost
    {
        [FunctionName("DeletePost")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(
                authLevel: AuthorizationLevel.Function,
                "delete",
                Route = "posts/{author}/{id}"
            )]
            HttpRequest req,
            [CosmosDB(
                databaseName: ConnectionParams.DatabaseName,
                collectionName: ConnectionParams.CollectionName,
                ConnectionStringSetting = ConnectionParams.DbConnectionStringSetting
            )]
            DocumentClient client,
            string author,
            string id,
            ILogger log
        )
        {
            log.LogInformation($"DeletePost {id}");

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId: ConnectionParams.DatabaseName,
                collectionId: ConnectionParams.CollectionName
            );

            var posts = client
                        .CreateDocumentQuery<Document>(
                            documentCollectionUri: collectionUri,
                            feedOptions: new FeedOptions {EnableCrossPartitionQuery = true}
                        )
                        .Where(d => d.Id == id);

            foreach (var doc in posts)
            {
                await client.DeleteDocumentAsync(
                    documentLink: doc.SelfLink,
                    options: new RequestOptions {PartitionKey = new PartitionKey(author)}
                );
            }

            return new OkResult();
        }
    }
}