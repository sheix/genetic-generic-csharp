using System.Collections.Generic;
using System.Linq;
using DSP;
using NUnit.Framework;

namespace DSPTest
{
    [TestFixture]
    public class SquareWaveShould
    {
        [Test]
        public void BeAWave()
        {
            var squareWave = new SquareWave(10,10,10,0);
            Assert.IsInstanceOf<Wave>(squareWave);
        }

        [Test]
        public void HaveHeightAndWidth()
        {
            var squareWave = new SquareWave(10,10,10,0);
            Assert.IsTrue(squareWave.Height > 0);
            Assert.IsTrue(squareWave.Width > 0);
        }
        
        
    }

    [TestFixture]
    public class WaveResultShould
    {
        [Test]
        public void ProduceZeroResultWhenNoWaves()
        {
            var result = new ResultWave();
            result.Generate(100);
            int sum = result.Generate(100).Aggregate(0, (current, b) => current + b);

            Assert.AreEqual(0,sum);
        }

        [Test]
        public void ProduceNonZeroImpactWithSquareWave()
        {
            var result = new ResultWave();
            result.AddWave(new SquareWave(10,10,10,0));
            
            int sum = result.Generate(100).Aggregate(0, (current, b) => current + b);
            Assert.IsTrue(sum>0);
        }

    }

}
