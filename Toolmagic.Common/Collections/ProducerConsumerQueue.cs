using System;
using System.Collections.Generic;
using System.Threading;

namespace Toolmagic.Common.Collections
{
	public sealed class ProducerConsumerQueue<T> : IDisposable
	{
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly Action<T> _consumerAction;
		private readonly Thread _consumerThread;
		private readonly object _lockObject = new object();
		private readonly Queue<T> _queue = new Queue<T>();

		private ProducerConsumerQueue(Action<T> consumerAction)
		{
			Argument.IsNotNull(consumerAction, nameof(consumerAction));

			_consumerAction = consumerAction;
			_consumerThread = new Thread(Consume);
		}

		public void Dispose()
		{
			Shutdown();
		}

		public static ProducerConsumerQueue<T> Start(Action<T> consumerAction)
		{
			var queue = new ProducerConsumerQueue<T>(consumerAction);
			queue.Start();
			return queue;
		}

		private void Start()
		{
			_consumerThread.Start();
		}

		public void Shutdown()
		{
			_cancellationTokenSource.Cancel();
			lock (_lockObject)
			{
				Monitor.Pulse(_lockObject);
			}
			_consumerThread.Join();
		}

		public void Enqueue(T item)
		{
			lock (_lockObject)
			{
				_queue.Enqueue(item);
				Monitor.Pulse(_lockObject);
			}
		}

		private void Consume()
		{
			while (!_cancellationTokenSource.IsCancellationRequested)
			{
				T item;
				lock (_lockObject)
				{
					while (_queue.Count == 0)
					{
						Monitor.Wait(_lockObject);
					}

					item = _queue.Dequeue();
				}

				if (item == null)
				{
					return;
				}

				_consumerAction.Invoke(item);
			}
		}
	}
}