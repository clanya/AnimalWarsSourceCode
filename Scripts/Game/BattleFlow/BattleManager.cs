using System.Collections.Generic;
using System.Linq;
using Game.BattleFlow.Character;
using Game.BattleFlow.Turn;
using Game.Character;
using Game.Character.Models;
using Game.PoseScene;
using UniRx;
using UnityEngine;
using VContainer;

namespace Game.BattleFlow
{
    public sealed class BattleManager : MonoBehaviour
    {
        private readonly List<TurnManager.Player> playerList = new();
        private IReadOnlyList<BaseCharacter> player1CharacterList;
        private IReadOnlyList<BaseCharacter> player2CharacterList;
        [SerializeField] private CharacterManager characterManager;
        
        //public TurnManager TurnManager { get; private set; }

        [Inject] private TurnManager turnManager; 

        //Todo: 選択中のキャラクター情報保持

        private void Start()
        {
            Initialize();
            turnManager.Result
                .Skip(1)
                .Take(1)
                .Subscribe(x =>
                {
                    //結果が出た時の各UIを登録
                    switch (x.playerID)
                    {
                        case PlayerID.Player1:
                            PoseScenePanelViewer.OpenResultPanel("クリア!");
                            break;
                        case PlayerID.Player2:
                            PoseScenePanelViewer.OpenResultPanel("ゲームオーバー");
                            break;
                    }
                }).AddTo(this);
        }
        private void Initialize()
        {
            player1CharacterList = characterManager.PlayerCharacterList;
            player2CharacterList = characterManager.EnemyCharacterList;
            playerList.Add(new TurnManager.Player(player1CharacterList,PlayerID.Player1));
            playerList.Add(new TurnManager.Player(player2CharacterList,PlayerID.Player2));
            turnManager.Init(playerList);
        }

        private void OnDestroy()
        {
            turnManager.OnDispose();
        }
    }
}
