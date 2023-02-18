using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Game.BattleFlow
{
    //キャラクター選択時に表示されるあかいオブジェクト
    public sealed class SelectCharacterView : MonoBehaviour
    {
        [Header("振幅"),SerializeField] private float amplitude;
        [SerializeField] private float initPositionY;
        [SerializeField] private float speed = 5f;
        private void Start()
        {
            transform.position = new Vector3(transform.position.x, initPositionY, transform.position.z);
            
            var direction = speed;
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (transform.position.y >= initPositionY + amplitude)
                    {
                        direction = -speed;
                    }
                    if (transform.position.y <= initPositionY - amplitude)
                    {
                        direction = speed;
                    }
                    transform.position = new Vector3(transform.position.x, transform.position.y + direction* Time.deltaTime,
                        transform.position.z);
                }).AddTo(this);
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetPositionXZ(float posX,float posZ)
        {
            transform.position = new Vector3(posX, transform.position.y, posZ);
            
        }
    }
}