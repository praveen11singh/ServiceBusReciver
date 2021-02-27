using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusReciver
{
    class Program
    {
        static QueueClient queueClient;
        static void Main(string[] args)
        {
            string sbConnectionString = "Endpoint=sb://pk11servicebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dXnrp4ojXaX9sNk8I0Pf7M3rsgtAew4FBTkI/FRi95c=";
            string sbQueueName = "pksbqueue";

            try
            {
                queueClient = new QueueClient(sbConnectionString, sbQueueName);

                var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false
                };
                queueClient.RegisterMessageHandler(ReceiveMessagesAsync, messageHandlerOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                queueClient.CloseAsync();
            }
        }

        static async Task ReceiveMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received message: {Encoding.UTF8.GetString(message.Body)}");

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine(exceptionReceivedEventArgs.Exception);
            return Task.CompletedTask;
        }
    }
}
