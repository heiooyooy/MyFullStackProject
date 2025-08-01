namespace SharedModule;

public enum SyncEventType { Created, Updated, Deleted }
public record DataSyncEvent(SyncEventType EventType, int EntityId);