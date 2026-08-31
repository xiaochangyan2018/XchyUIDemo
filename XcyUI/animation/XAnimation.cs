using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XcyUI.utils;

namespace XcyUI.animation
{
    public class XAnimation
    {
        private static readonly Stopwatch _stopwatch = new Stopwatch();
        private static XAnimation animation = new XAnimation();
        private List<XAnimate> animates;
        private List<XAnimate> redAnimates = new List<XAnimate>();
        private XAnimation()
        {
            animates = new List<XAnimate>();
            _stopwatch.Start();
        }

        /// <summary>
        /// 判断是否有动画
        /// </summary>
        /// <returns></returns>
        public static bool IsStart()
        {
            return animation.animates.Count > 0;
        }

        public static int AnimatCount()
        {
            return animation.animates.Count;
        }

        /// <summary>
        /// 创建一个默认的动画
        /// </summary>
        /// <returns></returns>
        public static XAnimate AnimateFloatOf()
        {
            return AnimateFloatOf(0, 1);
        }

        /// <summary>
        /// 创建一个动画
        /// </summary>
        /// <param name="values">关键帧</param>
        /// <returns></returns>
        public static XAnimate AnimateFloatOf(params float[] values)
        {
            var item = new XAnimate()
            {
                Duration = 250,
                Interpolator = XAnimationInterpolator.AccelerateDecelerate
            };
            item.StartTime = _stopwatch.ElapsedMilliseconds;
            item.SetValues(values);
            return item;
        }

        public static void Push(XAnimate item)
        {
            item.StartTime = _stopwatch.ElapsedMilliseconds + item.Delay;
            if (!animation.animates.Contains(item))
            {
                animation.animates.Add(item);
            }
        }

        public static void Remove(XAnimate item)
        {
            animation.animates.Remove(item);
        }

        /// <summary>
        /// 处理每一个动画在当前时间的动画值
        /// </summary>
        public static void HandlerAnimationItems()
        {
            animation.HandleTimer();
        }
        private void HandleTimer()
        {
            var currentTime = _stopwatch.ElapsedMilliseconds;
            redAnimates.Clear();
            redAnimates.AddRange(animates);
            for (int i=0;i< redAnimates.Count;i++)
            {
                var item = redAnimates[i];
                if (currentTime - item.StartTime > item.Duration)
                {
                    if (item.Times == int.MaxValue)
                    {
                        RenderImp.Post(() =>
                        {
                            item.OnCallback?.Invoke(item.Values[item.Values.Length - 1], item.Values.Length - 1);
                        });
                        item.StartTime = _stopwatch.ElapsedMilliseconds;
                    }
                    else if (item.Times > 1)
                    {
                        item.Times--;
                        RenderImp.Post(() =>
                        {
                            item.OnCallback?.Invoke(item.Values[item.Values.Length - 1], item.Values.Length - 1);
                        });
                        item.StartTime = _stopwatch.ElapsedMilliseconds;
                    }
                    else
                    {

                        RenderImp.Post(() =>
                        {
                            item.OnCallback?.Invoke(item.Values[item.Values.Length - 1], item.Values.Length - 1);
                            item.OnFinished?.Invoke();
                            animates.Remove(item);
                        });
                        continue;
                    }
                }
                // 获取时间进度
                float linearTime = (float)(currentTime - item.StartTime) / (float)item.Duration;
                // 找到分段区间第一段和第二段
                var keySegment = item.FintdSegmentValue(linearTime);
                if (!keySegment.Equals(SegmentValue.Empty))
                {
                    // 计算区间的相对位置
                    var localT = (linearTime - keySegment.KeyFrame0.KeyTime) / (keySegment.KeyFrame1.KeyTime - keySegment.KeyFrame0.KeyTime);
                    // 应用插值器
                    var interpolatedT = keySegment.Interpolator?.Invoke(localT) ?? localT;
                    // 获取具体的值
                    var value = keySegment.KeyFrame0.Value + interpolatedT * (keySegment.KeyFrame1.Value - keySegment.KeyFrame0.Value);

                    RenderImp.Post(() =>
                    {
                        item.OnCallback?.Invoke(value, keySegment.Index);
                    });
                }
            }
        }
    }

    internal struct KeyFrameSegment
    {
        internal float KeyTime { get; set; }
        internal float Value { get; set; }
    }

    internal struct SegmentValue
    {
        internal static readonly SegmentValue Empty = new SegmentValue();
        internal KeyFrameSegment KeyFrame0 { get; set; }
        internal KeyFrameSegment KeyFrame1 { get; set; }
        internal Func<float,float> Interpolator { get; set; }
        internal int Index { get; set; }
    }

    public class XAnimate
    {
        public XAnimate()
        {
            KeyFrames = new List<KeyFrameSegment>();
        }
        internal float[] Values { get; private set; }
        public long StartTime { get; set; }
        public int Duration { get; set; }
        public int Delay { get; set; }
        public int Times { get; set; }
        public Func<float, float> Interpolator { get; set; }

        public Func<float, float>[] Interpolators { get; private set; }
        public Action<float, int> OnCallback { get; set; }
        public Action OnCancel { get; set; }
        public Action OnStart { get; set; }
        public Action OnFinished { get; set; }

        internal List<KeyFrameSegment> KeyFrames { get; private set; }

        public float Value(float input, float start, float end)
        {
            return start + (end - start) * input;
        }

        public void SetValues(params float[] values)
        {
            Values = values;
            SetKeyValues();
        }

        public void SetInterpolators(params Func<float, float>[] interpolators)
        {
            Interpolators = interpolators;
        }

        public void SetKeyFrameTimes(params float[] times)
        {
            Duration = (int)times.Sum();
            var sum = times.Sum();
            var currentTime = 0f;
            for (int i = 1; i < KeyFrames.Count; i++)
            {
                var keyTime = currentTime + times[i - 1] / sum;
                currentTime += keyTime;
                KeyFrames[i] = new KeyFrameSegment()
                {
                    KeyTime = keyTime,
                    Value = KeyFrames[i].Value
                };
            }
        }

        public void ReStart()
        {
            OnCancel?.Invoke();
            OnStart?.Invoke();
            XAnimation.Push(this);
        }

        public void Start()
        {
            RenderImp.PostToQueue(() =>
            {
                OnStart?.Invoke();
                XAnimation.Push(this);
            });
        }

        public void Stop()
        {
            RenderImp.PostToQueue(() =>
            {
                OnCancel?.Invoke();
                XAnimation.Remove(this);
            });
        }
        private void SetKeyValues()
        {
            KeyFrames.Clear();
            var avg = 1f / (Values.Length - 1);
            for (int i = 0; i < Values.Length; i++)
            {
                KeyFrameSegment keyValue = new KeyFrameSegment();
                keyValue.KeyTime = avg * i;
                keyValue.Value = Values[i];
                KeyFrames.Add(keyValue);
            }
        }

        internal SegmentValue FintdSegmentValue(float linearTime)
        {
            for (int i = 0; i < KeyFrames.Count; i++)
            {

                if (i + 1 < KeyFrames.Count)
                {
                    var first = KeyFrames[i];
                    var second = KeyFrames[i + 1];
                    if (first.KeyTime <= linearTime && linearTime <= second.KeyTime)
                    {
                        var segmentValue = new SegmentValue();
                        segmentValue.KeyFrame0 = first;
                        segmentValue.KeyFrame1 = second;
                        segmentValue.Index = i;
                        segmentValue.Interpolator = Interpolators != null && Interpolators.Length > i ? segmentValue.Interpolator = Interpolators[i] : segmentValue.Interpolator = Interpolator;
                        return segmentValue;
                    }
                }
            }
            return SegmentValue.Empty;
        }
    }
}
