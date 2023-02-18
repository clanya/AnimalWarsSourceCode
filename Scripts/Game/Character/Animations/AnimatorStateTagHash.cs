using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Animations
{
    public enum AnimatorStateTagType
    {
        Empty,
        Move,
        Attack,
        Damaged,
        Die
    }

    public static class AnimatorStateTagHash
    {
        private static Dictionary<AnimatorStateTagType, int> tagHashDic = new Dictionary<AnimatorStateTagType, int>()
        {
            {AnimatorStateTagType.Empty, Animator.StringToHash("Empty") },
            {AnimatorStateTagType.Move, Animator.StringToHash("Move") },
            {AnimatorStateTagType.Attack, Animator.StringToHash("Attack") },
            {AnimatorStateTagType.Damaged, Animator.StringToHash("Damaged") },
            {AnimatorStateTagType.Die, Animator.StringToHash("Die") },
        };

        public static int GetTagHash(AnimatorStateTagType tagType)
            => tagHashDic[tagType];
    }
}

