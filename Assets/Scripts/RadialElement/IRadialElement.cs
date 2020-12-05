using System;

namespace alexnown
{
    public interface IRadialElement
    {
        event Action<float> ChangedNormValue;

        float MaxAngle { get; set; }
        float NormalizedValue { get; set; }
        float Angle { get; set; }
        bool ClockwiseDirection { get; set; }
    }
}
