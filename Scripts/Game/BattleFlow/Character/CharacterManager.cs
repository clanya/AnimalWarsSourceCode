using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Character.Managers;
using Game.Character.Models;
using Game.Character.Presenter;
using Game.Stages.Managers;
using Game.Teams.Managers;
using UnityEngine;
using VContainer;

namespace Game.BattleFlow.Character
{
    public sealed class CharacterManager : MonoBehaviour
    {
        private List<BaseCharacter> playerCharacterList = new();

        public IReadOnlyList<BaseCharacter> PlayerCharacterList =>
            playerCharacterList.Where(x=>!x.IsDead.Value).ToList(); //BattleScene上で他クラスがキャラクターの情報を知りたいとき参照

        private List<BaseCharacter> enemyCharacterList = new();

        public IReadOnlyList<BaseCharacter> EnemyCharacterList =>
            enemyCharacterList.Where(x=>!x.IsDead.Value).ToList(); //BattleScene上で他クラスがキャラクターの情報を知りたいとき参照
        public IEnumerable<BaseCharacter> AllCharacters => playerCharacterList.Union(enemyCharacterList).Where(x=>!x.IsDead.Value);
        
        [SerializeField] private CharacterGenerator characterGenerator;
        [Inject] private TeamManager teamManager;
        [Inject] private StageManager stageManager;
        [Inject] private TargetCharacterExplorer targetCharacterExplorer;
        [SerializeField] private CharacterDispatcher characterDispatcher;

        [SerializeField] private CharacterPresenter presenter;
        private void Awake()
        {
            InitializeCharacters();
            var characterList = playerCharacterList.Union(enemyCharacterList).ToList();
            targetCharacterExplorer.InitializeCharacterList(characterList);

            characterDispatcher.SetCharacterList(PlayerCharacterList);
            characterDispatcher.SetCharacterList(EnemyCharacterList);
            presenter.OnClickedObservable();
            
            presenter.TargetCharacterClickedObservable();
        }

        private void InitializeCharacters()
        {
            //Playerのキャラクターを生成
            var playerCharacterTypeList = teamManager.teamCharacterTypes;
            var friendCharacterPositionDataArray = stageManager.characterInitialPositionData.FriendCharacterPositionDataArray;
            var playerInitialPositionList = teamManager.teamMemberNumbers.Select(num => friendCharacterPositionDataArray[num].InitialPosition).ToList();

            var playerTypeAndPositionDic = playerCharacterTypeList
                .Zip(playerInitialPositionList, (k, v) => new { Key = k, Value = v })
                .ToDictionary(t => t.Key, t => t.Value);
            
            playerCharacterList = characterGenerator.GenerateCharacters(playerTypeAndPositionDic,PlayerID.Player1);

            //Enemyのキャラクターを生成
            var enemyCharacterPositionDataArray = stageManager.characterInitialPositionData.EnemyCharacterPositionDataArray;
            var enemyCharacterTypes = enemyCharacterPositionDataArray.Select(x => x.CharacterType);
            var enemyInitialPositions = enemyCharacterPositionDataArray.Select(x => x.InitialPosition);

            var enemyTypeAndPositionList = enemyCharacterTypes
                .Zip(enemyInitialPositions, (k, v) => new {Key = k, Value = v})
                .Select(x => new KeyValuePair<CharacterType,Vector2Int>(x.Key,x.Value)).ToList();
            
            enemyCharacterList = characterGenerator.GenerateCharacters(enemyTypeAndPositionList,PlayerID.Player2);
        }
    }
}