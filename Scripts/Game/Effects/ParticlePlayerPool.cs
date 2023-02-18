using UniRx.Toolkit;

namespace Game.Effects
{
    public class ParticlePlayerPool : ObjectPool<ParticlePlayer>
    {
        private readonly ParticlePlayer _original;

        public ParticlePlayerPool(ParticlePlayer original)
        {
            //オリジナルは非表示に
            _original = original;
            _original.gameObject.SetActive(false);
        }

        //インスタンスを作る処理
        protected override ParticlePlayer CreateInstance()
        {
            //オリジナルを複製してインスタンス作成(オリジナルと同じ親の下に配置)
            return ParticlePlayer.Instantiate(_original, _original.transform.parent);
        }
    }
}

