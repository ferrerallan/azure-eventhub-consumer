using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

var type = "robot";
async Task Main()
{
    
    Console.ForegroundColor = ConsoleColor.Green;
    string ehubNamespaceConnectionString = "Endpoint=sb://eh-allan.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=zOmvRiUoxoDgKXh5L2IAET/f2zaSjSqZ+kS2t4U8W3E=";
    string eventHubName =  "allan-test";
    string blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=allantestsrg90e8;AccountKey=nRd5mi547fU+XcZSyVX3vIFr5L1BjMYyY27ngDG3p5Dkp5k92WyE4TlbM0AlL0UEu/+5g7ikICXF+AStvkn9xg==;EndpointSuffix=core.windows.net";
    string blobContainerName = "test-eh";

    // Read from the default consumer group: $Default
    string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

    // Create a blob container client that the event processor will use 
    var storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

    // Create an event processor client to process events in the event hub
    var processor = new EventProcessorClient(storageClient, consumerGroup, ehubNamespaceConnectionString, eventHubName);

    // Register handlers for processing events and handling errors
    processor.ProcessEventAsync += ProcessEventHandler;
    processor.ProcessErrorAsync += ProcessErrorHandler;

    // Start the processing
   
    await processor.StartProcessingAsync();
    while(true)
        ;
    //await Task.Delay(TimeSpan.FromSeconds(300));
    //await processor.StopProcessingAsync();

   // await processor.StartProcessingAsync();

    // Wait for 30 seconds for the events to be processed
    //await Task.Delay(TimeSpan.FromSeconds(30));

    // Stop the processing
   // await processor.StopProcessingAsync();
}

int currentPos = 10;
void MoveRobot(string direction)
{
    if (direction == "left")
    {
        currentPos--;
    }
    else
    {
        currentPos++;
    }
    var blank="";
    for (int i = 0; i < currentPos; i++)
    {
        blank+=" ";
    }
    if (type=="robot"){
        Console.Clear();
        Console.WriteLine(" ");
        Console.WriteLine(" ");
        Console.WriteLine(" ");
        Console.WriteLine($"{blank}{direction}");
        Console.WriteLine(" ");
        Console.WriteLine(" ");
        Console.WriteLine(" ");
    }
    else
        Console.WriteLine($"{blank}■");
}

async Task ProcessEventHandler(ProcessEventArgs eventArgs)
{
    // Write the body of the event to the console window
    var message= Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
    if (type=="robot")
        MoveRobot(message);
    else
        {

            Console.WriteLine(message);
        }
    // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
    await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
}

Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
{
    // Write details about the error to the console window
    Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
    Console.WriteLine(eventArgs.Exception.Message);
    return Task.CompletedTask;
}

await Main();