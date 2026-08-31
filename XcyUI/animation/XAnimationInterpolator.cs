using System;

namespace XcyUI.animation
{
    public class XAnimationInterpolator
    {
        /// <summary>
        /// 简单的缓动下落
        /// </summary>
        public static Func<float, float> Bounce = (input) =>
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            if (input < 1 / d1)
            {
                return n1 * input * input;
            }
            else if (input < 2 / d1)
            {
                return n1 * (input -= 1.5f / d1) * input + 0.75f;
            }
            else if (input < 2.5 / d1)
            {
                return n1 * (input -= 2.25f / d1) * input + 0.9375f;
            }
            else
            {
                return n1 * (input -= 2.625f / d1) * input + 0.984375f;
            }
        };

        /// <summary>
        /// 减速运动
        /// </summary>
        public static Func<float, float> Decelerate = (input) =>
        {
            return 1.0f - (1.0f - input) * (1.0f - input);
        };

        /// <summary>
        /// 匀速运动
        /// </summary>
        public static Func<float, float> Uniform = (input) =>
        {
            return input;
        };
        /// <summary>
        /// 加速运动
        /// </summary>
        public static Func<float, float> Accelerate = (input) =>
        {
            return input * input;
        };

        /// <summary>
        /// 加速再减速
        /// </summary>
        public static Func<float, float> AccelerateDecelerate = (input) =>
        {
            return (float)(1 - Math.Cos(input * Math.PI)) / 2f;
        };

        /// <summary>
        /// 减速再加速
        /// </summary>
        public static Func<float, float> DecelerateAccelerate = (input) =>
        {
            return (float)Math.Sin(input * Math.PI / 2);
        };

        /// <summary>
        /// 预紧 / 先回后加速
        /// </summary>
        public static Func<float, float> Anticipate = (input) =>
        {
            var tension = 2;
            return tension * input * input * input - input * input;
        };

        /// <summary>
        /// 预紧回弹 / 先回后超调
        /// </summary>
        public static Func<float, float> AnticipateOvershoot = (input) =>
        {
            var tension = 2;
            if (input < 0.5f)
                return 0.5f * (tension * (float)Math.Pow(2 * input, 3) - (float)Math.Pow(2 * input, 2));
            else
                return 0.5f * ((float)Math.Pow(2 * input - 2, 2) * (1 + tension * (2 * input - 2)) + 2);
        };


        /// <summary>
        /// 回弹 / 超调回弹
        /// </summary>
        public static Func<float, float> OverShoot = (input) =>
        {
            var tension = 2;
            return (float)(input - tension * Math.Sin(input * Math.PI) * Math.Pow(2, -10 * input));
        };

        /// <summary>
        /// 指数加速
        /// </summary>
        public static Func<float, float> ExponentialAccelerate = (input) =>
        {
            return input == 0 ? 0 : (float)Math.Pow(2, 10 * (input - 1));
        };

        /// <summary>
        /// 指数减速
        /// </summary>
        public static Func<float, float> ExponentialDecelerate = (input) =>
        {
            return input == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * input);
        };

        /// <summary>
        /// 正弦加速
        /// </summary>
        public static Func<float, float> SineAccelerate = (input) =>
        {
            return (float)(1 - Math.Cos(input * Math.PI / 2));
        };

        /// <summary>
        /// 正弦减速
        /// </summary>
        public static Func<float, float> SineDecelerate = (input) =>
        {
            return (float)Math.Sin(input * Math.PI / 2);
        };
    }
}
