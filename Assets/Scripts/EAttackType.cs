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
        Hook = 1 << 4,
        HookHit = 1 << 5,
        LowKick = 1 << 6,
        LowKickHit = 1 << 7,
        FireBall = 1 << 8,
        FireBallHit = 1 << 9,
    }
}