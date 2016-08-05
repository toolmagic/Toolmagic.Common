using System;
using System.Collections.Generic;
using System.Threading;

namespace Toolmagic.Common.Collections
{
	public sealed class ProducerConsumerQueue<T> : IDisposable
	{
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly Action<T, CancellationToken> _consumerAction;
		private readonly List<Thread> _consumerThreads = new List<Thread>();
		private readonly object _lockObject = new object();
		private readonly Queue<T> _queue = new Queue<T>();

		private ProducerConsumerQueue(Action<T, CancellationToken> consumerAction, int consumerCount)
		{
			_consumerAction = consumerAction;

			for (var i = 0; i < consumerCount; i++)
			{
				_consumerThreads.Add(new Thread(Consume));
			}
		}

		public void Dispose()
		{
			Shutdown();
		}

		public static ProducerConsumerQueue<T> Start(Action<T, CancellationToken> consumerAction, int consumerCount)
		{
			Argument.IsNotNull(consumerAction, nameof(consumerAction));
			Argument.IsInRange(consumerCount, nameof(consumerCount), 1, int.MaxValue);

			var queue = new ProducerConsumerQueue<T>(consumerAction, consumerCount);
			queue.Start();
			return queue;
		}

		private void Start()
		{
			_consumerThreads.ForEach(thread => thread.Start());
		}

		public void Shutdown()
		{
			_cancellationTokenSource.Cancel();

			lock (_lockObject)
			{
				Monitor.PulseAll(_lockObject);
			}

			_consumerThreads.ForEach(thread => thread.Join());
		}

		public void Enqueue(T item)
		{
			lock (_lockObject)
			{
				_queue.Enqueue(item);
				Monitor.PulseAll(_lockObject);
			}
		}

		private void Consume()
		{
			while (!_cancellationTokenSource.IsCancellationRequested)
			{
				T item;
				lock (_lockObject)
				{
					if (_queue.Count == 0)
					{
						Monitor.Wait(_lockObject);
					}

					if (_queue.Count == 0)
					{
						continue;
					}

					item = _queue.Dequeue();
				}

				if (item == null)
				{
					return;
				}

				_consumerAction.Invoke(item, _cancellationTokenSource.Token);
			}
		}
	}
}