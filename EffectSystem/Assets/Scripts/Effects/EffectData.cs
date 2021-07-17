using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Effects.Data
{
    public enum StackMode : byte { None, Time , Multiplier }
    public enum ElapseMode : byte { Destroy, UseStack }

    public interface IEffectHandler
    {
        void DetachEffect(int id);
        void AttachEffect(Effect effect);
        void DetachEffect(Effect effect);
    }

    public class EffectManager
    {
        private Transform _containerTransform;
        private GameObject _container;

        public List<Effect> Effects { get; private set; } 
            = new List<Effect>();

        


        public void Initialize(MonoBehaviour owner)
        {
            GameObject container = new GameObject("Effects");
            container.transform.parent = owner.transform;

            _container = container;
            _containerTransform = _container.transform;
        }


        public Effect RemoveEffect(int id)
        {
            Effect selectedEffect = Effects.SingleOrDefault((attachedEffect) => attachedEffect.Id == id);
            if (selectedEffect == null) return null;

            selectedEffect.Detach();

            return selectedEffect;
        }

        public bool HasEffect(Effect effect)
        {
            return Effects.SingleOrDefault((attachedEffect) => attachedEffect == effect) != null;
        }

        /// <summary>
        /// Adds effect to the entity
        /// </summary>
        /// <param name="effect">Copy of the effect should be sent. Otherwise same effect won't be applied to another entities </param>
        public void AddEffect(Effect effect)
        {
            Effects.RemoveAll((attachedEffect) => attachedEffect == null);

            for (int i = 0; i < Effects.Count; i++)
                if (Effects[i].Stack(effect)) return;
            
            effect.Activate();
            effect.Transform.parent = _containerTransform;

            Effects.Add(effect);
        }

        public Effect RemoveEffect(Effect effect)
        {
            Effect selectedEffect = Effects.SingleOrDefault((attachedEffect)=> attachedEffect == effect);
            if (selectedEffect == null) return null;

            selectedEffect.Detach();

            return selectedEffect;
        }

        /// <summary>
        /// Adds effects to the entity
        /// </summary>
        /// <param name="effects">Copy of the effects should be sent. Otherwise same effects won't be applied to another entities </param>
        public void AddEffects(IEnumerable<Effect> effects)
        {
            for (int i = Effects.Count() - 1; i >= 0; i--)
            {
                AddEffect(effects.ElementAt(i));
            }
        }
    }

    public class EffectContainer
    {
        private List<Effect> _effects;

        public List<Effect> Effects
        {
            get
            {
                return
                    _effects ?? (_effects = new List<Effect>());
            }
        }


        public void AttachEffects(IEnumerable<Effect> effects)
        {
            Effects.AddRange(effects);
        }
    }
}
