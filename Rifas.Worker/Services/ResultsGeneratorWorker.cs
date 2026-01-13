using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GCIT.Core.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rifas.Client.Entities;
using Rifas.Client.Mappers;
using Rifas.Client.Repositories.Interfaces;
using Rifas.Client.Common;

namespace Rifas.Worker.Services
{
    public class ResultsGeneratorWorker : BackgroundService
    {
        private readonly ILogger<ResultsGeneratorWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly List<TimeSpan> _schedules = new();
        private readonly bool _runOnStart;
        private readonly int _maxAttempts;

        public ResultsGeneratorWorker(ILogger<ResultsGeneratorWorker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;

            _runOnStart = configuration.GetValue<bool>("RunOnStart", true);
            _maxAttempts = configuration.GetValue<int>("GenerationOptions:MaxAttempts", 10);

            var times = configuration.GetSection("WorkerSchedules").Get<string[]>() ?? Array.Empty<string>();
            foreach (var t in times)
            {
                if (TimeSpan.TryParseExact(t, "hh\\:mm", CultureInfo.InvariantCulture, out var ts) ||
                    TimeSpan.TryParse(t, out ts))
                {
                    _schedules.Add(ts);
                }
                else
                {
                    _logger.LogWarning("Worker schedule value '{value}' could not be parsed; skip.", t);
                }
            }

            _schedules.Sort();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ResultsGeneratorWorker started.");

            if (_runOnStart)
            {
                await SafeRunOnceAsync(stoppingToken);
            }

            if (!_schedules.Any())
            {
                _logger.LogWarning("No schedules configured; worker will exit.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayToNextSchedule();
                _logger.LogInformation("Waiting {delay} until next scheduled run.", delay);
                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException) { break; }

                await SafeRunOnceAsync(stoppingToken);
            }

            _logger.LogInformation("ResultsGeneratorWorker stopping.");
        }

        private TimeSpan GetDelayToNextSchedule()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            foreach (var ts in _schedules)
            {
                var scheduled = today.Add(ts);
                if (scheduled > now)
                {
                    return scheduled - now;
                }
            }

            var next = today.AddDays(1).Add(_schedules[0]);
            return next - now;
        }

        private async Task SafeRunOnceAsync(CancellationToken cancellationToken)
        {
            try
            {
                await RunOnceAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error while running generator.");
            }
        }

        private async Task RunOnceAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting results generation run at {time}.", DateTime.UtcNow);

            using var scope = _serviceProvider.CreateScope();
            var raffleRepo = scope.ServiceProvider.GetService<IRaffleRepository>();
            var resultsRepo = scope.ServiceProvider.GetService<IResultsRepository>();
            var ticketsRepo = scope.ServiceProvider.GetService<ITicketsRepository>();

            if (raffleRepo == null || resultsRepo == null || ticketsRepo == null)
            {
                _logger.LogError("One or more required repositories are not registered. Ensure Rifas.Cliente registrations are added to DI.");
                return;
            }

            var now = DateTime.UtcNow;

            var raffles = await raffleRepo.AllNoTracking(raffle => raffle.IsActive && raffle.CreatedAt <= now && raffle.EndAt.Value >= now).ToListAsync(cancellationToken);

            foreach (var raffle in raffles)
            {
                try
                {
                    if (!raffle.IsActive)
                        continue;

                    if (!raffle.EndAt.HasValue)
                    {
                        _logger.LogDebug("Raffle {raffleId} has no EndAt; skipping.", raffle.Id);
                        continue;
                    }

                    if (raffle.CreatedAt > now || raffle.EndAt.Value < now)
                    {
                        _logger.LogDebug("Raffle {raffleId} not in active date range; skipping.", raffle.Id);
                        continue;
                    }

                    var todayStart = now.Date;
                    var todayEnd = todayStart.AddDays(1);

                    var existsToday = await resultsRepo
                        .AllNoTracking()
                        .AnyAsync(r => r.RaffleId == raffle.Id && r.LotteryDate >= todayStart && r.LotteryDate < todayEnd, cancellationToken);

                    if (existsToday)
                    {
                        _logger.LogInformation("Result already exists for raffle {raffleId} today; skipping.", raffle.Id);
                        continue;
                    }

                    var level = Math.Clamp(raffle.level, 1, 6);
                    var maxByLevel = (int)Math.Pow(10, level) - 1;
                    var top = raffle.TopNUmber.HasValue && raffle.TopNUmber.Value >= 0
                        ? Math.Min(raffle.TopNUmber.Value, maxByLevel)
                        : maxByLevel;

                    var rnd = new Random();
                    string winningNumber = null!;
                    string FirstPlace = null!;
                    string SecondPlace = null!;
                    string ThirdPlace = null!;
                    long numericWinning = -1;

                    var isGuaranteed = raffle.AmountActive.HasValue && raffle.AmountActive.Value > raffle.Sold
                        && raffle.GarantedWinner == true;

                    var attempt = 0;
                    var found = false;

                    while (attempt < _maxAttempts && !found)
                    {
                        attempt++;

                        var num = rnd.Next(0, top + 1);
                        numericWinning = num;
                        winningNumber = num.ToString().PadLeft(level, '0');

                        


                        while(true)
                        {
                            FirstPlace = rnd.Next(0, top + 1).ToString().PadLeft(level, '0');
                            SecondPlace = rnd.Next(0, top + 1).ToString().PadLeft(level, '0');
                            ThirdPlace = rnd.Next(0, top + 1).ToString().PadLeft(level, '0');

                            if (FirstPlace != winningNumber && 
                                SecondPlace != winningNumber &&
                                ThirdPlace != winningNumber &&
                                SecondPlace != FirstPlace && 
                                ThirdPlace != SecondPlace && 
                                ThirdPlace != FirstPlace)
                                break;
                           
                        }

                        var existsResult = await resultsRepo.AllNoTracking()
                            .AnyAsync(r => r.RaffleId == raffle.Id 
                            && (r.WinningNumber == winningNumber || r.FirstPlace == FirstPlace || r.SecondPlace == SecondPlace || r.ThirdPlace == ThirdPlace), cancellationToken);

                        if (existsResult)
                            continue;

                        if (isGuaranteed)
                        {
                            //var sold = await ticketsRepo.AllNoTracking()
                            //    .AnyAsync(t => t.RaffleId == raffle.Id && t.TicketNumber == numericWinning, cancellationToken);
                            //if (sold)
                            //    continue;

                            var ticketsSold = await ticketsRepo.AllNoTracking()
                                .AnyAsync(t => t.RaffleId == raffle.Id && t.TicketNumber == numericWinning);

                            if (ticketsSold)
                                continue;

                            var ticketsSold2 = true;
                            while (ticketsSold2)
                            {
                                num = rnd.Next(0, top + 1);
                                numericWinning = num;
                                winningNumber = num.ToString().PadLeft(level, '0');
                                ticketsSold2 = await ticketsRepo.AllNoTracking()
                                    .AnyAsync(t => t.RaffleId == raffle.Id && t.TicketNumber == numericWinning);

                            }


                        }

                        found = true;
                    }

                    if (!found)
                    {
                        _logger.LogWarning("Could not generate unique winning number for raffle {raffleId} after {attempts} attempts. Skipping.", raffle.Id, _maxAttempts);
                        continue;
                    }

                    var resultEntity = new ResultsEntity
                    {
                        RaffleId = raffle.Id,
                        RaffleNumber = raffle.RaffleNumber,
                        WinningNumber = winningNumber,
                        FirstPlace = FirstPlace,
                        SecondPlace = SecondPlace,
                        ThirdPlace = ThirdPlace,
                        IsActive = true,
                        LotteryDate = now,
                        CreatedAt = now
                    };

                    await resultsRepo.AddAsync(resultEntity);
                    await resultsRepo.SaveChangesAsync();

                    raffle.IsActive = false;

                    await raffleRepo.UpdateAsync(raffle);
                    await raffleRepo.SaveChangesAsync();

                    //en ticketsEntity , actualizar el estado de los tickets del raffle indicando si es ganador, primer lugar, segundo lugar, tercer lugar
                    var tickets = await ticketsRepo.AllNoTracking()
                        .Where(t => t.RaffleId == raffle.Id && t.PurchaseId !=null)
                        .ToListAsync();

                    foreach (var ticket in tickets)
                    {
                        if (ticket.TicketNumber == long.Parse(winningNumber))
                        {
                            ticket.State = TicketStateEnum.Ganador;
                            
                        }
                        else if (ticket.TicketNumber == long.Parse(FirstPlace))
                            ticket.State = TicketStateEnum.PrimerLugar;
                        else if (ticket.TicketNumber == long.Parse(SecondPlace))
                            ticket.State = TicketStateEnum.SegundoLugar;
                        else if (ticket.TicketNumber == long.Parse(ThirdPlace))
                            ticket.State = TicketStateEnum.TercerLugar;
                        else
                            ticket.State = TicketStateEnum.Perdedor;

                        await ticketsRepo.UpdateAsync(ticket);
                    }

                    await ticketsRepo.SaveChangesAsync();

                    _logger.LogInformation("Generated result for raffle {raffleId}: {winning} (attempts: {attempts})", raffle.Id, winningNumber, attempt);
                }
                catch (Exception exRaffle)
                {
                    _logger.LogError(exRaffle, "Error generating result for raffle {raffleId}", raffle.Id);
                }
            }

            _logger.LogInformation("Results generation run completed at {time}.", DateTime.UtcNow);
        }
    }
}