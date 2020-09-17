IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spNumerosParteConsultar')
	DROP PROCEDURE [dbo].[spNumerosParteConsultar]
GO

-- =============================================
-- Author:		Francisco Mendoza
-- Create date: 23/05/18
-- Description:	Consultar la tabla NumerosParte
-- History:
-- <30-05-2018> <lk.centeno> <Cambiar consulta porque se actualizo la estructura de la Tabla>
-- <19-06-2018> <lk.centeno> <Agregar en la consulta el nombre del usuario que modifica el registro.>
-- =============================================
CREATE PROCEDURE spNumerosParteConsultar

AS
BEGIN
	-- OBTIENEN los registros de la tabla NumerosParte
	SELECT NP.IdNumParte, NP.NumParte, NP.Descripcion, U.Nombre AS Usuario, NP.FechaActualiza
	FROM NumerosParte NP
	INNER JOIN Usuario U ON U.IdUsuario = NP.IdUsuarioActualiza
	ORDER BY NumParte
END
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_023_Crea_spNumerosParteConsultar.sql', 'Script que obtiene los registros de la tabla NumerosParte.', GETDATE())
GO
