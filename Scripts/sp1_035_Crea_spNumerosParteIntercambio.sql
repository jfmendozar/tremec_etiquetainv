IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spNumerosParteIntercambio')
	DROP PROCEDURE [dbo].[spNumerosParteIntercambio]
GO


-- =============================================
-- Author:		Laura K. Centeno Ch.
-- Create date: 18/06/2018
-- Description:	Script que realiza la carga de los Numeros de parte de la carga masiva al catalogo de NumerosParte.
-- =============================================
CREATE PROCEDURE [dbo].[spNumerosParteIntercambio]
@pIdUsuario	INT

AS
BEGIN
  BEGIN TRY

	BEGIN TRAN

	-- DEFINE
	DECLARE  @vTableNumParteRow TABLE (NumParte VARCHAR(30), Descripcion VARCHAR(500), IdUsuario INT)

	-- OBTENER la cantidad total de piezas para el NumParte y Lote especificos que se agregaron en el módulo de Proveedor
	INSERT INTO @vTableNumParteRow (NumParte, Descripcion, IdUsuario)
	SELECT DISTINCT Codigo, Comentario, @pIdUsuario
	FROM NumerosParteRaw
	WHERE Codigo NOT IN (SELECT DISTINCT NumParte FROM NumerosParte)
	ORDER BY Codigo

	-- AGREGA un nuevo registro en la tabla de Producción
	INSERT NumerosParte(NumParte, Descripcion, IdUsuarioActualiza, FechaActualiza)
	SELECT NumParte, Descripcion, @pIdUsuario, GETDATE()
	FROM @vTableNumParteRow
	
	-- LIMPIA la tabla de carga masiva y reinicia el indice en 1
	DELETE FROM NumerosParteRaw
	DBCC CHECKIDENT ('NumerosParteRaw', RESEED, 1);

	-- RESPUESTA
	SELECT 'OK' AS Resultado

	COMMIT TRAN
  END TRY
  BEGIN CATCH
	-- Si existe algun problema revertir todas las transacciones
	IF @@TRANCOUNT > 0
		ROLLBACK TRAN;
	-- DECLARAR variables que guardan los detalles del error
  	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(100), @vIncidente VARCHAR(100), @vEsError BIT
	-- ESTABLECER el valor de las variables de error
  	SET @ErrMsg = ERROR_MESSAGE()
  	SET @ErrSev = ERROR_SEVERITY()
  	SET @ErrSt = ERROR_STATE()
  	SET @vFuncionalidad = 'spNumerosParteIntercambio'
  	SET @vIncidente = 'Error al realizar el intercambio de registros de la carga masiva entre la tabla NumerosParteRaw y NumerosParte.'
  	SET @vEsError = 1
	-- GUARDA log del error
  	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuario
  	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)

  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_035_Crea_spNumerosParteIntercambio.sql', 'Script que realiza la carga de los Numeros de parte de la carga masiva al catalogo de NumerosParte.', GETDATE())
GO