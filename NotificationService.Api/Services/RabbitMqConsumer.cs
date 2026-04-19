using Microsoft.EntityFrameworkCore;
using NotificationService.Api.Data;
using NotificationService.Api.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationService.Api.Services
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private RabbitMQ.Client.IConnection? _connection;
        private RabbitMQ.Client.IModel? _channel;

        public RabbitMqConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost"
            };

            int retries = 5;
            int delayMilliseconds = 3000;

            while (retries > 0)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    _channel.QueueDeclare(
                        queue: "order-created",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    Console.WriteLine("Connected to RabbitMQ");
                    break;
                }
                catch (Exception ex)
                {
                    retries--;
                    Console.WriteLine($"RabbitMQ not ready, retrying... Remaining tries: {retries}");
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(delayMilliseconds);
                }
            }

            if (_connection == null || _channel == null)
            {
                throw new Exception("Could not connect to RabbitMQ after several retries.");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($"[Notification Service] Received: {message}");

                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var notification = new Notification
                    {
                        OrderId = 0,
                        Message = message,
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.Notifications.Add(notification);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while processing RabbitMQ message: {ex.Message}");
                }
            };

            _channel.BasicConsume(
                queue: "order-created",
                autoAck: true,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}