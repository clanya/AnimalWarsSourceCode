using System;
using UniRx;

namespace Game.Character.Models
{
    /// <summary>
    /// インスタンス先でOnDestroy()を呼ぶこと
    /// </summary>
    public sealed class CharacterCurrentStatus : IDisposable
    {
        private readonly IntReactiveProperty hp;
        public IReadOnlyReactiveProperty<int> Hp => hp;

        private readonly IntReactiveProperty sp;
        public IReadOnlyReactiveProperty<int> Sp => sp;

        private readonly IntReactiveProperty attackPower;
        public IReadOnlyReactiveProperty<int> AttackPower => attackPower;
        
        private readonly IntReactiveProperty defensePower;
        public IReadOnlyReactiveProperty<int> DefensePower => defensePower;

        private readonly IntReactiveProperty magicDefensePower;
        public IReadOnlyReactiveProperty<int> MagicDefensePower => magicDefensePower;

        private readonly IntReactiveProperty speed;
        public IReadOnlyReactiveProperty<int> Speed => speed;

        private readonly ReactiveProperty<AttackType> attackType;
        public IReadOnlyReactiveProperty<AttackType> AttackType => attackType;
        
        private readonly IntReactiveProperty movableRange;
        public IReadOnlyReactiveProperty<int> MovableRange => movableRange;

        public float EvadeRate { get; private set; } = 0;

        public void SetEvadeRate(float value)
        {
            EvadeRate = value;
        }
        
        public CharacterCurrentStatus(int hp,int sp,int attackPower,int defensePower,int magicDefensePower,int speed,AttackType attackType,int movableRange)
        {
            this.hp = new IntReactiveProperty(hp);
            this.sp = new IntReactiveProperty(sp);
            this.attackPower = new IntReactiveProperty(attackPower);
            this.defensePower = new IntReactiveProperty(defensePower);
            this.magicDefensePower = new IntReactiveProperty(magicDefensePower);
            this.speed = new IntReactiveProperty(speed);
            this.attackType = new ReactiveProperty<AttackType>(attackType);
            this.movableRange = new IntReactiveProperty(movableRange);
        }
        
        public CharacterCurrentStatus(CharacterParam param)
            : this(param.Hp, param.Sp,param.AttackPower, param.DefensePower,
                param.MagicDefensePower, param.Speed, param.AttackType, param.MovableRange) { }

        public void SetHp(int hp)
        {
            this.hp.Value = hp;
        }

        public void AddHp(int value)
        {
            SetHp(Hp.Value + value);
        }
        
        public void DecreaseHp(int value)
        {
            SetHp(Hp.Value - value);
        }

        public void SetSp(int sp)
        {
            this.sp.Value = sp;
        }

        public void AddSp(int value)
        {
            SetSp(sp.Value + value);
        }
        
        public void DecreaseSp(int value)
        {
            SetSp(Sp.Value - value);
        }

        public void SetAttackPower(int value)
        {
            this.attackPower.Value = value;
        }

        public void AddAttackPower(int value)
        {
            SetAttackPower(AttackPower.Value + value);
        }
        
        public void DecreaseAttackPower(int value)
        {
            SetAttackPower(AttackPower.Value - value);
        }
        
        public void SetDefensePower(int value)
        {
            this.defensePower.Value = value;
        }

        public void AddDefensePower(int value)
        {
            SetDefensePower(DefensePower.Value + value);
        }
        
        public void DecreaseDefensePower(int value)
        {
            SetDefensePower(DefensePower.Value - value);
        }

        public void SetMagicDefensePower(int value)
        {
            this.magicDefensePower.Value = value;
        }

        public void AddMagicDefensePower(int value)
        {
            SetMagicDefensePower(MagicDefensePower.Value + value);
        }
        
        public void DecreaseMagicDefensePower(int value)
        {
            SetMagicDefensePower(MagicDefensePower.Value - value);
        }

        public void SetSpeed(int value)
        {
            this.speed.Value = value;
        }

        public void AddSpeed(int value)
        {
            SetSpeed(Speed.Value + value);
        }
        
        public void DecreaseSpeed(int value)
        {
            SetSpeed(Speed.Value - value);
        }
        
        public void SetMovableRange(int value)
        {
            this.movableRange.Value = value;
        }

        public void AddMovableRange(int value)
        {
            SetSpeed(MovableRange.Value + value);
        }
        
        public void DecreaseMovableRange(int value)
        {
            SetSpeed(MovableRange.Value - value);
        }

        public void SetAttackType(AttackType type)
        {
            attackType.Value = type;
        }
        
        public void Dispose()
        {
            hp.Dispose();
            sp.Dispose();
            attackPower.Dispose();
            defensePower.Dispose();
            magicDefensePower.Dispose();
            speed.Dispose();
            attackType.Dispose();
            movableRange.Dispose();
        }
    }
}