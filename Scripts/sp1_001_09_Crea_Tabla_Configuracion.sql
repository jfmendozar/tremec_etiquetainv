-- =============================================
-- Author:		Laura K. Centeno Ch.
-- Create date: 20/07/2018
-- Description:	Script que crea la tabla de configuración
-- =============================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Configuracion](
	[IdConfiguracion] [tinyint] NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Descripcion] [varchar](500) NULL,
	[Valor] [varchar](4000) NULL,
	[FechaActualizado] [datetime] NOT NULL CONSTRAINT [DF_ConfigParam_FechaActualizado]  DEFAULT (getdate()),
	[IdUsuarioActualiza] [int] NOT NULL,
	[Visible] [bit] NOT NULL,
	[ExpresionRegular] [varchar](200) NULL,
	[Categoria] [varchar](50) NULL,
 CONSTRAINT [PK_Configuracion] PRIMARY KEY CLUSTERED 
(
	[IdConfiguracion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Configuracion]  WITH CHECK ADD  CONSTRAINT [FK_Configuracion_Usuario] FOREIGN KEY([IdUsuarioActualiza])
REFERENCES [dbo].[Usuario] ([IdUsuario])
GO

ALTER TABLE [dbo].[Configuracion] CHECK CONSTRAINT [FK_Configuracion_Usuario]
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_001_09_Crea_Tabla_Configuracion.sql', 'Script que crea la tabla de configuración.', GETDATE())
GO