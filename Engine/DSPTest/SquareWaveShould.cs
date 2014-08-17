using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            SquareWave squareWave = new SquareWave();
            Assert.IsInstanceOf<Wave>(squareWave);
        }

        [Test]
        public void HaveHeightAndWidth()
        {
            SquareWave squareWave = new SquareWave();
            Assert.IsTrue(squareWave.Height>0);
            //Assert.IsTrue(squareWave.Height>0);
        }
    }

    }
