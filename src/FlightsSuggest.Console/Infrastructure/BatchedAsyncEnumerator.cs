using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightsSuggest.ConsoleApp.Infrastructure
{
    public class BatchedAsyncEnumerator<TItem, TState> : IAsyncEnumerator<TItem>
    {
        private readonly Func<TState, Task<IEnumerable<TItem>>> getNextBatchAsync;
        private readonly Func<TItem, TState> modifyState;

        private IEnumerator<TItem> currentBatch;
        private TState state;

        public BatchedAsyncEnumerator(
            Func<TState, Task<IEnumerable<TItem>>> getNextBatchAsync,
            Func<TItem, TState> modifyState,
            TState initialState
        )
        {
            this.getNextBatchAsync = getNextBatchAsync;
            this.modifyState = modifyState;
            state = initialState;
        }

        public async Task<(bool hasNext, TItem nextItem)> MoveNextAsync()
        {
            if (currentBatch == null || !currentBatch.MoveNext())
            {
                currentBatch = (await getNextBatchAsync(state)).GetEnumerator();
                // ReSharper disable once PossibleMultipleEnumeration
                if (!currentBatch.MoveNext())
                {
                    return (false, default);
                }
            }

            var item = currentBatch.Current;
            state = modifyState(item);
            return (true, item);
        }
    }
}