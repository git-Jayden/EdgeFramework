using System;
using System.Collections.Generic;
using UnityEngine;
namespace Ls_Mobile
{
    public class FSM<TStateEnum, TEventEnum> : IDisposable
    {
        private Action<TStateEnum, TStateEnum> mOnStateChanged = null;

        public FSM(Action<TStateEnum, TStateEnum> onStateChanged = null)
        {
            mOnStateChanged = onStateChanged;

        }

        /// <summary>
        /// FSM onStateChagned.
        /// </summary>
        public delegate void FSMOnStateChagned(params object[] param);

        /// <summary>
        /// QFSM state.
        /// </summary>
        class FSMState<TName>
        {
            public TName Name;
     
            public FSMState(TName name)
            {
                Name = name;
            }

            /// <summary>
            /// The translation dict.
            /// </summary>
            public readonly Dictionary<TEventEnum, FSMTranslation<TName, TEventEnum>> TranslationDict =
                new Dictionary<TEventEnum, FSMTranslation<TName, TEventEnum>>();
        }

        /// <summary>
        /// Translation 
        /// </summary>
        public class FSMTranslation<TStateName, KEventName>
        {
            public TStateName FromState;
            public KEventName Name;
            public TStateName ToState;
            public Action<object[]> OnTranslationCallback; // 回调函数

            public FSMTranslation(TStateName fromState, KEventName name, TStateName toState,
                Action<object[]> onStateChagned)
            {
                FromState = fromState;
                ToState = toState;
                Name = name;
                OnTranslationCallback = onStateChagned;
            }
        }

        /// <summary>
        /// The state of the m current.
        /// </summary>
        TStateEnum mCurState;

        public TStateEnum State
        {
            get { return mCurState; }
        }

        /// <summary>
        /// The m state dict.
        /// </summary>
        Dictionary<TStateEnum, FSMState<TStateEnum>> mStateDict = new Dictionary<TStateEnum, FSMState<TStateEnum>>();

        /// <summary>
        /// Adds the state.
        /// </summary>
        /// <param name="name">Name.</param>
        private void AddState(TStateEnum name)
        {
            mStateDict[name] = new FSMState<TStateEnum>(name);
        }

        /// <summary>
        /// Adds the translation.
        /// </summary>
        /// <param name="fromState">From state.</param>
        /// <param name="name">Name.</param>
        /// <param name="toState">To state.</param>
        /// <param name="onStateChagned">Callfunc.</param>
        public void AddTransition(TStateEnum fromState, TEventEnum name, TStateEnum toState, Action<object[]> onStateChagned = null)
        {
            if (!mStateDict.ContainsKey(fromState))
            {
                AddState(fromState);
            }

            if (!mStateDict.ContainsKey(toState))
            {
                AddState(toState);
            }

            mStateDict[fromState].TranslationDict[name] = new FSMTranslation<TStateEnum, TEventEnum>(fromState, name, toState, onStateChagned);
        }

        /// <summary>
        /// Start the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public void Start(TStateEnum name)
        {
            mCurState = name;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="param">Parameter.</param>
        public void HandleEvent(TEventEnum name, params object[] param)
        {
            if (mCurState != null && mStateDict[mCurState].TranslationDict.ContainsKey(name))
            {
                var tempTranslation = mStateDict[mCurState].TranslationDict[name];
                tempTranslation.OnTranslationCallback.InvokeGracefully(param);
                mOnStateChanged.InvokeGracefully(mCurState, tempTranslation.ToState);
                mCurState = tempTranslation.ToState;
            }
        }

        /// <summary>
        /// Clear this instance.
        /// </summary>
        public void Clear()
        {
            mStateDict.Values.ForEach(state =>
            {
                state.TranslationDict.Values.ForEach(translation => { translation.OnTranslationCallback = null; });
                state.TranslationDict.Clear();
            });

            mStateDict.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
