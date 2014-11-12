using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace OPUnitTest{
	
	[TestFixture()]
	public class OPGamePlayServiceTest
	{

		[Test()]
		public void testCaculateExp()
		{
			Assert.AreEqual(GamePlayService.Instance.caculateExp(-1), 0);
			Assert.AreEqual(GamePlayService.Instance.caculateExp(0), 0);
			Assert.AreEqual(GamePlayService.Instance.caculateExp(3), 0);
			Assert.AreEqual(GamePlayService.Instance.caculateExp(4), 2);
			Assert.AreEqual(GamePlayService.Instance.caculateExp(5), 4);
			Assert.AreEqual(GamePlayService.Instance.caculateExp(6), 8);
			Assert.AreEqual(GamePlayService.Instance.caculateExp(7), 16);
			Assert.AreEqual(GamePlayService.Instance.caculateExp(8), 32);
		}
	}
}