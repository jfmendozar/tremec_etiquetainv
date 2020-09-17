IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spProduccionAgregar')
	DROP PROCEDURE [dbo].[spProduccionAgregar]
GO


-- =============================================
-- Author:		Laura K. Centeno Ch.
-- Create date: 04/06/2018
-- Description:	Script que crea el un registro en la tabla Produccion
-- History:
-- <11-06-2018> <lk.centeno> <Se agrega validación antes de registrar desde el módulo Interno>
-- =============================================
CREATE PROCEDURE [dbo].[spProduccionAgregar]
@pNumParte	VARCHAR(30),
@pLote		VARCHAR(30),
@pCantidad	INT,
@pTipoEtiqueta VARCHAR(20),
@pIdModulo	INT,
@pIdUsuario	INT

AS
BEGIN
  BEGIN TRY

	-- DEFINE
	DECLARE  @vIdNumParte INT = NULL, @vTotalProveedor INT = 0

	-- OBTIENE el ID del numero de Parte
	SET @vIdNumParte = (SELECT IdNumParte FROM NumerosParte WHERE UPPER(NumParte) = UPPER(@pNumParte))

	IF(@vIdNumParte IS NULL)
	  BEGIN
		SELECT 'NOEXISTE' AS Resultado
		RETURN;
	  END

	-- OBTENER la cantidad total de piezas para el NumParte y Lote especificos que se agregaron en el módulo de Proveedor
	SELECT @vTotalProveedor = SUM(P.Cantidad)
	FROM Produccion P
	WHERE P.IdNumParte = @vIdNumParte AND P.Lote = @pLote AND TipoEtiqueta = 'Proveedor'
	GROUP BY P.IdNumParte, P.Lote, P.TipoEtiqueta

	-- VALIDAR que aun exista la cantidad disponible, de lo contrario enviar un mensaje de error.
	IF (@pCantidad > @vTotalProveedor)
	  BEGIN
		SELECT 'La cantidad es mayor a la dispinible (' + CONVERT(varchar(10), @vTotalProveedor) + ') de Proveedor, verifique.' AS Resultado
		RETURN;
	  END

	-- AGREGA un nuevo registro en la tabla de Producción
	INSERT Produccion (IdNumParte, Cantidad, Lote, TipoEtiqueta, IdModulo, IdUsuarioActualiza, FechaActualiza)
	VALUES (@vIdNumParte, @pCantidad, @pLote, @pTipoEtiqueta, @pIdModulo, @pIdUsuario, GETDATE())

	-- RESPUESTA
	SELECT 'OK' AS Resultado

  END TRY
  BEGIN CATCH

	-- DECLARAR variables que guardan los detalles del error
  	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(100), @vIncidente VARCHAR(100), @vEsError BIT
	-- ESTABLECER el valor de las variables de error
  	SET @ErrMsg = ERROR_MESSAGE()
  	SET @ErrSev = ERROR_SEVERITY()
  	SET @ErrSt = ERROR_STATE()
  	SET @vFuncionalidad = 'spProduccionAgregar'
  	SET @vIncidente = 'Error al crear un registro en la tabla de produccion.'
  	SET @vEsError = 1
	-- GUARDA log del error
  	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuario
  	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)

  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_029_Crea_spProduccionAgregar.sql', 'Script que CREA el registro en la tabla Produccion.', GETDATE())
GO