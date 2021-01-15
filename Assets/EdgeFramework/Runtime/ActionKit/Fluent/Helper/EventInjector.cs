/****************************************************
	文件：EventInjector.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/15 9:10   	
	Features：
*****************************************************/

using System;

namespace EdgeFramework
{
    /// <summary>
    /// 事件注入,和 NodeSystem 配套使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventInjector<T>
    {
        public delegate bool InjectEventTrigger(T lastValue, T newValue);

        public delegate T InjectEventGetter();

        private T mCahedLastValue;

        public readonly Func<T> mGetter;

        public T Value
        {
            get { return mCahedLastValue; }
        }

        public EventInjector(Func<T> getter)
        {
            mGetter = getter;
        }

        public bool GetOn(InjectEventTrigger triggerConditionWithOldAndNewValue)
        {
            var value = mGetter();
            var trig = triggerConditionWithOldAndNewValue(mCahedLastValue, value);
            mCahedLastValue = value;
            return trig;
        }

        public bool GetOnValueChanged(Func<T, bool> triggerConditionWithNewValue = null)
        {
            return GetOn((lastValue, newValue) =>
                lastValue.Equals(newValue) &&
                (triggerConditionWithNewValue == null || triggerConditionWithNewValue(newValue)));
        }
    }
}