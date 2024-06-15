using System;
using System.Collections.Generic;
using PJTC.Enums;
using UnityEngine;

namespace PJTC.General
{
    public static class AttackMap
    {
        //key who defeat, value who can beat
        public static Dictionary<CatsType.Attack, CatsType.Attack> attackMap = new Dictionary<
            CatsType.Attack,
            CatsType.Attack
        >()
        {
            { CatsType.Attack.None, CatsType.Attack.None },
            { CatsType.Attack.Paws, CatsType.Attack.Tail },
            { CatsType.Attack.Tail, CatsType.Attack.Jaws },
            { CatsType.Attack.Jaws, CatsType.Attack.Paws },
        };

        //key who attack, value who can be defeated
        public static Dictionary<CatsType.Attack, CatsType.Attack> defenceMap = new Dictionary<
            CatsType.Attack,
            CatsType.Attack
        >()
        {
            { CatsType.Attack.None, CatsType.Attack.None },
            { CatsType.Attack.Tail, CatsType.Attack.Paws },
            { CatsType.Attack.Jaws, CatsType.Attack.Tail },
            { CatsType.Attack.Paws, CatsType.Attack.Jaws },
        };
    }
}
