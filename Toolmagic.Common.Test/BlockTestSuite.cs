using System;
using System.Linq;
using NUnit.Framework;

namespace Toolmagic.Common.Test
{
	[TestFixture]
	public sealed class BlockTestSuite
	{
		[Test]
		public void TryFinallyWithResultFailsOnFinallyBlockTest()
		{
			Assert.Throws<TestException>
				(
					() =>
					{
						Block
							.Try
							(
								() => "Test"
							)
							.Finally
							(
								() => { throw new TestException(); }
							);
					});
		}

		[Test]
		public void TryFinallyWithResultFailsOnNullFinallyActionTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() =>
					{
						const Action nullFinallyAction = null;

						Block
							.Try
							(
								() => "Test"
							)
							.Finally
							(
								nullFinallyAction
							);
					});
			Assert.AreEqual(@"finallyAction", exception.ParamName);
		}

		[Test]
		public void TryFinallyWithResultFailsOnNullTryFuncTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() =>
					{
						const Func<string> nullTryFunc = null;

						Block
							.Try
							(
								nullTryFunc
							)
							.Finally
							(
								() => { }
							);
					}
				);
			Assert.AreEqual(@"tryFunc", exception.ParamName);
		}

		[Test]
		public void TryFinallyWithResultFailsOnTryAndFinallyBlocksTest()
		{
			var exception = Assert.Throws<AggregateException>
				(
					() =>
					{
						const string expected = "Test";

						Block
							.Try
							(
								() =>
								{
									if (expected.Contains("est")) // to avoid compiler warnings
									{
										throw new TestException();
									}
									return expected;
								}
							)
							.Finally
							(
								() => { throw new Test2Exception(); }
							);
					});
			Assert.IsInstanceOf<TestException>(exception.InnerExceptions.First());
			Assert.IsInstanceOf<Test2Exception>(exception.InnerExceptions.Last());
		}

		[Test]
		public void TryFinallyWithResultFailsOnTryBlockTest()
		{
			Assert.Throws<TestException>
				(
					() =>
					{
						var expected = "Test";
						var finallyExecuted = false;

						try
						{
							Block
								.Try
								(
									() =>
									{
										if (expected.Contains("est")) // to avoid compiler warnings
										{
											throw new TestException();
										}
										return expected;
									}
								)
								.Finally
								(
									() => { finallyExecuted = true; }
								);
						}
						finally
						{
							Assert.IsTrue(finallyExecuted);
						}
					});
		}

		[Test]
		public void TryFinallyWithResultTest()
		{
			var expected = "Test";
			var finallyExecuted = false;

			var actual = Block
				.Try
				(
					() => expected
				)
				.Finally
				(
					() => { finallyExecuted = true; }
				);

			Assert.IsTrue(finallyExecuted);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void VoidTryFinallyFailsOnNullFinallyBlockTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() =>
					{
						const Action nullFinallyAction = null;

						Block
							.Try
							(
								() => { }
							)
							.Finally
							(
								nullFinallyAction
							);
					}
				);
			Assert.AreEqual(@"finallyAction", exception.ParamName);
		}

		[Test]
		public void VoidTryFinallyFailsOnNullTryActionTest()
		{
			var exception = Assert.Throws<ArgumentNullException>
				(
					() =>
					{
						const Action nullTryAction = null;
						var finallyExecuted = false;

						try
						{
							Block
								.Try
								(
									nullTryAction
								)
								.Finally
								(
									() => { finallyExecuted = true; }
								);
						}
						finally
						{
							Assert.IsFalse(finallyExecuted);
						}
					}
				);
			Assert.AreEqual(@"tryAction", exception.ParamName);
		}

		[Test]
		public void VoidTryFinallyTest()
		{
			var counter = 0;

			Block
				.Try
				(
					() => { counter++; }
				)
				.Finally
				(
					() => { counter += 3; }
				);

			Assert.AreEqual(4, counter);
		}
	}
}