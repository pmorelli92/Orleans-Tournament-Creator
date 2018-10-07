using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Snaelro.Domain.Abstractions;
using Snaelro.Domain.Tournaments.Commands;

namespace Snaelro.Domain.Tournaments.Events
{
    public class TournamentStarted : ITraceable
    {
        public Guid TournamentId { get; }

        public Guid TraceId { get; }

        public Guid InvokerUserId { get; }

        public IImmutableList<Guid> Teams { get; }

        public TournamentStarted(Guid tournamentId, Guid traceId, Guid invokerUserId, IImmutableList<Guid> teams)
        {
            TournamentId = tournamentId;
            TraceId = traceId;
            InvokerUserId = invokerUserId;
            Teams = teams;
        }

        public static TournamentStarted From(StartTournament cmd, IImmutableList<Guid> teams)
            => new TournamentStarted(cmd.TournamentId, cmd.TraceId, cmd.InvokerUserId, teams);
    }
}