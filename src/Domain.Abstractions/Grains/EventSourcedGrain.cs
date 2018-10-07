using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.EventSourcing;
using Orleans.Streams;
using Snaelro.Domain.Abstractions.Events;

namespace Snaelro.Domain.Abstractions.Grains
{
    public abstract class EventSourcedGrain<TState> : JournaledGrain<TState>
        where TState : class, new()
    {
        private IAsyncStream<object> _stream;
        private readonly StreamOptions _streamOpt;
        protected readonly PrefixLogger PrefixLogger;

        protected EventSourcedGrain(
            StreamOptions streamOpt,
            PrefixLogger prefixLogger)
        {
            _streamOpt = streamOpt;
            PrefixLogger = prefixLogger;
        }

        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(_streamOpt.Name);
            _stream = streamProvider.GetStream<object>(this.GetPrimaryKey(), _streamOpt.Namespace);
            await base.OnActivateAsync();
        }

        protected async Task PersistPublishAsync(object evt)
        {
            RaiseEvent(evt);

            PrefixLogger.LogInformation(
                "handled event of type [{evtType}] for resource id: [{grainId}]", evt.GetType().Name, this.GetPrimaryKey());

            await _stream.OnNextAsync(evt);
        }

        protected async Task PublishErrorAsync(int code, string name, Guid traceId, Guid invokerUserId)
        {
            PrefixLogger.LogInformation(
                "handled error [{code}]-[{name}] for resource id: [{grainId}]", code, name, this.GetPrimaryKey());

            await _stream.OnNextAsync(new ErrorHasOccurred(code, name, traceId, invokerUserId));
        }
    }
}