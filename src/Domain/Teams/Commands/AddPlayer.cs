using System;
using Snaelro.Domain.Abstractions;

namespace Snaelro.Domain.Teams.Commands
{
    public class AddPlayer : ITraceable
    {
        public string Name { get; }

        public Guid TeamId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public AddPlayer(string name, Guid teamId, Guid traceId, Guid invokerUserId)
        {
            Name = name;
            TeamId = teamId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }

    }
}