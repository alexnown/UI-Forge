using UnityEngine;
using UnityEngine.EventSystems;

namespace alexnown
{
    /// <summary>
    /// Provides smooth moving slider handler toward to target value after single click.
    /// </summary>
    public class UpdatableRadialSlider : RadialSlider
    {
        /// <summary>
        /// Affects the speed for slider handler moving to target value.
        /// </summary>
        public float UpdateSpeed = 1;

        private float _currNormValue;
        private float _targetNormValue;
        private bool _moveToTargetValue;

        protected override void SetNormValueOnInitializeDrag(float normValue)
        {
            if (UpdateSpeed > 0)
            {
                _moveToTargetValue = true;
                _currNormValue = NormalizedValue;
                _targetNormValue = normValue;
            }
            else base.SetNormValueOnInitializeDrag(normValue);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_moveToTargetValue)
                _moveToTargetValue = false;
            base.OnDrag(eventData);
        }

        protected virtual void LateUpdate()
        {
            if (!_moveToTargetValue) return;
            if (Mathf.Approximately(_currNormValue, _targetNormValue))
            {
                _moveToTargetValue = false;
                return;
            }
            _currNormValue = Mathf.MoveTowards(_currNormValue, _targetNormValue, UpdateSpeed * Time.deltaTime);
            ApplyNormValue(_currNormValue);
        }
    }
}
