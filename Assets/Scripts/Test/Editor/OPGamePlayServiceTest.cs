using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace OPUnitTest
{
	
    [TestFixture()]
    public class OPGamePlayServiceTest
    {

        [Test()]
        public void testCaculateExp()
        {
            Assert.AreEqual(GamePlayService.Instance.calculateExp(-1), 0);
            Assert.AreEqual(GamePlayService.Instance.calculateExp(0), 0);
            Assert.AreEqual(GamePlayService.Instance.calculateExp(3), 0);
            Assert.AreEqual(GamePlayService.Instance.calculateExp(4), 2);
            Assert.AreEqual(GamePlayService.Instance.calculateExp(5), 4);
            Assert.AreEqual(GamePlayService.Instance.calculateExp(6), 8);
            Assert.AreEqual(GamePlayService.Instance.calculateExp(7), 16);
            Assert.AreEqual(GamePlayService.Instance.calculateExp(8), 32);
        }
        [Test()]
        public void testCalculateScore()
        {
            Assert.AreEqual(GamePlayService.Instance.calculateScore(1, 0), 50);
            Assert.AreEqual(GamePlayService.Instance.calculateScore(4, 0), 130);
        }
    }
}