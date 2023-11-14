using System;

namespace DefaultNamespace
{
    [Flags]
    public enum EAttackType
    {
        Block = 0,
        Jab = 1,
        JabHit = 1 << 1,
        Kick = 1 << 2,
        KickHit = 1 << 3,
    }
}