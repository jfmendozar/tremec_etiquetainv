-- ============================================= 
-- Author:      Laura K. Centeno Ch.
-- Create date: <21/06/2018>
-- Description: Agregar llave primaria a la tabal produccion y no permitir el IdNumParte Nulo
-- ============================================= 

-- ELIMINA la tabla si existe
BEGIN
  IF EXISTS(SELECT object_id FROM sys.tables WHERE name = N'Produccion')
	BEGIN
	  DROP TABLE [dbo].[Produccion]
	END
END
GO

-- CREA la tabla si no existe
BEGIN
  IF NOT EXISTS(SELECT object_id FROM sys.tables WHERE name = N'Produccion')
	BEGIN
	  CREATE TABLE [dbo].[Produccion](
	  	[IdProduccion] [int] IDENTITY(1,1) NOT NULL,
	  	[IdNumParte] [varchar](30) NOT NULL,
	  	[Cantidad] [int] NULL,
	  	[FechaActualiza] [datetime] NULL,
	  	[IdUsuarioActualiza] [nchar](10) NULL,
	  	[Lote] [varchar](30) NULL,
	  	[TipoEtiqueta] [varchar](30) NULL,
	  	[IdModulo] [int] NULL,
	   CONSTRAINT [PK_Produccion] PRIMARY KEY CLUSTERED
	  (
	  	[IdProduccion] ASC
	  )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	  ) ON [PRIMARY]
	END
END
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_001_07_Modifica_Tabla_Produccion.sql', 'Script que agrega la llave primaria a la tabla Produccion.', GETDATE())
GO