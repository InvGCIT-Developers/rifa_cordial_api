-- Script de inserción generado desde el JSON proporcionado.
-- CreatedAt usa @now; EndAt usa DATEADD(day, N, @now) para replicar isoDaysFromNow(N).
DECLARE @now datetime2(7) = SYSUTCDATETIME();

INSERT INTO [dbo].[Raffles]
           ([ImageUrl]
           ,[Title]
           ,[Description]
           ,[Sold]
           ,[Total]
           ,[Price]
           ,[TotalTickets]
           ,[Participants]
           ,[Organizer]
           ,[OrganizerRating]
           ,[OrganizerRatingCount]
           ,[Category]
           ,[IsActive]
           ,[CreatedAt]
           ,[EndAt])
     VALUES
-- 1
('assets/img/banner2.jpeg',
 'Gran Rifa Cordialito',
 'Participa en la gran rifa y gana increíbles premios.',
 50, 1000, 5.00, 1000, 12, 'Jhonson', 0, 0, 1, 1, @now, DATEADD(day, 5, @now)),
-- 2
('assets/img/banner3.png',
 'Rifa Especial Verano',
 'Disfruta del verano con esta rifa especial.',
 450, 1000, 10.00, 1000, 300, 'Raafael', 0, 0, 2, 0, @now, DATEADD(day, -2, @now)),
-- 3
('assets/img/banner4.avif',
 'Rifa Última Oportunidad',
 'No te pierdas la última oportunidad de ganar.',
 900, 1000, 20.00, 1000, 850, 'Ford', 0, 0, 3, 1, @now, DATEADD(day, 2, @now)),
-- 4
('assets/img/banner2.jpeg',
 'Rifa Primavera',
 'Celebra la primavera con esta rifa especial.',
 100, 1000, 15.00, 1000, 50, 'Rio', 0, 0, 1, 0, @now, DATEADD(day, -5, @now)),
-- 5
('assets/img/banner.jpeg',
 'Rifa de Verano',
 'Gana premios increíbles este verano.',
 600, 1000, 8.00, 1000, 400, 'Mcdonalds', 0, 0, 2, 1, @now, DATEADD(day, 7, @now)),
-- 5b
('assets/img/banner4.avif',
 'Rifa Agotada - Inscripciones Cerradas',
 'Todos los boletos se vendieron, pero el sorteo aún no se celebra.',
 1000, 1000, 10.00, 1000, 1000, 'Venta Express', 0, 0, 3, 1, @now, DATEADD(day, 3, @now)),
-- 6
('assets/img/banner2.jpeg',
 'Rifa Caducada - Reciente',
 'Rifa que ya finalizó recientemente.',
 1000, 1000, 5.00, 1000, 980, 'Grupo Cordialito', 0, 0, 4, 1, @now, DATEADD(day, -1, @now)),
-- 7
('assets/img/banner.jpeg',
 'Rifa Caducada - Mes Pasado',
 'Evento terminado, ya no disponible.',
 750, 1000, 12.00, 1000, 700, 'Republica de Venezuela', 0, 0, 1, 0, @now, DATEADD(day, -30, @now)),
-- 8
('assets/img/banner2.jpeg',
 'Rifa Caducada - Año Pasado',
 'Rifa del año anterior, cerrada.',
 1000, 1000, 20.00, 1000, 1000, 'Anónimo', 0, 0, 3, 1, @now, DATEADD(day, -365, @now))
;
GO