using System;

namespace TudaSuda
{
    public static class EventHandlerExtender
    {
        public static void InvokeSafe<T>(this EventHandler<T> eventHandler, T message)
        {
            eventHandler?.Invoke(null, message);
        }
    }
}