-- ============================================= 
-- Author:      Laura K. Centeno Ch.
-- Create date: <30/05/2018> 
-- Description: Crea la tabla donde se almacenaran los datos crudos de los Números de Parte, de la carga masiva.
--				Para después ser procesados por un script que valide los datos y la información.
-- =============================================

/****** Object:  Table [dbo].[NumerosParteRaw]    Script Date: 30/05/2018 13:44:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[NumerosParteRaw](
	[IdRegistro] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [varchar](30) NOT NULL,
	[Comentario] [varchar](500) NULL,
 CONSTRAINT [PK_NumerosParteRaw] PRIMARY KEY CLUSTERED 
(
	[IdRegistro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Codigo_NumerosParteRaw] UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_001_03_Crea_Tabla_NumerosParteRaw.sql', 'Script que crea la tabla donde se almacenaran los datos crudos de los Números de Parte, de la carga masiva.', GETDATE())
GO