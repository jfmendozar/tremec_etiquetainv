IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spProduccionObtenerRegistros')
	DROP PROCEDURE [dbo].[spProduccionObtenerRegistros]
GO


-- =============================================
-- Author:		Laura K. Centeno Ch.
-- Create date:	24/05/2018
-- Description:	Obtiene todos los registros de la tabla Producción que se han registrado desde la terminal, para la impresión de etiquetas.
-- History: 
-- <29/05/2018> <jf.mendoza> <Cambio de orden en las columnas y se agregó el campo de lote>
-- <30/05/2018> <lk.centeno> <Se actualiza consulta, por cambio de la estructura de la tabla NumerosParte y la tabla Produccion>
-- =============================================
CREATE PROCEDURE [dbo].[spProduccionObtenerRegistros]
AS
BEGIN
  BEGIN TRY

	-- ===================================================
	-- OBTIENE todos los registros de la tabla Producción. 
	-- ===================================================	
	SELECT N.NumParte, N.Descripcion, P.Lote, P.TipoEtiqueta, P.Cantidad, M.Nombre AS Modulo, U.Nombre AS Usuario, P.FechaActualiza
	FROM Produccion P
	INNER JOIN Usuario U ON U.IdUsuario = P.IdUsuarioActualiza
	INNER JOIN Modulo M ON M.IdModulo = P.IdModulo
	INNER JOIN NumerosParte N ON N.IdNumParte = P.IdNumParte
	ORDER BY P.FechaActualiza DESC

  END TRY
  BEGIN CATCH

	-- DECLARAR variables que guardan los detalles del error
  	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(100), @vIncidente VARCHAR(100), @vEsError BIT
	-- ESTABLECER el valor de las variables de error
  	SET @ErrMsg = ERROR_MESSAGE()
  	SET @ErrSev = ERROR_SEVERITY()
  	SET @ErrSt = ERROR_STATE()
  	SET @vFuncionalidad = 'spProduccionObtenerRegistros'
  	SET @vIncidente = 'Error al obtener los registros de producción.'
  	SET @vEsError = 1
	-- GUARDA log del error
  	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, NULL
  	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)

  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_021_Crea_spProduccionObtenerRegistros.sql', 'Script que Obtiene todos los registros para el Reporte de Producción', GETDATE())
GO
