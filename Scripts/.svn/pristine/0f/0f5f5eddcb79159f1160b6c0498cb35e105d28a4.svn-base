-- ============================================= 
-- Author:      Laura K. Centeno Ch.
-- Create date: <20/06/2018> 
-- Description: Asociar la acción de Consultar al módulo de Carga Masiva
-- ============================================= 

BEGIN
  BEGIN TRY

	DECLARE @vIdModulo INT = NULL, @vIdAccion INT = NULL 

	SELECT @vIdModulo = IdModulo
	FROM Modulo
	WHERE Nombre = 'Carga Masiva'

	SELECT @vIdAccion = IdAccion
	FROM Accion
	WHERE Nombre = 'Consultar'

	IF NOT EXISTS(SELECT TOP 1 * FROM ModuloAccion WHERE IdAccion = @vIdAccion AND IdModulo = @vIdModulo)
	  BEGIN
		INSERT INTO ModuloAccion(IdModulo, IdAccion, FechaActualizado, IdUsuarioActualiza)
		VALUES (@vIdModulo, @vIdAccion, GETDATE(), 1)
	  END

  END TRY
  BEGIN CATCH
	-- DECLARACION de variables
	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(50), @vIncidente VARCHAR(100), @vEsError BIT
	SET @ErrMsg = ERROR_MESSAGE()
	SET @ErrSev = ERROR_SEVERITY()
	SET @ErrSt = ERROR_STATE()		
	SET @vFuncionalidad = 'Agregar accion Consultar a ModuloAccion CargaMasiva.'
	SET @vIncidente = 'Error al insertar datos en la tabla ModuloAccion.'
	SET @vEsError = 1
	-- GUARADAR error en el log de movimientos
	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, NULL
	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
  END CATCH
END  
GO

-- GUARDA el cambio del script en la tabla
INSERT INTO [LogActualizacion](NombreScript, Descripcion, FechaEjecucion)
VALUES('sp_001_05_Inserta_ModuloAccion.sql', 'Script que agrega la accion de Consultar al la tabla ModuloAccion para el modulo Carga Masiva.', GETDATE())
GO