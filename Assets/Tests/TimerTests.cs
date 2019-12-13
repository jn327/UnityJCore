using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class TimerTests
    {
        [Test]
        public void Timer_DoesTickAndComplete()
        {
            Timer timer = new Timer(10.0f);

            bool isComplete = false;
            timer.onComplete += delegate { isComplete = true; };
            int ittr = 0;
            
            while(!isComplete && ittr < 5000)
            {
                timer.Tick(Random.value);
                ittr ++;
            }

            Assert.That( isComplete = true );
        }
    }
}
