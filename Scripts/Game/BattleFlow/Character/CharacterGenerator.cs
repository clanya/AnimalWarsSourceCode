using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Character.Models;
using Game.Stages.Managers;
using UnityEngine;
using VContainer;

namespace Game.BattleFlow.Character
{
    public sealed class CharacterGenerator : MonoBehaviour
    { 
        //indexで対応付け。
        [Inject] private StageManager stageManager;
        [SerializeField] private List<StageCharacterData> stageCharacterDataList;
        
        private const float InitialPositionY = 0;

        private BaseCharacter GenerateCharacter(CharacterType type,Vector2Int position, PlayerID id)
        {
            int index = stageManager.selectedStageNumber;
            if (type == CharacterType.None)
            {
                Debug.LogError("キャラクタータイプがセットされていない");
                return null;
            }

            Quaternion generateRotation;
            if (id == PlayerID.Player1)
            {
                generateRotation = Quaternion.identity;
            }
            else
            {
                generateRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }

            var character = Instantiate(stageCharacterDataList[index].CharacterList.Single(x => x.Param.Type == type).Prefab, new Vector3(position.x,InitialPositionY,position.y), generateRotation);
            character.SetPlayerID(id);
            //ここでSetterでsetするのでは無くもっと良いやり方ありそう
            character.SetParam(stageCharacterDataList[index].CharacterList.Single(x => x.Param.Type == type).Param); 
            character.Initialize();
            character.InitializeCurrentPosition(position);
            return character;
        }
        
        public List<BaseCharacter> GenerateCharacters(IEnumerable<KeyValuePair<CharacterType, Vector2Int>> list,PlayerID id)
        {
            var generatedCharacterList = list.Select(x => GenerateCharacter(x.Key, x.Value,id)).ToList();
            return generatedCharacterList;
        }
    }
}