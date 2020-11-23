using com.ls_mobile.tool;
using DG.Tweening;
using UniRx;
using UnityEngine;


namespace EdgeFramework.Example
{
    public class DelayExample : MonoBehaviour
    {
        public class DelayNodeExample : MonoBehaviour
        {
            void Start()
            {
                this.Delay(1.0f, () =>
                {
                    Log.I("延时 1s");
                });

                var delay2s = DelayAction.Allocate(2.0f, () => { Log.I("延时 2s"); });
                this.ExecuteNode(delay2s);
            }

            private DelayAction mDelay3s = DelayAction.Allocate(3.0f, () => { Log.I("延时 3s"); });

            private void Update()
            {

                //判断如果节点不为空并且节点执行完成了
                if (mDelay3s != null && !mDelay3s.Finished && mDelay3s.Execute(Time.deltaTime))
                {
                    Log.I("Delay3s 执行完成");
                }
            }
        }
        public class EventNodeExample : MonoBehaviour
        {
            private void Start()
            {
                var eventNode = EventAction.Allocate(() =>
                {
                    Log.I("event 1 called");

                }, () =>
                {
                    Log.I("event 2 called");

                });
                this.ExecuteNode(eventNode);
            }

            private EventAction mEventNode2 =
                EventAction.Allocate(() => { Log.I("event 3 called"); }, () => { Log.I("event 4 called"); });

            private void Update()
            {
                if (mEventNode2 != null && !mEventNode2.Finished && mEventNode2.Execute(Time.deltaTime))
                {
                    Log.I("eventNode2 执行完成");
                }
            }
        }
        public class SequenceNodeExample : MonoBehaviour
        {
            private void Start()
            {
                this.Sequence()
                    .Delay(1.0f)
                    .Event(() => Log.I("Sequence1 延时了 1s"))
                    .Begin()
                    .OnDisposed(() => { Log.I("Sequence1 destroyed"); });
                //添加三个节点到节点链中
                var sequenceNode2 = new SequenceNode(DelayAction.Allocate(1.5f));
                sequenceNode2.Append(EventAction.Allocate(() => Log.I("Sequence2 延时 1.5s")));
                sequenceNode2.Append(DelayAction.Allocate(0.5f));
                sequenceNode2.Append(EventAction.Allocate(() => Log.I("Sequence2 延时 2.0s")));
                //开始执行节点链
                this.ExecuteNode(sequenceNode2);

                /* 这种方式需要自己手动进行销毁
                sequenceNode2.Dispose();
                sequenceNode2 = null;
                */

                // 或者 OnDestroy 触发时进行销毁
                sequenceNode2.AddTo(this);
            }

            void OnLoginSucceed()
            {

            }

            private SequenceNode mSequenceNode3 = new SequenceNode(
                DelayAction.Allocate(3.0f),
                EventAction.Allocate(() => { Log.I("Sequence3 延时 3.0f"); }));

            private void Update()
            {
                if (mSequenceNode3 != null && !mSequenceNode3.Finished && mSequenceNode3.Execute(Time.deltaTime))
                {
                    Log.I("SequenceNode3 执行完成");
                }
            }

            private void OnDestroy()
            {
                mSequenceNode3.Dispose();
                mSequenceNode3 = null;
            }
        }
        public class CustomActionExample : MonoBehaviour
        {
            // Use this for initialization
            void Start()
            {
                this.ExecuteNode(OnlyBeginAction.Allocate(nodeAction =>
                {
                    this.transform.DOLocalMove(new Vector3(5, 5), 0.5f).OnComplete(() => { nodeAction.Finish(); });
                }));

                this.Sequence()
                    .Delay(1.0f)
                    .OnlyBegin(action =>
                    {
                        this.transform.DOLocalMove(new Vector3(-5, -5), 0.5f).OnComplete(() => { action.Finish(); });
                    })
                    .Begin();
            }
        }
        public class DotweenActionExample : MonoBehaviour
        {
            // Use this for initialization
            void Start()
            {
                this.ExecuteNode(DOTweenAction.Allocate(() => this.transform.DOLocalMoveX(5, 5)));

                this.Sequence()
                    .Delay(5.0f)
                    .Event(() => "开始执行 Sequence 中的 Dotween".LogInfo())
                    .DOTween(() => transform.DOLocalMoveX(-5, 5))
                    .Event(() => "结束执行 Sequence 中的 Dotween".LogInfo())
                    .Begin();
            }
        }
        public class WWWActionExample : MonoBehaviour
        {
            class WWWGetSikieduAction : NodeAction
            {
                protected override void OnBegin()
                {
                    ObservableWWW.Get("http://jianshu.com")
                        .Subscribe(text =>
                        {
                            text.LogInfo();
                            Finish();
                        }, e =>
                        {
                            e.LogException();
                        });
                }
            }


            // Use this for initialization
            void Start()
            {
                //			this.ExecuteNode(new WWWGetSikieduAction());

                //			this.Sequence()
                //				.Append(new WWWGetSikieduAction())
                //				.Begin();

                //			this.Repeat()
                //				.Append(new WWWGetSikieduAction())
                //				.Begin();

                ActionQueue.Append(new WWWGetSikieduAction());
            }
        }
        public class TestNodeSystemGC : MonoBehaviour
        {
            // Update is called once per frame
            void Update()
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    this.Sequence()
                        .Delay(3.0f)
                        .Event(() => Debug.Log("A"))
                        .Begin()
                        .OnDisposed(() => Log.I("Sequece: dispose when sequence ended"));
                }
            }
        }
        public class NodeExample : MonoBehaviour
        {

            void Start()
            {
                this.Sequence()
                   .Until(() => { return Input.GetKeyDown(KeyCode.Space); })
                   .Delay(2.0f)
                   .Event(() => { Debug.Log("延迟两秒"); })
                   .Delay(1f)
                   .Event(() => { Debug.Log("延迟一秒"); })
                   .Until(() => { return Input.GetKeyDown(KeyCode.A); })
                   .Event(() =>
                   {
                       this.Repeat()
                       .Delay(0.5f)
                       .Event(() => { Debug.Log("0.5s"); })
                       .Begin()
                       .DisposeWhen(() => { return Input.GetKeyDown(KeyCode.S); })
                       .OnDisposed(() => { Debug.Log("结束"); });
                   })
                    .Begin();
            }

            #region Update的情况
            //private float m_CurrentTime;
            //private bool isSpace = true;
            //private bool isBegin = false;
            //private bool isCanA = false;
            //private bool isA = false;
            //private bool isRepeatS = false;

            //private void Start()
            //{
            //    m_CurrentTime = Time.time;
            //}

            //private void Update()
            //{
            //    if (isSpace && Input.GetKeyDown(KeyCode.Space))
            //    {
            //        isSpace = false;
            //        isBegin = true;
            //        m_CurrentTime = Time.time;
            //    }

            //    if (isA && Input.GetKeyDown(KeyCode.A))
            //    {
            //        isA = false;
            //        isRepeatS = true;
            //        m_CurrentTime = Time.time;
            //    }

            //    if (isRepeatS)
            //    {
            //        if (Time.time - m_CurrentTime > 0.5f)
            //        {
            //            m_CurrentTime = Time.time;

            //            Debug.Log("0.5s");

            //        }

            //        if (Input.GetKeyDown(KeyCode.S))
            //        {
            //            Debug.Log("结束");
            //            isRepeatS = false;
            //        }
            //    }

            //    if (isBegin)
            //    {
            //        if (Time.time - m_CurrentTime > 2)
            //        {
            //            Debug.Log("延迟两秒");
            //            isBegin = false;
            //            isCanA = true;
            //            m_CurrentTime = Time.time;
            //        }
            //    }

            //    if (isCanA)
            //    {
            //        if (Time.time - m_CurrentTime > 1)
            //        {
            //            Debug.Log("延迟一秒");
            //            isCanA = false;
            //            isA = true;
            //        }
            //    }

            //}
            #endregion
        }
        public class NodeSystemExample : MonoBehaviour
        {
            private void Start()
            {
                this.Repeat()
                    .Delay(0.1f)
                    .Event(() => Log.I("Repeat:0.1s"))
                    .Begin()
                    .OnDisposed(() => Log.I("Repeat: dispose when gameObj OnDestroyedd"));

                this.Sequence()
                    .Delay(1.0f)
                    .Event(() => Log.I("Sequece:1.0s"))
                    .Begin()
                    .OnDisposed(() => Log.I("Sequece: dispose when sequence ended"));

                // TODO:spawn/timeline support
            }
        }
    }
}
