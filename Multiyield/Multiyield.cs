using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Multiyield
{
    public static class Yield
    {
        public static EnumerableStep<T> OneOf<T>(T item) => new EnumerableStep<T>(item);
        public static EnumerableStep<T> Multiple<T>(IEnumerable<T> items) => new EnumerableStep<T>(items);
        public static ConcatEnumerable<T> Break<T>() => new ConcatEnumerable<T>();
    }

    public interface IEnumerableStep : INotifyCompletion
    {
    }

    public class EnumerableStep<T>: IEnumerableStep
    {
        public readonly IEnumerable<T> Items;

        public EnumerableStep()
        { }

        public EnumerableStep(IEnumerable<T> items)
        {
            Items = items;
        }

        public EnumerableStep(T item)
        {
            Items = Enumerable.Repeat(item, 1);
        }

        public EnumerableStep<T> GetAwaiter() => this;

        public bool IsCompleted { get; set; }
        public void OnCompleted(Action continuation)
        {
        }

        public void GetResult() { }
    }

    [AsyncMethodBuilder(typeof(AsyncBuilder<>))]
    public interface IConcatEnumerable<T> : IEnumerable<T>, INotifyCompletion
    {
    }

    public class ConcatEnumerable<T>: IConcatEnumerable<T>
    {
        public IAsyncStateMachine StateMachine { get; set; }
        public EnumerableStep<T> CurrentStep { get;  set; }
        public bool HasNext { get; set; }

        public bool IsCompleted { get; set; }
        public void OnCompleted(Action continuation)
        {
        }

        public void GetResult() { }
        public ConcatEnumerable<T> GetAwaiter() => this;

        public IEnumerator<T> GetEnumerator()
        {
            for (; ; )
            {
                StateMachine.MoveNext();

                if (!HasNext) yield break;

                foreach (var item in CurrentStep.Items)
                {
                    yield return item;
                }

                HasNext = false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class AsyncBuilder<T>
    {
        private readonly ConcatEnumerable<T> _enum = new ConcatEnumerable<T>();
        public static AsyncBuilder<T> Create() => new AsyncBuilder<T>();

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            _enum.StateMachine = stateMachine;
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        public void SetException(Exception exception)
        {

        }

        public void SetResult(T result)
        {
            
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(
            ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            if (awaiter is EnumerableStep<T> step)
            {
                _enum.CurrentStep = step;
                _enum.HasNext = true;
            }
            else
            {
                awaiter.OnCompleted(stateMachine.MoveNext);
            }
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) 
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public IConcatEnumerable<T> Task => _enum;
    }
}
