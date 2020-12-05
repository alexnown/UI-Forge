using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace alexnown
{
    public class RadialSlider : Selectable, IDragHandler, IInitializePotentialDragHandler, IRadialElement, ISlider
    {
        public event Action<float> ChangedNormValue;

        /// <summary>
        /// Stops the slider when crossing over the 0-1 border. Set it value to 0 for smooth slider moving across start-end border.
        /// </summary>
        [Range(0, 0.5f)]
        public float BorderCrossingResistance = 0.5f;
        /// <summary>
        /// Allows you to setting max slider angle (for 180 and 90 angle sliders as example).
        /// </summary>
        [SerializeField, Range(10, 360)]
        private float _angleLimit = 360;
        [SerializeField]
        private bool _clockwiseDirection = true;
        [SerializeField]
        private float _minValue = 0.0f;
        [SerializeField]
        private float _maxValue = 1f;
        [SerializeField]
        private bool _wholeNumbers = false;
        [SerializeField]
        private float _value;
        [SerializeField, HideInInspector]
        private float _normValue;
        [Space]
        [SerializeField]
        private Slider.SliderEvent _onValueChanged = new Slider.SliderEvent();
        /// <summary>
        /// Allows you to ignore some clicks in the center and outside the slider circle.
        /// </summary>
        [Header("View")]
        public Vector2 RaycastLimits;
        [SerializeField]
        private RectTransform _rotatedTransform = null;

        private Camera _cachedMainCamera;
        private Canvas _parentCanvas;
        private bool _ignoreDrag;

        public Slider.SliderEvent ValueChanged
        {
            get => _onValueChanged;
            set => _onValueChanged = value;
        }

        public bool ClockwiseDirection
        {
            get => _clockwiseDirection;
            set
            {
                _clockwiseDirection = value;
                UpdateHanglerRotation();
            }
        }

        public float MaxAngle
        {
            get => _angleLimit;
            set => _angleLimit = value;
        }

        public float Value
        {
            get => _value;
            set { ApplyNormValue(RemapValueToNormValue(value)); }
        }

        public float NormalizedValue
        {
            get => _normValue;
            set { ApplyNormValue(value); }
        }

        public float Angle
        {
            get => NormalizedValue * MaxAngle; 
            set => ApplyNormValue(value / MaxAngle); 
        }

        public float MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public float MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            _ignoreDrag = !HitPositionInRaycastLimits(eventData.position);
            if (_ignoreDrag) return;
            var normValue = CalculateNormValueByInput(eventData.position);
            if (BorderCrossingResistance > 0 && MaxAngle >= 360)
                normValue = ApplyBroderCrossingResistanceToNewNormValue(normValue, _normValue, 0.02f);
            SetNormValueOnInitializeDrag(normValue);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (_ignoreDrag) return;
            var normValue = CalculateNormValueByInput(eventData.position);
            normValue = ApplyBroderCrossingResistanceToNewNormValue(normValue, _normValue, BorderCrossingResistance);
            ApplyNormValue(normValue);
        }

        protected virtual void SetNormValueOnInitializeDrag(float normValue)
        {
            ApplyNormValue(normValue);
        }

        protected void ApplyNormValue(float normValue)
        {
            _normValue = Mathf.Clamp01(normValue);
            float newValue = RemapNormValueToSliderValue(_normValue);
            if (_wholeNumbers)
            {
                newValue = Mathf.Round(newValue);
                _normValue = RemapValueToNormValue(newValue);
            }
            _value = newValue;
            if (_onValueChanged != null) _onValueChanged.Invoke(_value);
            if (ChangedNormValue != null) ChangedNormValue.Invoke(_normValue);
            UpdateHanglerRotation();
        }

        private float RemapNormValueToSliderValue(float normValue)
        {
            if (_minValue == _maxValue) return _minValue;
            var remaped = _minValue + (_maxValue - _minValue) * normValue;

            return remaped;
        }

        private float RemapValueToNormValue(float value)
        {
            if (_maxValue == _minValue) return 0;
            float remaped = (value - _minValue) / (_maxValue - _minValue);
            return remaped;
        }

        private float ApplyBroderCrossingResistanceToNewNormValue(float normValue, float prevValue, float resistance)
        {
            if (normValue < resistance)
            {
                if (prevValue > 0.75f) normValue = 1;
            }
            else if (normValue > 1 - resistance)
            {
                if (prevValue < 0.25f) normValue = 0;
            }
            return normValue;
        }

        private void UpdateHanglerRotation()
        {
            if (_rotatedTransform == null) return;
            var angle = transform.eulerAngles.z - _normValue * MaxAngle * (_clockwiseDirection ? 1 : -1);
            if (!_clockwiseDirection) angle = angle - MaxAngle;
            _rotatedTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private bool HitPositionInRaycastLimits(Vector2 pos)
        {
            if (RaycastLimits.x == 0 && RaycastLimits.y == 0) return true;
            var cam = GetCanvasCamera();
            Vector2 localPos;
            bool isOverlay = _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
            if (isOverlay) pos = cam.WorldToScreenPoint(pos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, pos, cam, out localPos);
            var length = localPos.magnitude;
            return length >= RaycastLimits.x && length <= RaycastLimits.y;
        }

        private float CalculateNormValueByInput(Vector2 pos)
        {
            var cam = GetCanvasCamera();
            bool isOverlay = _parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
            if (isOverlay) pos = cam.WorldToScreenPoint(pos);
            Vector2 diff = pos - RectTransformUtility.WorldToScreenPoint(cam, transform.position);
            var angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            var normValue = Mathf.Repeat((transform.eulerAngles.z - angle + 720f) / 360f, 1f) * 360 / MaxAngle;
            if (MaxAngle < 360 && normValue > 1)
            {
                var center = (360 / MaxAngle - 1) / 2 + 1;
                if (normValue < center) normValue = 1;
                else if (normValue >= center) normValue = 0;
            }
            if (!_clockwiseDirection) normValue = 1 - normValue;

            return normValue;
        }

        protected override void Awake()
        {
            base.Awake();
            _parentCanvas = GetComponentInParent<Canvas>();
        }

        private Camera GetCanvasCamera()
        {
            var cam = _parentCanvas.worldCamera;
            if (cam != null) return cam;
            if (_cachedMainCamera == null) _cachedMainCamera = Camera.main;
            return _cachedMainCamera;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_minValue > _maxValue)
            {
                float max = _minValue;
                _minValue = _maxValue;
                _maxValue = max;
            }
            ClockwiseDirection = _clockwiseDirection;
            Value = _value;
        }

        private void OnDrawGizmos()
        {
            if (UnityEditor.Selection.activeGameObject != gameObject ||
                RaycastLimits.Equals(Vector2.zero)) return;
            Gizmos.color = Color.red;
            var rectTransform = transform as RectTransform;
            var worldOffset = rectTransform.TransformPoint(RaycastLimits.x, 0, 0) - rectTransform.position;
            Gizmos.DrawWireSphere(rectTransform.position, worldOffset.magnitude);
            worldOffset = rectTransform.TransformPoint(RaycastLimits.y, 0, 0) - rectTransform.position;
            Gizmos.DrawWireSphere(rectTransform.position, worldOffset.magnitude);
        }
#endif

    }
}