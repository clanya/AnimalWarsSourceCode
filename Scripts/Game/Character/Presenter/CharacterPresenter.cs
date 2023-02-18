using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data.DataTable;
using Game.BattleFlow;
using Game.BattleFlow.Character;
using Game.BattleFlow.Turn;
using Game.Character.Managers;
using Game.Character.Models;
using Game.Character.Skills;
using Game.Character.View;
using Game.Stages.Explorers;
using Game.Stages.Managers;
using Game.Teams.Edit;
using Game.Tiles;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Game.Character.Presenter
{
    public sealed class CharacterPresenter: MonoBehaviour
    {
        [Inject] private TurnManager turnManager;         //おためし
        [Inject] private ExplorerFactory explorerFactory;
        private MovablePointsExplorer movablePointsExplorer;
        private MoveRouteExplorer moveRouteExplorer;
        [Inject] private NavigationTileViewDirector navigationTileViewDirector;
        [Inject] private CharacterViewDirector viewDirector;
        [Inject] private CharacterModelDirector modelDirector;
        [Inject] private TargetCharacterExplorer targetCharacterExplorer;   //とりあえずここに置く
        [Inject] private SkillManager skillManager;
        [SerializeField] private CharacterCommandView characterCommandView;
        [SerializeField] private CharacterStatusView characterStatusView;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private SelectCharacterView selectPointObj;
        private SelectCharacterView selectObj;
        private CharacterStatusObservable statusObservable;
        private BaseCharacter SelectedCharacter => modelDirector.SelectedCharacter.Value;
        private CharacterModelView targetCharacter;
        private bool IsTargetTurn { get; set; }
        private bool canClickForMove = true;
        private List<CharacterModelView> characterTmpList { get; } = new();

        private CancellationToken token;

        private bool hadPushedMoveButton;
        [Inject] private StageManager stageManager;
        [SerializeField] private List<EditViewParameterDataTable> editViewParameterDataTableList;
        [SerializeField] private CharacterImageTable characterImageTable;
        
        private void Start()
        {
            movablePointsExplorer = explorerFactory.GetMovablePointsExplorer(PlayerID.Player1);
            characterStatusView.Initialize();
            moveRouteExplorer = explorerFactory.GetMoveRouteExplorer();

            statusObservable = new CharacterStatusObservable(characterStatusView);
            SelectedCharacterObservable();
            selectObj = Instantiate(selectPointObj);
            SceneManager.MoveGameObjectToScene(selectObj.gameObject,gameObject.scene);
            
            selectObj.SetActive(false);
            token = this.GetCancellationTokenOnDestroy();
        }

        public void SetExplorers(MovablePointsExplorer movablePointsExplorer)
        {
            this.movablePointsExplorer = movablePointsExplorer;
        }

        /// <summary>
        /// キャラクターインスタンス時、ViewとModelを結びつける。
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="view"></param>
        /// <param name="commandView"></param>
        public void OnCreateCharacter(BaseCharacter character, CharacterView view,CharacterCommandView commandView)
        {
            character.SetSkillManager(skillManager);
            var color = GetColor(character.PlayerID);
            view.SetSliderFillColor(color);
            characterTmpList.Add(new CharacterModelView(character,view,commandView));
            viewDirector.OnPointerClickedObservable(view);

            //Move処理, 移動できるキャラクターが移動できるタイルをクリックしたとき、移動する。
            navigationTileViewDirector.ClickedMovePointOservable
                .Where(_=> canClickForMove)                                         //キャラクターが移動しているときに、別のタイルをクリックした他のところに行かないように制限.
                .Where(_=> modelDirector.SelectedCharacter.Value is not null)
                .Where(_=>hadPushedMoveButton)                                      //Moveボタンが押されたか
                .Where(_=>modelDirector.SelectedCharacter.Value.IsMovable.Value)    //選択されたキャラクターは移動可能か
                .Where(_ => character == modelDirector.SelectedCharacter.Value)     //常にステージにいる全てのキャラクターが購読しているので、選択キャラクターのみSubscribeできる。
                .Subscribe(async point =>
                {
                    cameraController.StartFollow(character.transform, token);
                    var moveStack = new Stack<Vector2Int>();
                    moveRouteExplorer.FindRoute(character.Position, point, character.Param.Type, character.Param.MovableRange, ref moveStack);
                    selectObj.SetActive(false);
                    canClickForMove = false;
                    commandView.ShowView(false);
                    await character.MoveAsync(moveStack, token);                    //移動処理
                    commandView.ShowView(true);
                    canClickForMove = true;
                    selectObj.SetActive(true);
                    selectObj.SetPositionXZ(character.Position.x, character.Position.y);
                    character.SetIsMovable(false);

                    cameraController.StopFollow();
                    hadPushedMoveButton = false;
                }).AddTo(this);
            
            character.CurrentStatus.Hp
                .Subscribe(value =>
                {
                    view.SetHp(value,character.Param.Hp);
                    if (value <= 0)
                    {
                        character.SetIsDead(true);
                        turnManager.CheckResult();
                        return;
                    }
                    character.SetIsDead(false);
                }).AddTo(this);

            //キャラクターが行動可能・不可能の状態が変わった時発行。
            character.IsActionable
                .Skip(1)    //Note: turnManagerがインスタンス化する前に購読することになるので一回スキップ。
                .Subscribe(isActionable =>
                {
                    if (isActionable)
                    {
                        character.SetState(CharacterViewState.Normal);
                        return;
                    }
                    character.SetState(CharacterViewState.HadActed);
                    characterStatusView.ShowView(false);
                    commandView.ShowView(false);
                    selectObj.SetActive(false);
                    turnManager.CheckSwapTurn();

                }).AddTo(this);

            //自分のターンかつ行動できるときか否かによって、キャラクターをクリックしたときに表示させるUIを変える。
            viewDirector.ClickedCharacterObservable
                .Where(x => x == view)  //クリックされたViewしか発行させない
                .Where(_ => IsTargetTurn == false)
                .Subscribe(x =>
                {
                    //とりあえず。なにかしらのオペレータがあったはず。次のIObservableにメッセージを渡すみたいな。
                    BaseCharacter tmpModel;
                    try
                    {
                        tmpModel = characterTmpList.Single(y => y.View == x).Model;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw;
                    }
                    //自分のターンかつ行動できるとき
                    var flag =  turnManager.CurrentTurnPlayer.characterList.Contains(tmpModel) && tmpModel.IsActionable.Value;
                    commandView.ShowView(flag);
                    characterStatusView.ShowView(true);
                    statusObservable.Dispose();         //クリックするたびに、Observableを作るので、Disposeで購読を中止する。
                    statusObservable.Observable(tmpModel,editViewParameterDataTableList[stageManager.selectedStageNumber],characterImageTable);
                }).AddTo(this);
        }

        /// <summary>
        /// コマンドViewのObservable.
        /// </summary>
        private void SelectedCharacterObservable()
        {
            characterCommandView.MoveButtonObservable
                .Subscribe( _ =>
                {
                    characterCommandView.SetMoveButtonInteractable(false);
                    modelDirector.SetIsLockedSelectedCharacter(true);
                    characterCommandView.SetEnableEscapeButtonInteractable(false);
                    hadPushedMoveButton = true;
                }).AddTo(this);

            characterCommandView.AttackButtonObservable
                .Subscribe(async _ =>
                {
                    IsTargetTurn = true;
                    characterCommandView.SetEnableEscapeButtonInteractable(false);
                    characterCommandView.SetAllActionButtonInteractable(false);
                    
                    var attackTargetPlayerID = SelectedCharacter.PlayerID switch
                    {
                        PlayerID.Player1 => PlayerID.Player2,
                        PlayerID.Player2 => PlayerID.Player1,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    //Target選択する待ち処理
                    modelDirector.SetTargetedCharacter(null);
                    var target = await WaitingForGetTargetCharacter(SelectedCharacter.Param.AttackRange,attackTargetPlayerID);

                    await FaceToFace(SelectedCharacter, target, token);

                    SelectedCharacter.AttackAsync(target).Forget();
                    SelectedCharacter.SetIsMovable(false);
                    IsTargetTurn = false;
                }).AddTo(this);

            characterCommandView.StandbyButtonObservable
                .Subscribe(_ =>
                {
                    SelectedCharacter.Standby();
                    characterCommandView.SetEnableEscapeButtonInteractable(false);
                    characterCommandView.SetAllActionButtonInteractable(false);
                }).AddTo(this);

            characterCommandView.UseSkillButtonObservable
                .Subscribe(async _ =>
                {
                    characterCommandView.SetEnableEscapeButtonInteractable(false);
                    characterCommandView.SetAllActionButtonInteractable(false);
                    if (skillManager.IsTargetSelectionRequired(SelectedCharacter.Param.SkillSetting.TargetType))
                    {
                        IsTargetTurn = true;
                        var skillSetting = SelectedCharacter.Param.SkillSetting;
                        modelDirector.SetTargetedCharacter(null);
                        var target = await WaitingForGetTargetCharacter(skillSetting.TargetRange,skillManager.TargetPlayerID(SelectedCharacter)); 
                        SelectedCharacter.UseSkillAsync(target).Forget();
                        IsTargetTurn = false;
                    }
                    else
                    {
                        SelectedCharacter.UseSkillAsync().Forget();
                    }
                    // SelectedCharacter.SetIsActionable(false);
                    SelectedCharacter.SetIsMovable(false);
                }).AddTo(this);

            characterCommandView.EscapeButtonObservable
                .Subscribe(_ =>
                {
                    modelDirector.ClearSelectedCharacter();
                }).AddTo(this);
            
            modelDirector.SelectedCharacter
                .Subscribe(character =>
                {
                    if (character is null)
                    {
                        return;
                    }
                    characterCommandView.SetEnableEscapeButtonInteractable(true);
                    
                    character.IsMovable
                        .Subscribe(value =>
                        {
                            var movablePointExists = movablePointsExplorer.MovablePointExists(character.Position,
                                character.CurrentStatus.MovableRange.Value,character.Param.Type);

                            bool canPushMoveButton = value && movablePointExists;
                            characterCommandView.SetMoveButtonInteractable(canPushMoveButton);

                            var attackTargetPlayerID = character.PlayerID switch
                            {
                                PlayerID.Player1 => PlayerID.Player2,
                                PlayerID.Player2 => PlayerID.Player1,
                                _ => throw new ArgumentOutOfRangeException()
                            };
                            var flag = targetCharacterExplorer.TargetCharacterExists(character.Position,character.Param.AttackRange,attackTargetPlayerID);
                            characterCommandView.SetAttackButtonInteractable(flag && character.IsActionable.Value);
                            //UseSkillの適用範囲はAttackと異なる。
                            characterCommandView.SetUseSkillButtonInteractable(skillManager.CanUseSkill(character) && character.IsActionable.Value);

                        }).AddTo(this);

                    character.IsActionable
                        .Subscribe(value =>
                        {
                            characterCommandView.SetStandbyButtonInteractable(value);
                            var attackTargetPlayerID = character.PlayerID switch
                            {
                                PlayerID.Player1 => PlayerID.Player2,
                                PlayerID.Player2 => PlayerID.Player1,
                                _ => throw new ArgumentOutOfRangeException()
                            };
                            var isAttackTargetExists = targetCharacterExplorer.TargetCharacterExists(character.Position,character.Param.AttackRange,attackTargetPlayerID);
                            characterCommandView.SetAttackButtonInteractable(isAttackTargetExists && value);
                            characterCommandView.SetUseSkillButtonInteractable(skillManager.CanUseSkill(character) && value);
                            if (value == false)
                            {
                                modelDirector.SetIsLockedSelectedCharacter(false);
                            }
                        }).AddTo(this);
                }).AddTo(this);
        }

        private async UniTask<BaseCharacter> WaitingForGetTargetCharacter(int range,PlayerID targetPlayerID)
        {
            var selfPosition = modelDirector.SelectedCharacter.Value.Position;
            modelDirector.ClearTargetCharacter();
            await UniTask.WaitUntil(() => targetCharacterExplorer.FindTargetCharacters(selfPosition,range,targetPlayerID)
                .Contains(modelDirector.TargetedCharacter.Value), cancellationToken:token);
            return modelDirector.TargetedCharacter.Value;
        }
        public void OnClickedObservable()
        {
            viewDirector.ClickedCharacterObservable
                .Where(_ => IsTargetTurn == false && modelDirector.IsLockedSelectedCharacter == false)
                .Subscribe(view =>
                {
                    var selectedModel = characterTmpList.Single(x => x.View == view).Model;
                    modelDirector.SetSelectedCharacter(selectedModel);
                }).AddTo(this);

            modelDirector.SelectedCharacter
                .Zip(modelDirector.SelectedCharacter.Skip(1), (s, t) => new {oldValue = s, newValue = t})
                .Subscribe(v =>
                {
                    if (v.oldValue is null)
                    {
                        //新しく選ばれたcharacterを選択状態にする
                        var characterModel = v.newValue;
                        var characterView = characterTmpList.Single(x => x.Model == characterModel).View;
                        selectObj.SetActive(true);
                        var pos = characterView.transform.position;
                        selectObj.SetPositionXZ(pos.x,pos.z);
                        cameraController.Forcus(characterView.transform.position, token).Forget();
                    }
                    else if (v.newValue is null)
                    {
                        //前に選んでいたcharacterを戻す
                        var characterModel = v.oldValue;
                        var commandView = characterTmpList.Single(x => x.Model == characterModel).CommandView;
                        selectObj.SetActive(false);
                        commandView.ShowView(false);
                        characterStatusView.ShowView(false);
                    }
                    else
                    {
                        if (v.oldValue != v.newValue)
                        {
                            //新しく選ばれたcharacterを選択状態にする
                            var newModel = v.newValue;
                            var newView = characterTmpList.Single(x => x.Model == newModel).View;
                            selectObj.SetActive(true);
                            var pos = newView.transform.position;
                            selectObj.SetPositionXZ(pos.x,pos.z);
                            cameraController.Forcus(newView.transform.position, token).Forget();
                        }
                    }
                }).AddTo(this);
        }
        
        /// <summary>
        /// Attack時などのTargetを選択するとき
        /// </summary>
        public void TargetCharacterClickedObservable()
        {
            viewDirector.ClickedCharacterObservable
                .Where(_ => IsTargetTurn)   //Attack時など、IsTargetTurn状態がtrueのとき発行
                .Subscribe(view =>
                {
                    Debug.Log($"SelectedCharacterPoint:{view}");
                    var targeted = characterTmpList.Single(x => x.View == view).Model;
                    modelDirector.SetTargetedCharacter(targeted);
                }).AddTo(this);
        }

        private Color GetColor(PlayerID id)
        {
            return id switch
            {
                PlayerID.Player1 => Color.green,
                PlayerID.Player2 => Color.red,
                _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
            };
        }
        //modelとViewの結びつきを知るために使う。
        private class CharacterModelView
        {
            public BaseCharacter Model { get; }
            public CharacterView View { get; }
            
            public CharacterCommandView CommandView { get; }

            public CharacterModelView(BaseCharacter model, CharacterView view,CharacterCommandView commandView)
            {
                this.Model = model;
                this.View = view;
                this.CommandView = commandView;
            }
        }

        private async UniTask FaceToFace(BaseCharacter character1, BaseCharacter character2, CancellationToken token)
        {
            Vector2Int lookVec1 = (character2.Position - character1.Position);
            if (lookVec1.x == 0)
            {
                lookVec1.y = (int)Mathf.Sign(lookVec1.y);
            }
            else
            {
                lookVec1.x = (int)Mathf.Sign(lookVec1.x);
            }

            Vector2Int lookVec2 = (character1.Position - character2.Position);
            if (lookVec2.x == 0)
            {
                lookVec2.y = (int)Mathf.Sign(lookVec2.y);
            }
            else
            {
                lookVec2.x = (int)Mathf.Sign(lookVec2.x);
            }

            var forcusPoint = new Vector3((character1.Position.x + character2.Position.x) / 2, 0, (character1.Position.y + character2.Position.y) / 2);
            await cameraController.Forcus(forcusPoint, token);
            await character1.TurnToLookAt(lookVec1, token);
            await character2.TurnToLookAt(lookVec2, token);
        }

        private void OnDestroy()
        {
            modelDirector.Dispose();
            viewDirector.Dispose();
        }
    }
}