IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spGeneralObtenerFechaHora')
	DROP PROCEDURE [dbo].[spGeneralObtenerFechaHora]
GO

-- =============================================
-- Author:		Laura K. Centeno Ch.
-- Create date: 13/06/2018
-- Description:	Script que obtriene la fecha del servidor.
-- =============================================
CREATE PROCEDURE [dbo].[spGeneralObtenerFechaHora]

AS
BEGIN
  BEGIN TRY 

	-- OBTENER la cantidad disponible
	SELECT FORMAT(GETDATE(), 'dd-MM-yyyy') AS FechaServidor

  END TRY
  BEGIN CATCH

	-- DECLARAR variables que guardan los detalles del error
  	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(100), @vIncidente VARCHAR(100), @vEsError BIT
	-- ESTABLECER el valor de las variables de error
  	SET @ErrMsg = ERROR_MESSAGE()
  	SET @ErrSev = ERROR_SEVERITY()
  	SET @ErrSt = ERROR_STATE()
  	SET @vFuncionalidad = 'spGeneralObtenerFechaHora'
  	SET @vIncidente = 'Error al obtener la fecha y hora del servidor'
  	SET @vEsError = 1
	-- GUARDA log del error
  	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, NULL
  	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)

  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_031_Crea_spGeneralObtenerFechaHora.sql', 'Script que obtiene la fecha y hora del servidor.', GETDATE())
GO