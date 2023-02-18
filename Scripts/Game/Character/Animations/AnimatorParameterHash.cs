using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character.Animations
{
    public enum AnimatorParameterType
    {
        Move,
        Attack,
        Damaged,
        Die,
    }

    public static class AnimatorParameterHash
    {
        private static Dictionary<AnimatorParameterType, int> parameterHashDic = new Dictionary<AnimatorParameterType, int>()
        {
            {AnimatorParameterType.Move, Animator.StringToHash("Move") },
            {AnimatorParameterType.Attack, Animator.StringToHash("Attack") },
            {AnimatorParameterType.Damaged, Animator.StringToHash("Damaged") },
            {AnimatorParameterType.Die, Animator.StringToHash("Die") },
        };

        public static int GetParameterHash(AnimatorParameterType tagType)
           => parameterHashDic[tagType];
    }

}
