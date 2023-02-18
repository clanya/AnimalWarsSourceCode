using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Character.Skills
{
    [Serializable]
    public class SkillSetting
    {
        [HideInInspector] public Skill Skill;   //Editor拡張用
        [SerializeField] private AttackType attackSkillType;
        [SerializeField] private List<StatusType> buffForStatusTypeList;
        
        [SerializeField] private int amount;
        
        [SerializeField] private TargetType targetType;
        [SerializeField] private int targetRange;
        
        [SerializeField] private uint applicableTurn;
        [SerializeField] private int cost;

        public AttackType AttackSkillType => attackSkillType;
        public IReadOnlyList<StatusType> BuffForStatusTypeList => buffForStatusTypeList;
        public int Amount => amount;
        public TargetType TargetType => targetType;
        public int TargetRange => targetRange;
        public uint ApplicableTurn => applicableTurn;
        public int Cost => cost;
    }
}