using System.Collections.Generic;
using PJTC.Enums;

namespace PJTC.General
{
    public static class AttackMap
    {
        //key victim, value hunter
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

        //key hunter, value victim
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
