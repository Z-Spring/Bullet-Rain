using System;

public static class EventManager
{
    public static event Action<string> OnPlayerDeadGlobal;

    public static void TriggerPlayerDead(string playerId)
    {
        OnPlayerDeadGlobal?.Invoke(playerId);
    }
}