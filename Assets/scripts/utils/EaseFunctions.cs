using UnityEngine;
using System.Collections;

public static class EaseFunctions
{
    public const float HALFPI = (Mathf.PI / 2.0f);

    public static class Sine
    {
        public static float EaseIn(float s)
        {
            return Mathf.Sin(s * HALFPI - HALFPI) + 1.0f;
        }

        public static float EaseOut(float s)
        {
            return Mathf.Sin(s * HALFPI);
        }

        public static float EaseInOut(float s)
        {
            return (Mathf.Sin(s * Mathf.PI - HALFPI) + 1) / 2.0f;
        }
    }

    public static class Power
    {
        public static float EaseIn(float s, int power)
        {
            return Mathf.Pow(s, power);
        }

        public static float EaseOut(float s, int power)
        {
            float sign = power % 2 == 0 ? -1.0f : 1.0f;
            return sign * (Mathf.Pow(s - 1, power) + sign);
        }

        public static float EaseInOut(float s, int power)
        {
            s *= 2.0f;
            if (s < 1.0f) return EaseIn(s, power) / 2.0f;
            float sign = power % 2 == 0 ? -1.0f : 1.0f;
            return sign / 2.0f * (Mathf.Pow(s - 2.0f, power) + sign * 2.0f);
        }
    }

    #region Linear

    public static float LinearNone(float k)
    {
        return k;
    }

    #endregion

    #region Quadratic

    public static float QuadraticIn(float k)
    {
        return Power.EaseIn(k, 2);
    }

    public static float QuadraticOut(float k)
    {
        return Power.EaseOut(k, 2);
    }

    public static float QuadraticInOut(float k)
    {
        return Power.EaseInOut(k, 2);
    }

    #endregion

    #region Cubic

    public static float CubicIn(float k)
    {
        return Power.EaseIn(k, 3);
    }

    public static float CubicOut(float k)
    {
        return Power.EaseOut(k, 3);
    }

    public static float CubicInOut(float k)
    {
        return Power.EaseInOut(k, 3);
    }

    #endregion

    #region Quart

    public static float QuartIn(float k)
    {
        return Power.EaseIn(k, 4);
    }

    public static float QuartOut(float k)
    {
        return Power.EaseOut(k, 4);
    }

    public static float QuartInOut(float k)
    {
        return Power.EaseInOut(k, 4);
    }

    #endregion

    #region Quint

    public static float QuintIn(float k)
    {
        return Power.EaseIn(k, 5);
    }

    public static float QuintOut(float k)
    {
        return Power.EaseOut(k, 5);
    }

    public static float QuintInOut(float k)
    {
        return Power.EaseInOut(k, 5);
    }

    #endregion

    #region Sinusoidal

    public static float SinusoidalIn(float k)
    {
        return -1.0f * Mathf.Cos(k * (Mathf.PI / 2.0f)) + 1.0f;
    }

    public static float SinusoidalOut(float k)
    {
        return Mathf.Sin(k * (Mathf.PI / 2.0f));
    }

    public static float SinusoidalInOut(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = SinusoidalIn(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = SinusoidalOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float SinusoidalOutIn(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = SinusoidalOut(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = SinusoidalIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    #endregion

    #region Exponential

    public static float ExponentialIn(float k)
    {
        return Mathf.Pow(2.0f, 10.0f * (k - 1.0f)) - 0.001f;
    }

    public static float ExponentialOut(float k)
    {
        return 1.001f * (-Mathf.Pow(2.0f, -10.0f * k) + 1.0f);
    }

    public static float ExponentialInOut(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = ExponentialIn(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = ExponentialOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float ExponentialOutIn(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = ExponentialOut(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = ExponentialIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    #endregion

    #region Circular

    public static float CircularIn(float k)
    {
        return -1.0f * (Mathf.Sqrt(1.0f - (k * k)) - 1.0f);
    }

    public static float CircularOut(float k)
    {
        return Mathf.Sqrt(1.0f - (k - 1.0f) * (k - 1.0f));
    }

    public static float CircularInOut(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = CircularIn(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = CircularOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float CircularOutIn(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = CircularOut(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = CircularIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    #endregion

    #region Elastic

    public static float ElasticIn(float k)
    {
        return (-1.0f * Mathf.Pow(2.0f, 10.0f * (k - 1.0f)) *
                Mathf.Sin(((k - 1.0f) - 0.075f) * (2.0f * Mathf.PI) / 0.3f));
    }

    public static float ElasticOut(float k)
    {
        return 1.0f * Mathf.Pow(2.0f, -10.0f * k) * Mathf.Sin((k - 0.075f) * (2.0f * Mathf.PI) / 0.3f) + 1.0f;
    }

    public static float ElasticInOut(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = ElasticIn(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = ElasticOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float ElasticOutIn(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = ElasticOut(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = ElasticIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    #endregion

    #region Back

    public static float BackIn(float k)
    {
        return k * k * ((1.7016f + 1.0f) * k - 1.7016f);
    }

    public static float BackOut(float k)
    {
        return (k - 1.0f) * (k - 1.0f) * ((1.7016f + 1.0f) * (k - 1.0f) + 1.7016f) + 1.0f;
    }

    public static float BackInOut(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = BackIn(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = BackOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float BackOutIn(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = BackOut(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = BackIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    #endregion

    #region Bounce

    public static float BounceIn(float k)
    {
        k = 1.0f - k;
        if (k < 1.0f / 2.75f)
            k = 7.5625f * k * k;
        else if (k < 2.0f / 2.75f)
        {
            k = k - (1.5f / 2.75f);
            k = 7.5625f * k * k + 0.75f;
        }
        else if (k < 2.5f / 2.75f)
        {
            k = k - (2.25f / 2.75f);
            k = 7.5625f * k * k + 0.9375f;
        }
        else
        {
            k = k - (2.625f / 2.75f);
            k = 7.5625f * k * k + 0.984375f;
        }
        return 1.0f - k;
    }

    public static float BounceOut(float k)
    {
        if (k < 1.0f / 2.75f)
            k = 7.5625f * k * k;
        else if (k < 2.0f / 2.75f)
        {
            k = k - (1.5f / 2.75f);
            k = 7.5625f * k * k + 0.75f;
        }
        else if (k < 2.5f / 2.75f)
        {
            k = k - (2.25f / 2.75f);
            k = 7.5625f * k * k + 0.9375f;
        }
        else
        {
            k = k - (2.625f / 2.75f);
            k = 7.5625f * k * k + 0.984375f;
        }
        return k;
    }

    public static float BounceInOut(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = BounceIn(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = BounceOut(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    public static float BounceOutIn(float k)
    {
        if (k <= 0.5f)
        {
            k = k * 2.0f;
            k = BounceOut(k);
            return k / 2.0f;
        }
        else
        {
            k = (k - 0.5f) * 2.0f;
            k = BounceIn(k);
            return (k / 2.0f) + 0.5f;
        }
    }

    #endregion
}
