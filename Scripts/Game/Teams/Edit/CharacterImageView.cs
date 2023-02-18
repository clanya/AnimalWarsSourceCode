using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using Game.Character;
using Data.DataTable;
using Game.Character.Images;
using TMPro;
using UnityEngine.UI;
using VContainer;
using System;
using Game.Audio;

namespace Game.Teams.Edit
{
    public class CharacterImageView : MonoBehaviour
    {
        [SerializeField] private CharacterImage[] characterImageArray;
        [SerializeField] private TeamCharacterImage[] teamCharacterImageArray;
        [SerializeField] private CharacterParameterView characterParameterView;
        [SerializeField] private CharacterImageTable characterImageTable;
        [SerializeField] private Image draggingCharacterImage;
        [SerializeField] private Sprite dashedBoxSprite;

        private TMP_Text[] teamNumberTextArray;

        private ISubject<KeyValuePair<int, CharacterType>> attachedSubject = new Subject<KeyValuePair<int, CharacterType>>();
        public IObservable<KeyValuePair<int, CharacterType>> AttachedObservable => attachedSubject.AsObservable();

        IEnumerable<CharacterType> cantSelectCharacters = new List<CharacterType>();

        private void Start()
        {
            ViewCharacterList();

            CharacterImageObservables();
            TeamCharacterImageObservables();
        }

        private void ViewCharacterList()
        {
            foreach (var image in characterImageArray)
            {
                Sprite sprite = characterImageTable.GetCharacterSprite(image.CharacterType);
                image.SetCharacterImage(sprite);
            }
        }

        public void ViewTeamImage(int teamMenberCount)
        {
            teamNumberTextArray = teamCharacterImageArray.Take(teamMenberCount).Select(x => x.transform.GetChild(0).GetComponent<TMP_Text>()).ToArray();

            for(int i = 0; i < teamMenberCount; i++)
            {
                teamCharacterImageArray[i].gameObject.SetActive(true);
                teamNumberTextArray[i].text = (i + 1).ToString();
            }
        }

        public void SetSelectableCharacterImage(IEnumerable<CharacterType> characterTypes)
        {
            cantSelectCharacters = characterTypes;

            foreach(var image in characterImageArray)
            {
                image.SetSrelrctable(!characterTypes.Any(x => x == image.CharacterType));
            }
        }

        //キャラクターリストの画像が操作されたときの処理
        private void CharacterImageObservables()
        {
            foreach (var image in characterImageArray)
            {
                image.OnPointerPositionObservable
                    .Where(_ => !image.IsDragging)
                    .Subscribe(pos =>
                    {
                        characterParameterView.ViewParameter(image.CharacterType, pos);
                    })
                    .AddTo(this);

                image.ExitPointerObservable
                    .Subscribe(_ =>
                    {
                        characterParameterView.HideParameter();
                    })
                    .AddTo(this);

                image.StartDragObservable
                    .Where(_=>image.IsSelectable)
                    .Subscribe(_ =>
                    {
                        characterParameterView.HideParameter();
                        draggingCharacterImage.gameObject.SetActive(true);
                        draggingCharacterImage.sprite = characterImageTable.GetCharacterSprite(image.CharacterType);
                        AudioPlayer.PlayPopSE();
                    })
                    .AddTo(this);

                image.DragPointerPositionObservable
                    .Where(_ => image.IsSelectable)
                    .Subscribe(x =>
                    {
                        draggingCharacterImage.transform.position = x;
                    })
                    .AddTo(this);

                image.EndDragObservable
                    .Subscribe(_ =>
                    {
                        draggingCharacterImage.gameObject.SetActive(false);
                    })
                    .AddTo(this);
            }
        }

        //パーティのイメージが操作されたときの処理
        private void TeamCharacterImageObservables()
        {
            int length = teamCharacterImageArray.Length;
            for (int i = 0; i < length; i++)
            {
                int index = i;
                teamCharacterImageArray[index].AttachedCharacter
                    .Skip(1)
                    .Where(x=>!cantSelectCharacters.Any(y=>x==y))
                    .Subscribe(x =>
                    {
                        if (x == CharacterType.None)
                        {
                            teamCharacterImageArray[index].SetCharacterImage(dashedBoxSprite);
                            teamCharacterImageArray[index].HideFrame();
                            teamNumberTextArray[index].gameObject.SetActive(true);
                            AudioPlayer.PlayPiSE();
                        }
                        else
                        {
                            Sprite sprite = characterImageTable.GetCharacterSprite(x);
                            teamCharacterImageArray[index].SetCharacterImage(sprite);
                            teamCharacterImageArray[index].ViewFrame();
                            teamNumberTextArray[index].gameObject.SetActive(false);
                            AudioPlayer.PlaySetSE();
                        }

                        attachedSubject.OnNext(new KeyValuePair<int, CharacterType>(index, x));
                    })
                    .AddTo(this);
            }
        }
    }
}

