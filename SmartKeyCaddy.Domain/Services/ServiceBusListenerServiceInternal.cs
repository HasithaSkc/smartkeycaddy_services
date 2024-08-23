using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartKeyCaddy.Common;
using SmartKeyCaddy.Domain.Contracts;
using SmartKeyCaddy.Domain.Repository;
using SmartKeyCaddy.Models;
using SmartKeyCaddy.Models.Messages;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml;

namespace SmartKeyCaddy.Domain.Services;

public partial class ServiceBusListenerService
{
    private async Task ProcessMessagesAsync(Message message, CancellationToken token)
    {
        string messageBody = string.Empty;
        bool success = false;

        try
        {
            messageBody = Encoding.UTF8.GetString(message.Body) ?? string.Empty;

            var messageType = GetMessageType(messageBody);

            switch (messageType)
            {
                case MessageType.KeyTransaction:
                    await ProcessDeviceKeyTransaction(messageBody);
                    break;
                case MessageType.DeviceRegistration:
                    await ProcessDeviceRegistration(messageBody);
                    break;
            }
            success = true;

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessMessagesAsync");
        }
        finally 
        {
            await InsertIntoServiceBusMessageQueue(messageBody, success);
        }
    }

    private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        _logger.LogError($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
        return Task.CompletedTask;
    }

    private MessageType GetMessageType(string messageStr)
    {
        JObject jsonObject1 = JObject.Parse(messageStr);

        var jsonObject = JsonConvert.DeserializeObject<dynamic>(messageStr);
        var messageType = jsonObject?.messageType?.Value;

        if (!Enum.TryParse(messageType, true, out MessageType deviceMessageType))
            return MessageType.Unknown;

        return deviceMessageType;
    }

    private async Task ProcessDeviceKeyTransaction(string messageBody)
    {
        var keyTransactionMessage = JsonConvert.DeserializeObject<KeyTransactionMessage>(messageBody);

        if (keyTransactionMessage == null)
            return;

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedService = scope.ServiceProvider.GetRequiredService<IKeyAllocationService>();
            await scopedService.ProcessDeviceKeyTransaction(keyTransactionMessage);
        }
    }

    private async Task ProcessDeviceRegistration(string messageBody)
    {
        var deviceRegisterMessage = JsonConvert.DeserializeObject<DeviceRegisterMessage>(messageBody);

        if (deviceRegisterMessage == null)
            return;

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedService = scope.ServiceProvider.GetRequiredService<IAdminService>();
            await scopedService.RegisterDevice(deviceRegisterMessage);
        }
    }

    private async Task InsertIntoServiceBusMessageQueue(string messageBody, bool success)
    {
        if (string.IsNullOrEmpty(messageBody))
            return;

        var baseMessage = JsonConvert.DeserializeObject<BaseMessage>(messageBody);

        if (baseMessage == null) return;

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedService = scope.ServiceProvider.GetRequiredService<IMessageQueueRepository>();
            await scopedService.InsertMessage(new ServiceBusMessage()
            {
                DeviceId = baseMessage.DeviceId,
                DeviceName = baseMessage.DeviceName,
                MessageBody = messageBody,
                IsProcessed = success,
                EnqueuedDateTime = baseMessage.EnqueuedDateTime
            });
        }
        
    }
}
