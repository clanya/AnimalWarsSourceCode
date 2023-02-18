using Game.BattleFlow;
using Game.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Teams.Edit
{
    public enum RoleType
    {
        Attacker,
        SubAttacker,
        AllRounder,
        Tank,
        Healer,
    }

    [CreateAssetMenu(menuName ="MyScriptable/EditViewParameterDataTable")]
    public class EditViewParameterDataTable : ScriptableObject
    {
        [Serializable]
        public class ViewData
        {
            [SerializeField] private CharacterType characterType;
            [SerializeField] private string name;
            [SerializeField, Multiline] private string introduce;
            [SerializeField] private RoleType role;
            [SerializeField, Multiline] private string skillExplain;

            public CharacterType CharacterType => characterType;
            public string Name => name;
            public string Introduce => introduce;
            public RoleType Role => role;
            public string SkillExplain => skillExplain;
        }

        [SerializeField] private List<ViewData> viewDataList;

        public ViewData GetViewData(CharacterType characterType)
            => viewDataList.First(x => x.CharacterType == characterType);
    }
}

