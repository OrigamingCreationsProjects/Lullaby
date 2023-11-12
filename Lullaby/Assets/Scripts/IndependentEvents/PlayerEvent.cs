using System;
using Lullaby.Entities;
using UnityEngine.Events;

namespace Lullaby
{
    [Serializable]
    public class PlayerEvent : UnityEvent<Player> { }
}