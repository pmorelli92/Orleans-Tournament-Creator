using System;
using Snaelro.Domain.Abstractions;

namespace Snaelro.Domain.Teams.Commands
{
    public class CreateTeam : ITraceable
    {
        public string Name { get; }

        public Guid TeamId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public CreateTeam(string name, Guid teamId, Guid traceId, Guid invokerUserId)
        {
            Name = name;
            TeamId = teamId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
        }
    }
}