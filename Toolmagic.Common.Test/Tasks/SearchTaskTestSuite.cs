using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Toolmagic.Common.Tasks;

namespace Toolmagic.Common.Test.Tasks
{
	[TestFixture]
	public sealed class SearchTaskTestSuite
	{
		private const int InitialItemsCount = 2;
		private const int HierarchyDepth = 5;
		private const int SubItemsCount = 3;

		private static int GetExpectedItems(int hierarchyDepth, int initialItemsCount, int subItemsCount)
		{
			Assert.IsTrue(hierarchyDepth > 0, "hierarchyDepth must be greater than 0");
			Assert.IsTrue(initialItemsCount > 0, "initialItemsCount must be greater than 0");
			Assert.IsTrue(subItemsCount > 0, "subItemsCount must be greater than 0");

			var sum = 0;
			for (var i = 0; i < hierarchyDepth; i++)
			{
				sum += (int) Math.Pow(subItemsCount, i);
			}

			return initialItemsCount*sum;
		}

		private async Task StartUrlTest(ConcurrentBag<string> outputItems, CancellationToken cancellationToken,
			SearchTaskOptions options = null)
		{
			Assert.IsNotNull(outputItems);
			Assert.IsNotNull(cancellationToken);

			var searchOptions = options ??
			                    new SearchTaskOptions
			                    {
				                    MaxDegreeOfParallelism = Environment.ProcessorCount,
				                    MaxHierarchyDepth = HierarchyDepth,
				                    MaxQueueLength = int.MaxValue,
				                    DequeueWaitTimeout = 5
			                    };

			var initialUrls = new Collection<string>();
			for (var i = 0; i < InitialItemsCount; i++)
			{
				initialUrls.Add($"http://{i}");
			}

			await SearchTask.StartNew
				(
					initialUrls,
					(url, token) =>
					{
						outputItems.Add(url);

						Thread.Sleep(10);

						var subItems = new Collection<string>();

						for (var i = 0; i < SubItemsCount; i++)
						{
							subItems.Add($"{url}/{i}");
						}

						return subItems;
					},
					cancellationToken,
					searchOptions
				);
		}

		[Test]
		public void SearchTaskCompletesTest()
		{
			// Arrange
			var outputItems = new ConcurrentBag<string>();
			var expectedItemsCount = GetExpectedItems(HierarchyDepth, InitialItemsCount, SubItemsCount);

			// Act
			var cancellationTokenSource = new CancellationTokenSource();
			var task = StartUrlTest(outputItems, cancellationTokenSource.Token);
			task.Wait(CancellationToken.None);

			// Assert
			//if (expectedItemsCount != outputItems.Count)
			//{
			//	outputItems.ToList().ForEach(System.Console.WriteLine);
			//}

			Assert.AreEqual(expectedItemsCount, outputItems.Count);
		}

		[Test]
		public void SearchTaskCompletesWithCustomOptionsTest()
		{
			// Arrange
			var outputItems = new ConcurrentBag<string>();

			var customHierarchyDelpth = HierarchyDepth - 1;
			var expectedItemsCount = GetExpectedItems(customHierarchyDelpth, InitialItemsCount, SubItemsCount);

			var options =
				new SearchTaskOptions
				{
					MaxDegreeOfParallelism = 2, //Environment.ProcessorCount * 2,
					MaxHierarchyDepth = customHierarchyDelpth,
					MaxQueueLength = int.MaxValue,
					DequeueWaitTimeout = 10
				};

			// Act
			var cancellationTokenSource = new CancellationTokenSource();
			var task = StartUrlTest(outputItems, cancellationTokenSource.Token, options);
			task.Wait(cancellationTokenSource.Token);

			// Assert
			//if (expectedItemsCount != outputItems.Count)
			//{
			//	outputItems.ToList().ForEach(System.Console.WriteLine);
			//}

			Assert.AreEqual(expectedItemsCount, outputItems.Count);
		}

		[Test]
		public void SearchTaskFailsWithEmptyInitialItemCollectionTest()
		{
			Func<string, CancellationToken, IEnumerable<string>> action =
				(url, token) => Enumerable.Empty<string>();

			var emptyInilialItems = Enumerable.Empty<string>();

			try
			{
				SearchTask
					.StartNew(emptyInilialItems, action, CancellationToken.None)
					.Wait(CancellationToken.None);
			}
			catch (AggregateException ex)
			{
				Assert.AreEqual(1, ex.InnerExceptions.Count);
				Assert.IsTrue(ex.InnerExceptions.First() is ArgumentException);
			}
		}

		[Test]
		public void SearchTaskFailsWithNullActionTest()
		{
			const Func<string, CancellationToken, IEnumerable<string>> nullAction = null;

			IEnumerable<string> inilialItems = new[] {"http://mail.ru"};

			try
			{
				SearchTask
					.StartNew(inilialItems, nullAction, CancellationToken.None)
					.Wait(CancellationToken.None);
			}
			catch (AggregateException ex)
			{
				Assert.AreEqual(1, ex.InnerExceptions.Count);
				Assert.IsTrue(ex.InnerExceptions.First() is ArgumentNullException);
			}
		}

		[Test]
		public void SearchTaskFailsWithNullInitialItemCollectionTest()
		{
			Func<string, CancellationToken, IEnumerable<string>> action =
				(url, token) => Enumerable.Empty<string>();

			const IEnumerable<string> nullInilialItems = null;

			try
			{
				SearchTask
					.StartNew(nullInilialItems, action, CancellationToken.None)
					.Wait(CancellationToken.None);
			}
			catch (AggregateException ex)
			{
				Assert.AreEqual(1, ex.InnerExceptions.Count);
				Assert.IsTrue(ex.InnerExceptions.First() is ArgumentNullException);
			}
		}

		[Test]
		public void SearchTaskIsAbortedTest()
		{
			// Arrange
			var outputItems = new ConcurrentBag<string>();
			var totalItemsCount = GetExpectedItems(HierarchyDepth, InitialItemsCount, SubItemsCount);

			// Act
			var cancellationTokenSource = new CancellationTokenSource();
			var task = StartUrlTest(outputItems, cancellationTokenSource.Token);

			task.Wait(100, cancellationTokenSource.Token);
			try
			{
				cancellationTokenSource.Cancel();
				task.Wait(cancellationTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				// It's expected
			}

			// Assert
			Assert.IsTrue
				(
					totalItemsCount > outputItems.Count,
					$"Too many items for cancelled task: {outputItems.Count}. If the task completes successfully, queue size must be equal {totalItemsCount}"
				);
		}

		[Test]
		public void SearchTaskRunsWithVariousParallelismValueTest(
			[Values(2, 4, 6, 8, 10, 12, 16, 24, 32)] int degreeOfParallelism)
		{
			// Arrange
			var outputItems = new ConcurrentBag<string>();

			var customHierarchyDelpth = HierarchyDepth - 1;
			var expectedItemsCount = GetExpectedItems(customHierarchyDelpth, InitialItemsCount, SubItemsCount);

			var options =
				new SearchTaskOptions
				{
					MaxDegreeOfParallelism = degreeOfParallelism,
					MaxHierarchyDepth = customHierarchyDelpth,
					MaxQueueLength = int.MaxValue,
					DequeueWaitTimeout = 10
				};

			// Act
			var cancellationTokenSource = new CancellationTokenSource();
			var task = StartUrlTest(outputItems, cancellationTokenSource.Token, options);
			task.Wait(cancellationTokenSource.Token);

			// Assert
			//if (expectedItemsCount != outputItems.Count)
			//{
			//	outputItems.ToList().ForEach(System.Console.WriteLine);
			//}

			Assert.AreEqual(expectedItemsCount, outputItems.Count);
		}
	}
}