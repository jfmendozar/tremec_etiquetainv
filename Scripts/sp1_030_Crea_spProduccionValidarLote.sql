IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spProduccionValidarLote')
	DROP PROCEDURE [dbo].[spProduccionValidarLote]
GO


-- =============================================
-- Author:		Laura K. Centeno Ch.
-- Create date: 29/05/2018
-- Description:	Script que obtriene las existencias del Numero de Parte y Lote.
-- History:
-- <11-06-2018> <lk.centeno> <Se agrega alias Resultado al select.>
-- =============================================
CREATE PROCEDURE [dbo].[spProduccionValidarLote]
@pNumParte	VARCHAR(30),
@pLote		VARCHAR(30)

AS
BEGIN
  BEGIN TRY

	-- DEFINIR VARIABLES
	DECLARE @vTotalProveedor INT = 0, @vTotalInterno INT = 0

	-- VALIDAR si existe, si no existe enviar mensaje al usuario indicando que NO EXISTE y romper el flujo.
	IF NOT EXISTS(
			SELECT P.Lote
			FROM Produccion P
			INNER JOIN NumerosParte N ON N.IdNumParte = P.IdNumParte
			WHERE N.NumParte = @pNumParte AND P.Lote = @pLote AND TipoEtiqueta = 'Proveedor'
			)
	  BEGIN
		SELECT 'NOEXISTE' AS Resultado
		RETURN;
	  END

	-- OBTENER la cantidad total de piezas para el NumParte y Lote especificos que se agregaron en el módulo de Proveedor
	SELECT @vTotalProveedor = SUM(Cantidad)
	FROM Produccion P
	INNER JOIN NumerosParte N ON N.IdNumParte = P.IdNumParte
	WHERE N.NumParte = @pNumParte AND P.Lote = @pLote AND TipoEtiqueta = 'Proveedor'
	GROUP BY P.IdNumParte, P.Lote, P.TipoEtiqueta

	-- OBTENER la cantidad total de piezas para el NumParte y Lote especificos que se agregaron en el módulo de Proveedor
	SELECT @vTotalInterno = SUM(Cantidad)
	FROM Produccion P
	INNER JOIN NumerosParte N ON N.IdNumParte = P.IdNumParte
	WHERE N.NumParte = @pNumParte AND P.Lote = @pLote AND TipoEtiqueta = 'Interno'
	GROUP BY P.IdNumParte, P.Lote, P.TipoEtiqueta

	-- OBTENER la cantidad disponible
	SELECT (@vTotalProveedor - @vTotalInterno) AS Resultado

  END TRY
  BEGIN CATCH

	-- DECLARAR variables que guardan los detalles del error
  	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(100), @vIncidente VARCHAR(100), @vEsError BIT
	-- ESTABLECER el valor de las variables de error
  	SET @ErrMsg = ERROR_MESSAGE()
  	SET @ErrSev = ERROR_SEVERITY()
  	SET @ErrSt = ERROR_STATE()
  	SET @vFuncionalidad = 'spProduccionValidarLote'
  	SET @vIncidente = 'Error al obtener las piezas disponibles del lote.'
  	SET @vEsError = 1
	-- GUARDA log del error
  	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, NULL
  	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)

  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_030_Crea_spProduccionValidarLote.sql', 'Script que obtriene las existencias del Numero de Parte y Lote.', GETDATE())
GO