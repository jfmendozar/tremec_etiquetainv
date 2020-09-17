IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spProduccionAgregarProveedor')
	DROP PROCEDURE [dbo].[spProduccionAgregarProveedor]
GO


-- =============================================
-- Author:		Francisco Mendoza
-- Create date: 13/06/2018
-- Description:	Script que registra los datos de proveedor y los carga en Producción
-- =============================================
CREATE PROCEDURE spProduccionAgregarProveedor
@pNumParte	VARCHAR(30),
@pLote		VARCHAR(30),
@pCantidad	INT,
@pTipoEtiqueta VARCHAR(20),
@pIdUsuario	INT

AS
BEGIN
	BEGIN TRY
		DECLARE  @vIdNumParte INT = NULL

		SET @vIdNumParte = (SELECT IdNumParte FROM NumerosParte WHERE UPPER(NumParte) = UPPER(@pNumParte))
	    
		IF(@vIdNumParte IS NOT NULL)
		  BEGIN
			INSERT Produccion (IdNumParte, Cantidad, Lote, TipoEtiqueta, IdUsuarioActualiza, FechaActualiza,IdModulo)
			VALUES (@vIdNumParte, @pCantidad, @pLote, @pTipoEtiqueta, @pIdUsuario, GETDATE(),8)
			SELECT 'OK' AS Resultado
		  END
		ELSE
		  BEGIN
			SELECT 'NOEXISTE' AS Resultado
		  END
	END TRY
	BEGIN CATCH

	-- DECLARAR variables que guardan los detalles del error
  	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(100), @vIncidente VARCHAR(100), @vEsError BIT
	-- ESTABLECER el valor de las variables de error
  	SET @ErrMsg = ERROR_MESSAGE()
  	SET @ErrSev = ERROR_SEVERITY()
  	SET @ErrSt = ERROR_STATE()
  	SET @vFuncionalidad = 'spProduccionAgregarProveedor'
  	SET @vIncidente = 'Error al crear un registro en la tabla de produccion.'
  	SET @vEsError = 1
	-- GUARDA log del error
  	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuario
  	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)

  END CATCH
END
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_034_Crea_spProduccionAgregarProveedor.sql', 'Script que agrega el registro de datos del Módulo Provedor en Producción.', GETDATE())
GO
