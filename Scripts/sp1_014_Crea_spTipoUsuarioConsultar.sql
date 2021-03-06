IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spTipoUsuarioConsultar')
	DROP PROCEDURE [dbo].[spTipoUsuarioConsultar]
GO


-- =============================================
-- Author:	     Antonio Gonzalez
-- Create date:  05/06/2017
-- Description:	 Consultar tipo usuarios
-- =============================================
CREATE PROCEDURE [dbo].[spTipoUsuarioConsultar] 

AS
BEGIN

	SELECT *
	FROM TipoUsuario
	ORDER BY Nombre ASC

END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_014_Crea_spTipoUsuarioConsultar.sql', 'Script que obtiene los tipos de usuario.', GETDATE())
GO