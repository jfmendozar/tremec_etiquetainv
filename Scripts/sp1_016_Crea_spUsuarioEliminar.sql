IF EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME = 'spUsuarioEliminar')
	DROP PROCEDURE [dbo].[spUsuarioEliminar]
GO


-- =============================================
-- Author:	     Antonio Gonzalez
-- Create date:  06/06/2017
-- Description:	 Eliminar usuario
-- =============================================
CREATE PROCEDURE [dbo].[spUsuarioEliminar]
@pIdUsuario INT,
@pIdUsuarioActualiza INT

AS
BEGIN
  BEGIN TRY
	BEGIN TRAN
	
	-- ELIMINA todos los permisos asociados		
	DELETE FROM Permiso 
	WHERE IdUsuario = @pIdUsuario

	-- ACTUALIZA el estado Activo a Inactivo
	UPDATE Usuario SET
		Activo = 0,
		IdPerfil = NULL,
		IdUsuarioActualiza = @pIdUsuarioActualiza,
		FechaActualizado = GETDATE()
	WHERE IdUsuario = @pIdUsuario
	SELECT 'OK'

	COMMIT TRAN
  END TRY
  BEGIN CATCH
	-- CANCELAR movimientos
	IF @@TRANCOUNT > 0
		ROLLBACK TRAN;
	-- DECLARACION de variables
	DECLARE @ErrMsg VARCHAR(MAX), @ErrSev INT, @ErrSt INT, @vFuncionalidad VARCHAR(50), @vIncidente VARCHAR(100), @vEsError BIT
	SET @ErrMsg = ERROR_MESSAGE()
	SET @ErrSev = ERROR_SEVERITY()
	SET @ErrSt = ERROR_STATE()		
	SET @vFuncionalidad = 'Elimiar usuario.'
	SET @vIncidente = 'Error al actualziar datos en la tabla Usuario.'
	SET @vEsError = 1
	-- GUARADAR error en el log de movimientos
	EXEC spLogMovimientoAgregar @vFuncionalidad, @vIncidente, @ErrMsg, @vEsError, @pIdUsuarioActualiza
	RAISERROR(@ErrMsg,@ErrSev,@ErrSt)
  END CATCH
END
GO

-- GUARDAR el log del script creado
INSERT INTO LogActualizacion(NombreScript, Descripcion, FechaEjecucion)
VALUES('sp1_016_Crea_spUsuarioEliminar.sql', 'Script que actualiza el estado del usuario a Inactivo y elimina los permisos asignados.', GETDATE())
GO