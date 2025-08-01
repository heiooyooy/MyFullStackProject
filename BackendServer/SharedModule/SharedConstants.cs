namespace SharedModule;

public static class SharedConstants
{
    public const string OUTBOX_SIGNAL_CHANNEL = "outbox_new_message_signal";

    public const string OUTBOX_MESSAGE_EXCHANGE = "outboxMessage.exchange";
    public const string OUTBOX_MESSAGE_ROUTINGKEY = "outboxMessage.new";
    
    public const string OUTBOX_MESSAGE_ROUTINGMATCH = "outboxMessage.*";
    
}