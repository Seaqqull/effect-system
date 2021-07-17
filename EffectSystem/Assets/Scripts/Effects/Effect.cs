using Effects.Data;
using UnityEngine;

namespace Effects
{
    public abstract class Effect : Utilities.BaseMono
    {
        [SerializeField] protected int _id;
        [SerializeField] protected bool _active;
        [SerializeField] protected bool _activeOnAttach;
        [Header("Properties")]
        [SerializeField] protected StackMode _stackMode;
        [SerializeField] protected ElapseMode _elapseMode;
        [SerializeField] protected float _timeMax;
        [SerializeField] protected float _timeLeft;
        [SerializeField] protected int _maxMultiplier = 1;
        [Header("Readonly")]
        [SerializeField] protected int _multiplier;

        protected IEffectHandler _owner;
        protected bool _wasActive;

        public float TimeLeftPercent
        {
            get { return _timeLeft / _timeMax; }
        }
        public float TimeLeft
        {
            get { return _timeLeft; }
        }
        public float TimeMax
        {
            get { return _timeMax; }
        }
        public int Id
        {
            get { return _id; }
        }


        protected override void Awake()
        {
            base.Awake();

            if (_timeLeft > _timeMax)
                _timeLeft = _timeMax;
            _multiplier = 1;
        }

        protected void OnEnable()
        {
            if(_wasActive)
                _active = true;
        }

        protected void OnDisable()
        {
            if (_active) _wasActive = true;
            _active = false;
        }

        protected void Update()
        {
            if (!_active) return;

            if (_timeMax > 0.0f)
            {
                if (_timeLeft < 0.0f)
                {
                    if ((_elapseMode == Data.ElapseMode.UseStack) &&
                        (_multiplier > 1))
                    {
                        _multiplier--;
                        _timeLeft = _timeMax;
                    }
                    else
                    {
                        OnElapsed();
                        return;
                    }
                }

                _timeLeft -= Time.deltaTime;
            }

            Process();
        }


        protected virtual void OnElapsed()
        {
            Destroy(GameObj);
        }


        protected abstract void Process();


        public void Detach()
        {
            _active = false;

            _owner = null;
        }

        public void Unstack()
        {
            if(_multiplier <= 1)
            {
                OnElapsed();
                return;
            }

            _multiplier--;
            _timeLeft = _timeMax;
        }

        public void Activate()
        {
            _active = true;
        }

        public void Deactivate()
        {
            _active = false;
        }

        public bool Stack(Effect effect)
        {
            if (Id != effect.Id) return false;
            if (_stackMode != StackMode.Time && _stackMode != StackMode.Multiplier) return false;

            if (_stackMode == StackMode.Time)
            {
                _timeLeft += effect._timeLeft;
                if (_timeLeft > _timeMax)
                    _timeLeft = _timeMax;
            }
            else if (_stackMode == StackMode.Multiplier)
            {
                _timeLeft = _timeMax;
                if(_multiplier < _maxMultiplier)
                    _multiplier += effect._multiplier;
            }

            effect.OnElapsed();
            return true;
        }

        public void Attach(IEffectHandler entity)
        {
            if (entity == null) return;

            if (_activeOnAttach && GameObj.activeInHierarchy)
                _active = true;
            _owner = entity;
        }

        #region Comparison
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public virtual bool Equals(Effect other)
        {
            return _id == other._id;
        }

        public override bool Equals(System.Object obj)
        {
            if ((obj == null) ||
                !(GetType().Equals(obj.GetType())))
                return false;

            var effect = obj as Effect;
            return  (effect != null) && Equals(effect);
        }
        #endregion
    }
}