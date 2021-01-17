using System.Collections;
using System.Diagnostics;
using EdgeFramework;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ActionKitTests
    {

        //[UnityTest]
        public IEnumerator UntilActionTest()
        {
            var watch = new Stopwatch();
            var untilAction = UntilAction.Allocate(() => Time.time > 3);
            
            watch.Start();
            while (!untilAction.Execute(Time.deltaTime))
            {
                yield return new WaitForEndOfFrame();
            }

            watch.Stop();
           Debug.Log(watch.ElapsedMilliseconds);

    }

        //[Test]
        public void OnlyBeginActionTest()
        {
            var called = false;
            var onlyBeginAction = OnlyBeginAction.Allocate((action) =>
            {

                called = true;
            });

            onlyBeginAction.Execute(Time.deltaTime);
        Debug.Log(called);

        }

        //[Test]
        public void EventActionTest()
        {
            var called = false;
            
            var eventAction = EventAction.Allocate(() => { called = true; });
            
            eventAction.Execute(Time.deltaTime);
        Debug.Log(called);

    }

    //[UnityTest]
    public IEnumerator DelayActionTest()
        {
            var watch = new Stopwatch();
            watch.Start();
            var delayAction = DelayAction.Allocate(1, () =>
            {
                watch.Stop();
            });

            while (!delayAction.Execute(Time.deltaTime))
            {
                yield return new WaitForEndOfFrame();
            }
        Debug.Log(watch.ElapsedMilliseconds);

        }
        

        //[UnityTest]
        public IEnumerator RepeatNodeTest()
        {
            var callCount = 0;

            var delayAction = DelayAction.Allocate(1.0f, () => { callCount++; });

            var repeatNode = new RepeatNode(delayAction, 2);
            
            while (!repeatNode.Execute(Time.deltaTime))
            {
                yield return null;
            }
        Debug.Log(callCount);

        }


        //[UnityTest]
        public IEnumerator TimelineNodeTest()
        {
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            
            var timelineNode = new Timeline();
            timelineNode.Append(1.0f,EventAction.Allocate(() =>
            {
                stopwatch.Stop();
            }));
            
            while (!timelineNode.Execute(Time.deltaTime))
            {
                yield return null;
            }
        Debug.Log(stopwatch.ElapsedMilliseconds);
   
        }
        
        //[UnityTest]
        public IEnumerator SpawnNodeTest()
        {
            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            
            var spawnNode = new SpawnNode();
            spawnNode.Add(DelayAction.Allocate(1, () => {  }));
            spawnNode.Add(DelayAction.Allocate(1, () => {  }));
            spawnNode.Add(DelayAction.Allocate(1, () => { stopwatch.Stop(); }));
            
            while (!spawnNode.Execute(Time.deltaTime))
            {
                yield return null;
            }
        Debug.Log(stopwatch.ElapsedMilliseconds);

        }
        
        //[UnityTest]
        public IEnumerator SequenceNodeTest()
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var sequenceNode = new SequenceNode();
            sequenceNode.Append(DelayAction.Allocate(1, () => {  }));
            sequenceNode.Append(DelayAction.Allocate(1, () => {  }));
            sequenceNode.Append(DelayAction.Allocate(1, () => { stopwatch.Stop(); }));
            
            while (!sequenceNode.Execute(Time.deltaTime))
            {
                yield return null;
            }
        Debug.Log(stopwatch.ElapsedMilliseconds);
    
        }
    }
