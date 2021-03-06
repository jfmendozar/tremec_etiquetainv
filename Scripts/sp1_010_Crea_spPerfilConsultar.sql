IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spPerfilConsultar')
	DROP PROCEDURE [dbo].[spPerfilConsultar]
GO


-- =============================================
-- Author:	     Antonio Gonzalez
-- Create date:  05/06/2017
-- Description:	 Consultar perfiles
-- =============================================
CREATE PROCEDURE [dbo].[spPerfilConsultar] 
AS
BEGIN
	SELECT *
	FROM Perfil
	ORDER BY Nombre ASC
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_010_Crea_spPerfilConsultar.sql', 'Script que obtiene los datos de todos los perfiles.', GETDATE())
GO