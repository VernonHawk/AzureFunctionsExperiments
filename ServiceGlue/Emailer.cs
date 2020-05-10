using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using SendGrid.Helpers.Mail;

namespace ServiceGlue
{
    public static class Emailer
    {
[FunctionName("Emailer")]
        public static async Task EmailerFun(
            [CosmosDBTrigger(
                databaseName: ConnectionParams.Database,
                collectionName: ConnectionParams.PostsCollection,
                ConnectionStringSetting = ConnectionParams.DbConnectionStringSetting,
                CreateLeaseCollectionIfNotExists = true,
                LeaseCollectionName = "leases"
            )]
            IReadOnlyList<Document> posts,
            [CosmosDB(
                databaseName: ConnectionParams.Database,
                collectionName: ConnectionParams.PostsCollection,
                ConnectionStringSetting = ConnectionParams.DbConnectionStringSetting
            )]
            DocumentClient client,
            [SendGrid(ApiKey = "SendGridApiKey")] IAsyncCollector<SendGridMessage> collector
        )
        {
            var authorsUri = UriFactory.CreateDocumentCollectionUri(
                databaseId: ConnectionParams.Database,
                collectionId: ConnectionParams.AuthorsCollection
            );

            foreach (var doc in posts)
            {
                var author = doc.GetPropertyValue<string>("Author");

                var messages = client
                               .CreateDocumentQuery<Author>(
                                   documentCollectionUri: authorsUri,
                                   feedOptions: new FeedOptions {EnableCrossPartitionQuery = true}
                               )
                               .Where(a => a.Name == author)
                               .Select(a => a.Subscribers)
                               .ToList()
                               .FirstOrDefault()
                               .Select(
                                   sub => ComposeMessage(
                                       from: "i.morenets@ukma.edu.ua",
                                       to: sub,
                                       author: author,
                                       title: doc.GetPropertyValue<string>("Title")
                                   )
                               );

                foreach (var message in messages)
                {
                    await collector.AddAsync(message);
                }
            }
        }

        private static SendGridMessage ComposeMessage(
            string from,
            string author,
            string title,
            string to
        )
        {
            var message = new SendGridMessage
            {
                From = new EmailAddress(from),
                Subject = "New post by the author you are subscribed to!",
                PlainTextContent = $"{author} has just published a new post {title}!"
            };

            message.AddTo(new EmailAddress(to));

            return message;
        }
    }
}