using System;

namespace DefaultNamespace
{
    public class Events
    {
        public static Action<bool> playerStartMovement;
        public static Action<bool> playerStopMovement;
        public static Action playerTriggerSetPosition;
    }
}