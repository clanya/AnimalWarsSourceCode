using System.Collections.Generic;
using System.Linq;
using Game.BattleFlow;
using Game.Character.Models;
using UnityEngine;
using VContainer;

namespace Game.Character.Managers
{
    public class TargetCharacterExplorer
    {
        private IReadOnlyList<BaseCharacter> characterList;
        
        public void InitializeCharacterList(IReadOnlyList<BaseCharacter> characterList)
        {
            this.characterList = characterList;
        }
        
        /// <summary>
        /// selfPositionからrange内にいるtargetPlayerIDのキャラクターたちを返す。
        /// </summary>
        public IEnumerable<BaseCharacter> FindTargetCharacters(Vector2Int? selfPosition,int range,PlayerID targetPlayerID)
        {
            var position = selfPosition.Value;
            
            if (range == 2)
            {
                var target2Characters = characterList
                    .Where(character => character.IsDead.Value == false)
                    .Where(character => character.PlayerID == targetPlayerID)
                    .Where(character => character.Position == new Vector2Int(position.x + 2, position.y) ||         //右２
                                        character.Position == new Vector2Int(position.x - 2, position.y) ||         //左２
                                        character.Position == new Vector2Int(position.x, position.y + 2) ||         //上２
                                        character.Position == new Vector2Int(position.x, position.y - 2) ||         //下２
                                        character.Position == new Vector2Int(position.x + 1,position.y + 1) ||     //右上
                                        character.Position == new Vector2Int(position.x + 1,position.y - 1) ||     //右下
                                        character.Position == new Vector2Int(position.x - 1,position.y + 1) ||     //左上
                                        character.Position == new Vector2Int(position.x - 1,position.y - 1)         //左下
                    );
                return target2Characters;
            }
            var targetCharacters = characterList
                    .Where(character => character.IsDead.Value == false)
                    .Where(character => character.PlayerID == targetPlayerID)
                    .Where(character => character.Position == new Vector2Int(position.x + range, position.y) ||
                                        character.Position == new Vector2Int(position.x - range, position.y) ||
                                        character.Position == new Vector2Int(position.x, position.y + range) ||
                                        character.Position == new Vector2Int(position.x, position.y - range));
            return targetCharacters;
        }
        
        /// <summary>
        /// selfPositionからrange内にいるtargetPlayerIDのキャラクターがいた場合true.そうでなければfalse.
        /// </summary>
        public bool TargetCharacterExists(Vector2Int selfPosition,int range,PlayerID targetPlayerID)
        {
            var targetOpponents = FindTargetCharacters(selfPosition,range,targetPlayerID);
            if (targetOpponents.Any())
            {
                return true;
            }

            return false;
        }
    }
}

