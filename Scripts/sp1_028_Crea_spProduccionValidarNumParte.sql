IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spProduccionValidarNumParte')
	DROP PROCEDURE [dbo].[spProduccionValidarNumParte]
GO


-- =============================================
-- Author:		Laura K. Centeno Ch.
-- Create date: 04/06/2018
-- Description:	Script que valida si el Número de Parte existe con ese Lote en la tabla Produccion
-- History
-- <13/06/2018> <jf.mendoza> <Se agregó el campo de pIdUsuarioActualiza y el parametro Resultado en SELECT y se cambio el valor OK por NOEXISTE>
-- =============================================
CREATE PROCEDURE [dbo].[spProduccionValidarNumParte]
@pNumParte	VARCHAR(30),
@pLote		VARCHAR(30),
@pIdUsuarioActualiza INT
AS
BEGIN
  BEGIN TRY

	-- DEFINE
	DECLARE  @vIdNumParte INT

	-- OBTIENE
	SET @vIdNumParte = (SELECT IdNumParte FROM NumerosParte WHERE UPPER(NumParte) = UPPER(@pNumParte))

    -- VALIDA si el Numero de Parte existe cuando @pContinuar es false
	IF EXISTS(SELECT IdNumParte FROM Produccion WHERE IdNumParte = @vIdNumParte AND UPPER(Lote) = UPPER(@pLote))
	  BEGIN
		SELECT 'EXISTE' Resultado
	  END
	ELSE 
	  BEGIN
		SELECT 'NOEXISTE' Resultado
	  END

  END TRY
  BEGIN CATCH

	-- DECLARAR variables que guardan los detalles del error
  	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(100), @vIncidente VARCHAR(100), @vEsError BIT
	-- ESTABLECER el valor de las variables de error
  	SET @ErrMsg = ERROR_MESSAGE()
  	SET @ErrSev = ERROR_SEVERITY()
  	SET @ErrSt = ERROR_STATE()
  	SET @vFuncionalidad = 'spProduccionValidarNumParte'
  	SET @vIncidente = 'Error al validar si un Números de parte existe con el Lote.'
  	SET @vEsError = 1
	-- GUARDA log del error
  	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
  	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)

  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_028_Crea_spProduccionValidarNumParte.sql', 'Script que valida si el Número de Parte existe con ese Lote en la tabla Produccion.', GETDATE())
GO