using Rifas.Client.Common;
using Rifas.Client.Entities;
using Rifas.Client.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Rifas.Client.Mappers
{
    public static class EntityDtoMapperExtensions
    {
        // Raffle
        public static RaffleDTO ToDto(this RaffleEntity src)
        {
            if (src == null) return new RaffleDTO();
            return new RaffleDTO
            {
                Id = src.Id,
                RaffleNumber = src.RaffleNumber,
                level = src.level,
                TopNUmber = src.TopNUmber,
                AmountActive = src.AmountActive,
                BottomNumber = src.BottomNumber,
                WinnersNumber = src.WinnersNumber ?? 1,
                GarantedWinner = src.GarantedWinner,
                ImageUrl = src.ImageUrl,
                ImageFile = src.ImageFile,
                Title = src.Title,
                Description = src.Description,
                Sold = src.Sold,
                Total = src.Total,
                Price = src.Price,
                TotalTickets = src.TotalTickets,
                Participants = src.Participants,
                Organizer = src.Organizer,
                OrganizerRating = src.OrganizerRating,
                OrganizerRatingCount = src.OrganizerRatingCount,
                Category = src.CategoryEntity?.ToDto(),  
                Tickets = src.Tickets?.ToDtoList(),
                //Purchases = src.Purchases?.ToDtoList(),
                IsActive = src.IsActive,
                CreatedAt = src.CreatedAt.ToUniversalTime(),
                StartedAt = src.StartedAt.HasValue ? src.StartedAt.Value.ToUniversalTime() : DateTime.MinValue.ToUniversalTime(),
                EndAt = src.EndAt.HasValue ? src.EndAt.Value.ToUniversalTime() : DateTime.MinValue.ToUniversalTime(),                
            };
        }

        public static RaffleEntity ToEntity(this RaffleDTO src)
        {
            if (src == null) return new RaffleEntity();
            return new RaffleEntity
            {
                Id = src.Id ?? 0,
                RaffleNumber = src.RaffleNumber ?? string.Empty,
                level = src.level,
                TopNUmber = src.TopNUmber,
                AmountActive = src.AmountActive,
                BottomNumber = src.BottomNumber,
                GarantedWinner = src.GarantedWinner,
                WinnersNumber = src.WinnersNumber,
                ImageUrl = src.ImageUrl,
                ImageFile = src.ImageFile,
                Title = src.Title,
                Description = src.Description,
                Sold = src.Sold,
                Total = src.Total,
                Price = src.Price,
                TotalTickets = src.TotalTickets,
                Participants = src.Participants,
                Organizer = src.Organizer,
                OrganizerRating = src.OrganizerRating,
                OrganizerRatingCount = src.OrganizerRatingCount,
                Category = src.Category != null ? src.Category.Id ?? 0 : 0,
                Tickets = src.Tickets?.ToEntityList(),
                Purchases = src.Purchases?.ToEntityList(),
                IsActive = src.IsActive,
                CreatedAt = src.CreatedAt,
                StartedAt = src.StartedAt,
                EndAt = src.EndAt
            };
        }

        public static List<RaffleDTO> ToDtoList(this IEnumerable<RaffleEntity> source)
            => source?.Select(x => x.ToDto()).ToList() ?? new List<RaffleDTO>();

        public static List<RaffleEntity> ToEntityList(this IEnumerable<RaffleDTO> source)
            => source?.Select(x => x.ToEntity()).ToList() ?? new List<RaffleEntity>();

        // Category
        public static CategoryDTO ToDto(this CategoryEntity src)
        {
            if (src == null) return new CategoryDTO();
            return new CategoryDTO
            {
                Id = src.Id,
                Name = src.Name,
                Description = src.Description,
                IsActive = src.IsActive
            };
        }

        public static CategoryEntity ToEntity(this CategoryDTO src)
        {
            if (src == null) return new CategoryEntity();
            return new CategoryEntity
            {
                Id = src.Id ?? 0,
                Name = src.Name,
                Description = src.Description,
                IsActive = src.IsActive
            };
        }

        public static List<CategoryDTO> ToDtoList(this IEnumerable<CategoryEntity> source)
            => source?.Select(x => x.ToDto()).ToList() ?? new List<CategoryDTO>();

        public static List<CategoryEntity> ToEntityList(this IEnumerable<CategoryDTO> source)
            => source?.Select(x => x.ToEntity()).ToList() ?? new List<CategoryEntity>();

        // Tickets
        public static TicketsDTO ToDto(this TicketsEntity src)
        {
            if (src == null) return new TicketsDTO();
            return new TicketsDTO
            {
                Id = src.Id,
                RaffleId = src.RaffleId,
                UserId = src.UserId,
                TicketNumber = src.TicketNumber,
                BuyedDate = src.BuyedDate.HasValue ? src.BuyedDate.Value.ToUniversalTime() : DateTime.MinValue.ToUniversalTime(),
                Status = src.Status,                  
                State = src.State,
                StatusDescription = src.StatusDescription,
                StatusDate = src.StatusDate.ToUniversalTime(),
                CreatedAt = src.CreatedAt.ToUniversalTime(),
                PurchaseId = src.PurchaseId
            };
        }


        public static TicketsListadoDTO ToListadoTicketsDto(this TicketsEntity src)
        {
            if (src == null) return new TicketsListadoDTO();
            return new TicketsListadoDTO
            {
                RaffleId = src.RaffleId,                
                TicketNumber = src.TicketNumber,                
                Status = src.Status.GetDisplayName(),
                State = src.State.GetDisplayName(),
                Note = src.StatusDescription,
                Category = src.Raffle?.Category ?? 0,
                RaffleName = src.Raffle?.Title ?? string.Empty,
                Purchase = src.Purchase != null ? src.Purchase.ToDto() : null,
                RaffleImage = src.Raffle?.ImageUrl ?? string.Empty,                
                PurchasedAt = src.BuyedDate.HasValue ? src.BuyedDate.Value.ToUniversalTime() : DateTime.MinValue.ToUniversalTime(),
                PurchaseId = src.PurchaseId
            };
        }

        public static TicketsEntity ToEntity(this TicketsDTO src)
        {
            if (src == null) return new TicketsEntity();
            return new TicketsEntity
            {
                Id = src.Id ?? 0,
                RaffleId = src.RaffleId,
                UserId = src.UserId,
                TicketNumber = src.TicketNumber,
                BuyedDate = src.BuyedDate,
                Status = src.Status,
                State = src.State,
                StatusDescription = src.StatusDescription,
                StatusDate = src.StatusDate,
                CreatedAt = src.CreatedAt,
                PurchaseId = src.PurchaseId,

            };
        }

        public static List<TicketsDTO> ToDtoList(this IEnumerable<TicketsEntity> source)
            => source?.Select(x => x.ToDto()).ToList() ?? new List<TicketsDTO>();

        public static List<TicketsEntity> ToEntityList(this IEnumerable<TicketsDTO> source)
            => source?.Select(x => x.ToEntity()).ToList() ?? new List<TicketsEntity>();

        // Transactions
        public static TransactionsDTO ToDto(this TransactionsEntity src)
        {
            if (src == null) return new TransactionsDTO();
            return new TransactionsDTO
            {
                Id = src.Id,
                RaffleId = src.RaffleId,
                TicketNumber = src.TicketNumber,
                UserId = src.UserId,
                User = src.User,
                AgenteId = src.AgenteId,
                Agente = src.Agente,
                IP = src.IP,
                Transaction = src.Transaction,
                Date = src.Date,
                PlayerBalance = src.PlayerBalance,
                Amount = src.Amount,
                Action = src.Action,
                Description = src.Description,
                RestMethod = src.RestMethod,
                JsonRequest = src.JsonRequest,
                CreatedAt = src.CreatedAt.ToUniversalTime()
            };
        }

        public static TransactionsEntity ToEntity(this TransactionsDTO src)
        {
            if (src == null) return new TransactionsEntity();
            return new TransactionsEntity
            {
                Id = src.Id ?? 0,
                RaffleId = src.RaffleId,
                TicketNumber = src.TicketNumber,
                UserId = src.UserId,
                User = src.User,
                AgenteId = src.AgenteId,
                Agente = src.Agente,
                IP = src.IP,
                Transaction = src.Transaction,
                Date = src.Date,
                PlayerBalance = src.PlayerBalance,
                Amount = src.Amount,
                Action = src.Action,
                Description = src.Description,
                RestMethod = src.RestMethod,
                JsonRequest = src.JsonRequest,
                CreatedAt = src.CreatedAt.ToUniversalTime()
            };
        }

        public static List<TransactionsDTO> ToDtoList(this IEnumerable<TransactionsEntity> source)
            => source?.Select(x => x.ToDto()).ToList() ?? new List<TransactionsDTO>();

        public static List<TransactionsEntity> ToEntityList(this IEnumerable<TransactionsDTO> source)
            => source?.Select(x => x.ToEntity()).ToList() ?? new List<TransactionsEntity>();

        // Purchase
        public static PurchaseDTO ToDto(this PurchaseEntity src)
        {
            if (src == null) return new PurchaseDTO();
            return new PurchaseDTO
            {
                Id = src.Id,
                UserId = src.UserId,
                RaffleId = src.RaffleId,
                Quantity = src.Quantity,
                TotalAmount = src.TotalAmount,
                PurchaseDate = src.PurchaseDate.ToUniversalTime(),
                IsActive = src.IsActive,
                Tickets = src.Tickets?.ToDtoList() ?? new List<TicketsDTO>()
            };
        }

        public static PurchaseEntity ToEntity(this PurchaseDTO src)
        {
            if (src == null) return new PurchaseEntity();
            return new PurchaseEntity
            {
                Id = src.Id ?? 0,
                UserId = src.UserId,
                RaffleId = src.RaffleId,
                RaffleNumber = string.Empty,
                Quantity = src.Quantity,
                TotalAmount = src.TotalAmount,
                PurchaseDate = src.PurchaseDate,
                IsActive = src.IsActive
            };
        }

        public static List<PurchaseDTO> ToDtoList(this IEnumerable<PurchaseEntity> source)
            => source?.Select(x => x.ToDto()).ToList() ?? new List<PurchaseDTO>();

        public static List<PurchaseEntity> ToEntityList(this IEnumerable<PurchaseDTO> source)
            => source?.Select(x => x.ToEntity()).ToList() ?? new List<PurchaseEntity>();

        // Results
        public static ResultsDTO ToDto(this ResultsEntity src)
        {
            if (src == null) return new ResultsDTO();
            return new ResultsDTO
            {
                Id = src.Id,
                RaffleId = src.RaffleId,
                RaffleNumber = src.RaffleNumber,
                WinningNumber = src.WinningNumber,
                FirstPlace = src.FirstPlace,
                SecondPlace = src.SecondPlace,
                ThirdPlace = src.ThirdPlace,
                IsActive = src.IsActive,
                LotteryDate = src.LotteryDate.ToUniversalTime(),
                CreatedAt = src.CreatedAt.ToUniversalTime(),
                Raffle = src.Raffle?.ToDto(),
                Ticket = src.Ticket?.ToDto()
            };
        }

        public static ResultsEntity ToEntity(this ResultsDTO src)
        {
            if (src == null) return new ResultsEntity();
            return new ResultsEntity
            {
                Id = src.Id ?? 0,
                RaffleId = src.RaffleId,
                RaffleNumber = src.RaffleNumber,
                WinningNumber = src.WinningNumber,
                FirstPlace = src.FirstPlace,
                SecondPlace = src.SecondPlace,
                ThirdPlace = src.ThirdPlace,
                IsActive = src.IsActive,
                LotteryDate = src.LotteryDate,
                CreatedAt = src.CreatedAt
            };
        }

        public static List<ResultsDTO> ToDtoList(this IEnumerable<ResultsEntity> source)
            => source?.Select(x => x.ToDto()).ToList() ?? new List<ResultsDTO>();

        public static List<ResultsEntity> ToEntityList(this IEnumerable<ResultsDTO> source)
            => source?.Select(x => x.ToEntity()).ToList() ?? new List<ResultsEntity>();
    }
}
